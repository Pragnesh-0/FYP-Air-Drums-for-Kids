using System;
using Unity.Mathematics;
using Unity.InferenceEngine;
using UnityEngine;
using UnityEngine.UI;

public class HandDetection : MonoBehaviour
{
    //public HandPreview handPreview;
    //public ImagePreview imagePreview;
    public Texture2D imageTexture;


    public RenderTexture imgPrev;
    public ModelAsset handDetector;
    public ModelAsset handLandmarker;
    public TextAsset anchorsCSV;

    public float scoreThreshold = 0.5f;
    public float iouThreshold = 0.3f;
    const int k_NumAnchors = 2016;
    float[,] m_Anchors;

    const int k_NumKeypoints = 21;
    const int detectorInputSize = 192;
    const int landmarkerInputSize = 224;

    Worker m_HandDetectorWorker;
   // Worker m_HandLandmarkerWorker;
    Tensor<float> m_DetectorInput;
    //Tensor<float> m_LandmarkerInput;
    Awaitable m_DetectAwaitable;

    float m_TextureWidth;
    float m_TextureHeight;


    public RectTransform baguette;
    public RectTransform baguette2;


    public WebCamTexture webCamTexture;
    public RawImage img;
    public async void Start()
    {

        webCamTexture = new WebCamTexture
        {
            requestedWidth = 192,
            requestedHeight = 192,
        };

        webCamTexture.Play();

        m_Anchors = BlazeUtils.LoadAnchors(anchorsCSV.text, k_NumAnchors);

        var handDetectorModel = ModelLoader.Load(handDetector);

        // post process the model to filter scores + argmax select the best hand
        var graph = new FunctionalGraph();
        var inputs = graph.AddInput(handDetectorModel,0);
        var outputs = Functional.Forward(handDetectorModel, inputs);
        var boxes = outputs[0]; // (1, 2016, 18)
        var scores = outputs[1]; // (1, 2016, 1)
        Debug.Log(boxes+" "+scores);
        var anchorsData = new float[k_NumAnchors * 4];
        Buffer.BlockCopy(m_Anchors, 0, anchorsData, 0, anchorsData.Length * sizeof(float));
        var anchors = Functional.Constant(new TensorShape(k_NumAnchors, 4), anchorsData);
        var idx_scores_boxes = BlazeUtils.Top10(boxes, scores);
        //handDetectorModel = graph.Compile(idx_scores_boxes.Item1.Item1, idx_scores_boxes.Item1.Item2, idx_scores_boxes.Item1.Item3, idx_scores_boxes.Item2.Item1, idx_scores_boxes.Item2.Item2, idx_scores_boxes.Item2.Item3);
        handDetectorModel = graph.Compile(idx_scores_boxes.Item1, idx_scores_boxes.Item2, idx_scores_boxes.Item3);
        m_HandDetectorWorker = new Worker(handDetectorModel, BackendType.GPUCompute);

        //var handLandmarkerModel = ModelLoader.Load(handLandmarker);
        // m_HandLandmarkerWorker = new Worker(handLandmarkerModel, BackendType.GPUCompute);

        m_DetectorInput = new Tensor<float>(new TensorShape(1, detectorInputSize, detectorInputSize, 3));
        //m_LandmarkerInput = new Tensor<float>(new TensorShape(1, landmarkerInputSize, landmarkerInputSize, 3));

        while (true)
        {
            try
            {
                m_DetectAwaitable = Detect();
                await m_DetectAwaitable;
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        m_HandDetectorWorker.Dispose();
        //m_HandLandmarkerWorker.Dispose();
        m_DetectorInput.Dispose();
        //m_LandmarkerInput.Dispose();
    }



    async Awaitable Detect()
    {
        var texture = webCamTexture;
        m_TextureWidth = texture.width;
        m_TextureHeight = texture.height;
        //imagePreview.SetTexture(texture);

        var size = Mathf.Max(texture.width, texture.height);

        // The affine transformation matrix to go from tensor coordinates to image coordinates
        var scale = size / (float)detectorInputSize;
        var M = BlazeUtils.mul(BlazeUtils.TranslationMatrix(0.5f * (new Vector2(texture.width, texture.height) + new Vector2(-size, size))), BlazeUtils.ScaleMatrix(new Vector2(scale, -scale)));
        BlazeUtils.SampleImageAffine(texture, m_DetectorInput, M);


        img.texture = texture;

        m_HandDetectorWorker.Schedule(m_DetectorInput);

        var outputIdxAwaitable = (m_HandDetectorWorker.PeekOutput(0) as Tensor<int>).ReadbackAndCloneAsync();
        var outputScoreAwaitable = (m_HandDetectorWorker.PeekOutput(1) as Tensor<float>).ReadbackAndCloneAsync();
        var outputBoxAwaitable = (m_HandDetectorWorker.PeekOutput(2) as Tensor<float>).ReadbackAndCloneAsync();
        //var outputIdxAwaitable1 = (m_HandDetectorWorker.PeekOutput(3) as Tensor<int>).ReadbackAndCloneAsync();
        //var outputScoreAwaitable1 = (m_HandDetectorWorker.PeekOutput(4) as Tensor<float>).ReadbackAndCloneAsync();
        //var outputBoxAwaitable1 = (m_HandDetectorWorker.PeekOutput(5) as Tensor<float>).ReadbackAndCloneAsync();

        using var outputIdx = await outputIdxAwaitable;
        using var outputScore = await outputScoreAwaitable;
        using var outputBox = await outputBoxAwaitable;

        //using var outputIdx1 = await outputIdxAwaitable1;
        //using var outputScore1 = await outputScoreAwaitable1;
        //using var outputBox1 = await outputBoxAwaitable1;

        //Debug.Log(outputScore[0]);

        //Debug.Log(outputIdx.shape.length);

        //var scorePassesThreshold = outputScore[0] >= scoreThreshold;
        //handPreview.SetActive(scorePassesThreshold);

        //if (!scorePassesThreshold)
        //return;


        //for (int i = 0; i < outputIdx.shape.length - 1; i++)
        //{

        //var anchorPosition = detectorInputSize * new float2(m_Anchors[idx, 0], m_Anchors[idx, 1]);
        //var kp0_TensorSpace = anchorPosition + new float2(outputBox[0, 0, 4 + 2 * 0 + 0], outputBox[0, 0, 4 + 2 * 0 + 1]);
        //baguette.anchoredPosition = new((1080f / 2f) - (kp0_TensorSpace.x * (1080f / 192f)), (1080f / 2f) - (kp0_TensorSpace.y * (1080f / 192f)));
        //Debug.Log(i+" "+outputScore[i]);
        var firstHand = new Vector2();
        var checkFHand = new Vector2();
        var secondHand = new Vector2();
        for (int i = 0; i <= 19; i++)
        {
            if (outputScore[i] > 0.7)
            {
                var idx = outputIdx[i];
                var anchorPosition = detectorInputSize * new float2(m_Anchors[idx, 0], m_Anchors[idx, 1]);
                //Debug.Log(i + " " + anchorPosition);
                if (firstHand == Vector2.zero)
                {
                    firstHand = new((1080f / 2f) - (anchorPosition.x * (1080f / 192f)), (1080f / 2f) - (anchorPosition.y * (1080f / 192f)));
                    checkFHand = anchorPosition;
                }
                else
                {
                    if ((new Vector2(anchorPosition.x, anchorPosition.y) - checkFHand).magnitude > 10f && secondHand == Vector2.zero)
                    {
                        secondHand = new((1080f / 2f) - (anchorPosition.x * (1080f / 192f)), (1080f / 2f) - (anchorPosition.y * (1080f / 192f)));
                        break;
                    }
                }
            }
            ;
        }



        float dist1 = (baguette.anchoredPosition - firstHand).magnitude;
        float dist2 = (baguette.anchoredPosition - secondHand).magnitude;

        if (dist1 < dist2)
        {
            baguette.anchoredPosition = (firstHand == Vector2.zero)? baguette.anchoredPosition : firstHand;
            baguette2.anchoredPosition = (secondHand == Vector2.zero)? baguette2.anchoredPosition : secondHand;
        }
        else
        { 
            baguette2.anchoredPosition = (firstHand == Vector2.zero)? baguette2.anchoredPosition : firstHand;
            baguette.anchoredPosition = (secondHand == Vector2.zero)? baguette.anchoredPosition : secondHand;
        }

        //Debug.Log(firstHand+" "+secondHand);


        //var idx1 = outputIdx1[0];
        //var anchorPosition1 = detectorInputSize * new float2(m_Anchors[idx1, 0], m_Anchors[idx1, 1]);

        //var boxCentre_TensorSpace = anchorPosition + new float2(outputBox[0, 0, 0], outputBox[0, 0, 1]); 
        //}

        // return;


        //var idx = outputIdx[0];

        //var anchorPosition = detectorInputSize * new float2(m_Anchors[idx, 0], m_Anchors[idx, 1]);





        //var boxCentre_TensorSpace = anchorPosition + new float2(outputBox[0, 0, 0], outputBox[0, 0, 1]);
        // var boxSize_TensorSpace = math.max(outputBox[0, 0, 2], outputBox[0, 0, 3]);





        //var kp0_TensorSpace = anchorPosition + new float2(outputBox[0, 0, 4 + 2 * 0 + 0], outputBox[0, 0, 4 + 2 * 0 + 1]);
        //var kp2_TensorSpace = anchorPosition + new float2(outputBox[0, 0, 4 + 2 * 2 + 0], outputBox[0, 0, 4 + 2 * 2 + 1]);
        //var delta_TensorSpace = kp2_TensorSpace - kp0_TensorSpace;
        //var up_TensorSpace = delta_TensorSpace / math.length(delta_TensorSpace);
        //var theta = math.atan2(delta_TensorSpace.y, delta_TensorSpace.x);
        //var rotation = 0.5f * Mathf.PI - theta;
        //boxCentre_TensorSpace += 0.5f * boxSize_TensorSpace * up_TensorSpace;
        //boxSize_TensorSpace *= 2.6f;




        //baguette.anchoredPosition = new((1080f / 2f) - (kp0_TensorSpace.x * (1080f / 192f)), (1080f / 2f) - (kp0_TensorSpace.y * (1080f / 192f)));
        //Debug.Log((kp2_TensorSpace.y * (720f/192f)));
        //Debug.Log((720f/2f) - (kp2_TensorSpace.y * (720f/192f)));

        //var origin2 = new float2(0.5f * landmarkerInputSize, 0.5f * landmarkerInputSize);
        //var scale2 = boxSize_TensorSpace / landmarkerInputSize;
        //var M2 = BlazeUtils.mul(M, BlazeUtils.mul(BlazeUtils.mul(BlazeUtils.mul(BlazeUtils.TranslationMatrix(boxCentre_TensorSpace), BlazeUtils.ScaleMatrix(new float2(scale2, -scale2))), BlazeUtils.RotationMatrix(rotation)), BlazeUtils.TranslationMatrix(-origin2)));



        /*BlazeUtils.SampleImageAffine(texture, m_LandmarkerInput, M2);

        m_HandLandmarkerWorker.Schedule(m_LandmarkerInput);

        var landmarksAwaitable = (m_HandLandmarkerWorker.PeekOutput("Identity") as Tensor<float>).ReadbackAndCloneAsync();
        using var landmarks = await landmarksAwaitable;

        for (var i = 0; i < k_NumKeypoints; i++)
        {
            var position_ImageSpace = BlazeUtils.mul(M2, new float2(landmarks[3 * i + 0], landmarks[3 * i + 1]));

            Vector3 position_WorldSpace = ImageToWorld(position_ImageSpace) + new Vector3(0, 0, landmarks[3 * i + 2] / m_TextureHeight);
            //handPreview.SetKeypoint(i, true, position_WorldSpace);
        }*/
    }

    void OnDestroy()
    {
        m_DetectAwaitable.Cancel();
    }
}