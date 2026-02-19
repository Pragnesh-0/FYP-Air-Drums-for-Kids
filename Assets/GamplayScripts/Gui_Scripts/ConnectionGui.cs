using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Android;
using TMPro;
using ZXing;

public class ConnectionGui : MonoBehaviour
{

    public Button connectionButton;
    public Button qrButton;
    public TMP_InputField ipText;
    public RawImage output;
    public AlertBox alertBox;

    public LibraryPopGui popGui;



    private RenderTexture rt;
    private WebCamTexture ct;
    private Color32[] c32;
    private Texture2D t2d;
    private IBarcodeReader barcodeReader = new BarcodeReader
    {
        AutoRotate = false,
        Options = new ZXing.Common.DecodingOptions
        {
            TryHarder = false,
        }
    };

    void Start()
    {
        qrButton.onClick.AddListener(delegate{qrCode();});
        connectionButton.onClick.AddListener(delegate{localConnect();});


        c32 = new Color32[256 * 256];
        rt = new RenderTexture(256, 256, 0);
        t2d = new Texture2D(256,256, TextureFormat.RGBA32, false);
    }


    void Update()
    {
        if(ct==null || !ct.isPlaying) return;
        if (ct.isPlaying)
        {
            Graphics.Blit(ct, rt, new Vector2(1, 1), new Vector2(0, 0));
            output.texture = rt;

            RenderTexture.active = rt;
            t2d.ReadPixels(new Rect(0,0,256,256),0,0);
            t2d.Apply();
            RenderTexture.active = null;

            c32 = t2d.GetPixels32();
            
            Result results = barcodeReader.Decode(c32, 256, 256);
            if (results != null)
            {
                ipText.text = results.Text;
                stopqrReader();
                localConnect();
            }
        }
    }

    public void localConnect(){
        if (ipText.text.Length < 1)
        {
            popGui.resetGui();
            return;
        }
        popGui.reqServer(ipText.text);
    }

    public void qrCode()
    {
        if (ct != null && ct.isPlaying){stopqrReader(); return;}
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            alertBox.alert("This function requires Camera Permission. Please allow Camera access and try again or restart and try again!...");
            Permission.RequestUserPermission(Permission.Camera);
            return;
        }
        if (WebCamTexture.devices.Length == 0)
        {
            alertBox.alert("No Cameras Found..");
        }
        else
        {
            foreach (var device in WebCamTexture.devices)
            {
                if (!device.isFrontFacing)
                {
                    ct = new WebCamTexture(device.name,256,256);
                }
            }
            if(!ct){ct = new WebCamTexture(256,256); print(ct);};
            ct.Play();
        }
    }


    public void stopqrReader()
    {
        if(!ct){return;}
        ct.Stop();
        ct = null;
        output.texture = null;
    }

    public void cancelConnection()
    {
        popGui.cancelConnection();
    }

}
