using UnityEngine;
using System.Collections.Generic;

public class AudioCapture : MonoBehaviour
{

    bool isCapturing = false;
    List<float> capturedData = new List<float>();
    

    void OnAudioFilterRead(float[] data, int channel)
    {
        if(!isCapturing){return;}

        for (int i = 0; i < data.Length; i++)
        {
            capturedData.Add(data[i]);
        }
    }

    public List<float> getCapture()
    {
        isCapturing = false;
        return capturedData;
    }


    public void startCapture()
    {
        capturedData = new List<float>();
        isCapturing = true;
    }
}
