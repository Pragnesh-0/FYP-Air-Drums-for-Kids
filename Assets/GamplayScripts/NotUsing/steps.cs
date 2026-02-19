using System;
using System.Collections.Generic;
using System.IO;
using Unity.InferenceEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Unity.Mathematics;


public class RunYOLO : MonoBehaviour
{
    [Tooltip("Drag a YOLO model .onnx file here")]
    public ModelAsset modelAsset;

    [Tooltip("Drag the classes.txt here")]
    public TextAsset classesAsset;

    [Tooltip("Create a Raw Image in the scene and link it here")]
    public RawImage displayImage;

    [Tooltip("Drag a border box texture here")]
    public Texture2D borderTexture;

    [Tooltip("Select an appropriate font for the labels")]
    public Font font;

    [Tooltip("Change this to the name of the video you put in the Assets/StreamingAssets folder")]
    public string videoFilename = "giraffes.mp4";

    const BackendType backend = BackendType.GPUCompute;

    private Transform displayLocation;
    private Worker worker;
    private string[] labels;
    private RenderTexture targetRT;
    //private Sprite borderSprite;

    //Image size for the model
    private const int imageWidth = 256;
    private const int imageHeight = 256;

    private VideoPlayer video;


    private WebCamTexture webCamTexture;



    public RectTransform stick1;
    public RectTransform stick2;

    List<GameObject> boxPool = new();

    [Tooltip("Intersection over union threshold used for non-maximum suppression")]
    [SerializeField, Range(0, 1)]
    float iouThreshold = 0.5f;

    [Tooltip("Confidence score threshold used for non-maximum suppression")]
    [SerializeField, Range(0, 1)]
    float scoreThreshold = 0.5f;

    Tensor<float> centersToCorners;
    //bounding box data
    public struct BoundingBox
    {
        public float centerX;
        public float centerY;
        public float width;
        public float height;
        public string label;
    }

    void Start()
    {
        
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        //Parse neural net labels
        labels = classesAsset.text.Split('\n');

        LoadModel();

        targetRT = new RenderTexture(imageWidth, imageHeight, 0);

        //Create image to display video
        displayLocation = displayImage.transform;

        SetupInput();

        //borderSprite = Sprite.Create(borderTexture, new Rect(0, 0, borderTexture.width, borderTexture.height), new Vector2(borderTexture.width / 2, borderTexture.height / 2));
    }
    void LoadModel()
    {
        //Load model
        var model1 = ModelLoader.Load(modelAsset);

        centersToCorners = new Tensor<float>(new TensorShape(4, 4),
        new float[]
        {
                    1,      0,      1,      0,
                    0,      1,      0,      1,
                    -0.5f,  0,      0.5f,   0,
                    0,      -0.5f,  0,      0.5f
        });

        //Here we transform the output of the model1 by feeding it through a Non-Max-Suppression layer.
        var graph = new FunctionalGraph();
        var inputs = graph.AddInputs(model1);
        var modelOutput = Functional.Forward(model1, inputs)[0];                        //shape=(1,84,8400)
        var boxCoords = modelOutput[0, 0..4, ..].Transpose(0, 1);               //shape=(8400,4)
        var allScores = modelOutput[0, 4.., ..];                                //shape=(80,8400)
        var scores = Functional.ReduceMax(allScores, 0);                                //shape=(8400)
        var classIDs = Functional.ArgMax(allScores, 0);                                 //shape=(8400)
        var boxCorners = Functional.MatMul(boxCoords, Functional.Constant(centersToCorners));   //shape=(8400,4)
        var indices = Functional.NMS(boxCorners, scores, iouThreshold, scoreThreshold); //shape=(N)
        var coords = Functional.IndexSelect(boxCoords, 0, indices);                     //shape=(N,4)
        var labelIDs = Functional.IndexSelect(classIDs, 0, indices);                    //shape=(N)

        //Create worker to run model
        worker = new Worker(graph.Compile(coords, labelIDs), backend);
    }

    void SetupInput()
    {
        /*video = gameObject.AddComponent<VideoPlayer>();
        video.renderMode = VideoRenderMode.APIOnly;
        video.source = VideoSource.Url;
        video.url = Path.Join(Application.streamingAssetsPath, videoFilename);
        video.isLooping = true;
        video.Play();*/


        webCamTexture = new WebCamTexture
        {
            requestedWidth = 256,
            requestedHeight = 256,
        };

        webCamTexture.Play();



    }

    private void Update()
    {


        ExecuteML();

        //print(1/Time.unscaledDeltaTime);
    }

