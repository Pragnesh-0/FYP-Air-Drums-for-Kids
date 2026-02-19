using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class AddDrumHandler : MonoBehaviour
{
    public List<GameObject> drumSpawnwers;
    public List<GameObject> drumList;
    public Editing editObj;
    void Start()
    {
        for (int i = 0; i < drumSpawnwers.Count; i++)
        {
            GameObject drumType = drumList[i];
            drumSpawnwers[i].GetComponentInChildren<Button>().onClick.AddListener(delegate{editObj.addDrum(drumType);});
        }
    }
}
