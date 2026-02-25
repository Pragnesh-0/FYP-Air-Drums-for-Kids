using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.UI;
using System;

public class PlaneKit : MonoBehaviour
{
    public GameObject equippedKit;

    public ComputerVisionDetection cvd;


    public Action<string> minigameCallBack;


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

    public void setOnHitCallback(Action<string> setOnHitCallback)
    {
        minigameCallBack = setOnHitCallback;
    }


    public List<Vector2> getPositions(string drumType)
    {
        int index = -1;
        List<Vector2> myList = new List<Vector2>();
        foreach (GameObject obj in drumObjects)
        {
            index += 1;
            if (obj.GetComponent<DrumGameObj>().drumType == drumType)
            {
                Vector4 norm = planeObjects[index];
                myList.Add(new Vector2((norm.z + norm.x)/2f,(norm.w + norm.y)/2f));
            }
        }
        return myList;
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
                    minigameCallBack(drumObjects[index].GetComponent<DrumGameObj>().drumType);
                }
            }
        }
    }
}
