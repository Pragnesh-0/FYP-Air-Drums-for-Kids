using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.UI;

public class PlaneKit : MonoBehaviour
{
    public GameObject equippedKit;

    public ComputerVisionDetection cvd;


    public List<GameObject> drumObjects;
    public List<Vector4> planeObjects;
    public bool isGamemode;

    public GridMapEffect gridMapEfx;
    public void setValues(bool isgm)
    {
        foreach(Transform obj in equippedKit.GetComponent<Transform>())
        {
            drumObjects.Add(obj.gameObject);
        }
        planeObjects = DrumKit2dData.planeData(equippedKit);
        cvd.cameraAction(true);
        isGamemode = isgm;
    }

    public void resetValues()
    {
        drumObjects = new List<GameObject>();
        planeObjects = new List<Vector4>();
        cvd.cameraAction(false);
        isGamemode = false;
    }


    public void playSound(Vector2 pos)
    {
        var index = -1;
        foreach(Vector4 v in planeObjects)
        {
            index += 1;
            if(v.x < pos.x && v.z > pos.x && v.y < pos.y && v.w > pos.y)
            {
                drumObjects[index].GetComponent<AudioSource>().PlayOneShot(drumObjects[index].GetComponent<AudioSource>().clip);
                gridMapEfx.spikeBloom();
                if (isGamemode)
                {
                    print("Send to thing!");
                }
            }
        }
    }
}
