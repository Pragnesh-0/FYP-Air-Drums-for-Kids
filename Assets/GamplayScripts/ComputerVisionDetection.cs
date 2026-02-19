using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.InferenceEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Unity.Mathematics;
using UnityEngine.Android;


public class ComputerVisionDetection : MonoBehaviour
{

    public RawImage showcaseDisplay;

    public Vector2 stick1;
    public Vector2 stick2;

    
    public ModelAsset yoloModel;
    public TextAsset classNames;
    private string[] labels;
    private Worker worker;
    public float iouThreshold = 0.5f;
    public float scoreThreshold = 0.5f;
    const BackendType backend = BackendType.GPUCompute;



    private WebCamTexture wcTexture;
    private RenderTexture targetRT;
    private const int imageWidth = 256; 
    private const int imageHeight = 256; 

    Tensor<float> centersToCorners;
    struct BoundingBox
    {
        public float centerX;
        public float centerY;
        public float width;
        public float height;
        public string label;
    }


    public GameObject notice;
    public Button retryCam;
    public AlertBox alertBox;

    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        labels = classNames.text.Split("\n");
        LoadModel();
        targetRT = new RenderTexture(imageWidth, imageHeight, 0);
        retryCam.onClick.AddListener(delegate{setWebCam();});

    }

    void LoadModel()
    {
        var model1 = ModelLoader.Load(yoloModel);

        centersToCorners = new Tensor<float>(new TensorShape(4, 4),
        new float[]
        {
                    1,      0,      1,      0,
                    0,      1,      0,      1,
                    -0.5f,  0,      0.5f,   0,
                    0,      -0.5f,  0,      0.5f
        });
        var graph = new FunctionalGraph();
        var inputs = graph.AddInputs(model1);
        var modelOutput = Functional.Forward(model1, inputs)[0];
        var boxCoords = modelOutput[0, 0..4, ..].Transpose(0, 1);
        var allScores = modelOutput[0, 4.., ..];
        var scores = Functional.ReduceMax(allScores, 0);
        var classIDs = Functional.ArgMax(allScores, 0);
        var boxCorners = Functional.MatMul(boxCoords, Functional.Constant(centersToCorners));
        var indices = Functional.NMS(boxCorners, scores, iouThreshold, scoreThreshold);
        var coords = Functional.IndexSelect(boxCoords, 0, indices);
        var labelIDs = Functional.IndexSelect(classIDs, 0, indices);


        worker = new Worker(graph.Compile(coords, labelIDs), backend);
    }
    void setWebCam()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            alertBox.alert("This function requires Camera Permission. Please allow Camera access and try again or restart and try again!...");
            notice.SetActive(true);
            Permission.RequestUserPermission(Permission.Camera);
            return;
        }
        if (WebCamTexture.devices.Length == 0)
        {
            alertBox.alert("No Cameras Found..");
            notice.SetActive(true);
            return;
        }
        foreach (var device in WebCamTexture.devices)
        {
            if (device.isFrontFacing)
            {
                wcTexture = new WebCamTexture(device.name, imageWidth, imageHeight);
            }
        }
        if (!wcTexture){wcTexture = new WebCamTexture(imageWidth, imageHeight);}
        wcTexture.Play();
        notice.SetActive(false);
    }


    void Update()
    {
        if(!wcTexture || !wcTexture.isPlaying) return;
        ExecuteML();
    }


    void ExecuteML()
    {
        
        Graphics.Blit(wcTexture, targetRT, new Vector2(1, 1), new Vector2(0, 0));
        showcaseDisplay.texture = targetRT;

        using Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 3, imageHeight, imageWidth));
        TextureConverter.ToTensor(targetRT, inputTensor, default);
        worker.Schedule(inputTensor);

        using var output = (worker.PeekOutput("output_0") as Tensor<float>).ReadbackAndClone();
        using var labelIDs = (worker.PeekOutput("output_1") as Tensor<int>).ReadbackAndClone();
        int boxesFound = output.shape[0];

        bool stick1Det = false;
        bool stick2Det = false; 

        for (int n = 0; n < Mathf.Min(boxesFound, 2); n++)
        {
            var box = new BoundingBox
            {
                centerX = MathF.Abs(1 - (output[n, 0]/imageHeight)),
                centerY = MathF.Abs(1 - (output[n, 1]/imageWidth)),
                width = output[n, 2],
                height = output[n, 3],
                label = labels[labelIDs[n]],
            };

            Vector2 a = new(box.centerX, box.centerY);
            Vector2 b = stick1;
            Vector2 c = stick2;

            float stick1dis = (a - b).magnitude;
            float stick2dis = (a - c).magnitude;


            if (!stick1Det && (stick1dis <= stick2dis))
            {
                stick1Det = true;
                stick1 = Step(1, Time.time, a);
                continue;
            }
            if (!stick2Det && (stick2dis <= stick1dis))
            {
                stick2Det = true;
                stick2 = Step(2, Time.time, a);
                continue;
            }
        
        }
    }



    void OnDestroy()
    {
        centersToCorners?.Dispose();
        worker?.Dispose();
    }

    public void cameraAction(bool val)
    {
        if (val)
        {
            if(wcTexture && wcTexture.isPlaying) return;
            setWebCam();
        }
        else
        {
            notice.SetActive(false);
            if(!wcTexture) return;
            if (wcTexture.isPlaying)
            {
                wcTexture.Stop();
                wcTexture = null;
            }
        }
    }

    public Vector2 getStickPos(int n)
    {
        return n == 0 ? stick1: stick2;
    }



    (float t, Vector2 x, Vector2 deltapos) prev1;
    (float t, Vector2 x, Vector2 deltapos) prev2;
    [SerializeField, Range(0, 1)]
    public float MinCutoff = 0.0f;
    [SerializeField, Range(0, 0.1f)]
    public float Beta = 0.0f;
    public void changeMinCutoff(float val)
    {
        MinCutoff = val;
    }
    public void changeBeta(float val)
    {
        Beta = 0.1f * val;
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