    public void ExecuteML()
    {
        ClearAnnotations();

        if (webCamTexture)
        {

            float aspect = webCamTexture.width * 1f / webCamTexture.height;
            Graphics.Blit(webCamTexture, targetRT, new Vector2(1, 1), new Vector2(0, 0));
            displayImage.texture = targetRT;
        }
        else return;

        using Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 3, imageHeight, imageWidth));
        TextureConverter.ToTensor(targetRT, inputTensor, default);
        worker.Schedule(inputTensor);

        using var output = (worker.PeekOutput("output_0") as Tensor<float>).ReadbackAndClone();
        using var labelIDs = (worker.PeekOutput("output_1") as Tensor<int>).ReadbackAndClone();

        int displayWidth = Screen.width;
        int displayHeight = Screen.height;

        float scaleX = displayWidth / imageWidth;
        float scaleY = displayHeight / imageHeight;

        int boxesFound = output.shape[0];

        bool stick1Det = false;
        bool stick2Det = false;

        //Debug.Log(boxesFound + " Detected");
        //Draw the bounding boxes
        for (int n = 0; n < Mathf.Min(boxesFound, 2); n++)
        {
            var box = new BoundingBox
            {
                centerX = output[n, 0] * scaleX - displayWidth/2,
                centerY = output[n, 1] * scaleY - displayHeight/2,
                width = output[n, 2] * scaleX,
                height = output[n, 3] * scaleY,
                label = labels[labelIDs[n]],
            };
            //Debug.Log(box);
            //DrawBox(box, n, 4);
            Vector2 a = new(box.centerX, box.centerY);
            Vector2 b = new(stick1.anchoredPosition.x, stick1.anchoredPosition.y);
            Vector2 c = new(stick2.anchoredPosition.x, stick2.anchoredPosition.y);
            a = -a;
            float z = 0.25f;


            float stick1dis = (a - b).magnitude;
            float stick2dis = (a - c).magnitude;

            if (stick1dis <= stick2dis)
            {
                if (!stick1Det)
                {
                    stick1.anchoredPosition = Step(1, Time.time, a);
                    //stick1.anchoredPosition = a;
                    stick1Det = true;
                }
                else
                {
                    if (!stick2Det)
                    {
                        stick2.anchoredPosition = Step(2, Time.time, a);
                        //stick2.anchoredPosition = a;
                        stick2Det = true;
                    }
                }
            }
            else
            {
                if (!stick2Det)
                {
                    stick2.anchoredPosition = Step(2, Time.time, a);
                    //stick2.anchoredPosition = a;
                    stick2Det = true;
                }
                else
                {
                    if (!stick1Det)
                    {
                        stick1.anchoredPosition = Step(1, Time.time, a);
                        //stick1.anchoredPosition = a;
                        stick1Det = true;
                    }
                }
            }

        }
    }

    public void DrawBox(BoundingBox box, int id, float fontSize)
    {
        //Create the bounding box graphic or get from pool
        GameObject panel;
        if (id < boxPool.Count)
        {
            panel = boxPool[id];
            panel.SetActive(true);
        }
        else
        {
            panel = CreateNewBox(Color.blue);
        }
        //Set box position
        panel.transform.localPosition = new Vector3(box.centerX, -box.centerY);

        //Set box size
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(box.width, box.height);

        //Set label text
    }

    public GameObject CreateNewBox(Color color)
    {
        //Create the box and set image

        var panel = new GameObject("ObjectBox");
        panel.AddComponent<CanvasRenderer>();
        Image img = panel.AddComponent<Image>();
        img.color = color;
        //img.sprite = borderSprite;
        img.type = Image.Type.Sliced;
        panel.transform.SetParent(displayLocation, false);

        //Create the label

        var text = new GameObject("ObjectLabel");
        text.AddComponent<CanvasRenderer>();
        text.transform.SetParent(panel.transform, false);
        Text txt = text.AddComponent<Text>();
        txt.font = font;
        txt.color = color;
        txt.fontSize = 40;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;

        RectTransform rt2 = text.GetComponent<RectTransform>();
        rt2.offsetMin = new Vector2(20, rt2.offsetMin.y);
        rt2.offsetMax = new Vector2(0, rt2.offsetMax.y);
        rt2.offsetMin = new Vector2(rt2.offsetMin.x, 0);
        rt2.offsetMax = new Vector2(rt2.offsetMax.x, 30);
        rt2.anchorMin = new Vector2(0, 0);
        rt2.anchorMax = new Vector2(1, 1);

        boxPool.Add(panel);
        return panel;
    }

    public void ClearAnnotations()
    {
        foreach (var box in boxPool)
        {
            box.SetActive(false);
        }
    }

    void OnDestroy()
    {
        centersToCorners?.Dispose();
        worker?.Dispose();
    }



    (float t, Vector2 x, Vector2 deltapos) prev1;
    (float t, Vector2 x, Vector2 deltapos) prev2;


    [SerializeField, Range(0, 1)]
    public float MinCutoff = 0.0f;

    [SerializeField, Range(0, 0.001f)]
    public float Beta = 0.0f;

    public void changeMinCutoff(float val)
    {
        MinCutoff = val;
    }

    public void changeBeta(float val)
    {
        Beta = 0.001f * val;
    }

    float Alpha(float time_elapsed, float cutoff)
    {
        var r = 2 * math.PI * cutoff * time_elapsed;
        return r / (r + 1); 
    }



    public Vector2 Step(int stickv, float t, Vector2 p)
    {
        var a = stickv == 1 ? prev1 : prev2;
        float time_elapsed = t - a.t;

        if (time_elapsed < 1e-5f) return a.x;

        var change = (p - a.x) / time_elapsed;
        var change_res = math.lerp(a.deltapos, change, Alpha(time_elapsed, 1.0f));

        var cutoff = MinCutoff + Beta * math.length(change_res);
        var x_res = math.lerp(a.x, p, Alpha(time_elapsed, cutoff));

        if (stickv == 1)
        {
            prev1.t = t;
            prev1.x = x_res;
            prev1.deltapos = change_res;
        }
        else
        {
            prev2.t = t;
            prev2.x = x_res;
            prev2.deltapos = change_res;
        }


        return x_res;
    }

}