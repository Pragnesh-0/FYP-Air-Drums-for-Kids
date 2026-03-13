using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class KitClass
{
    public List<DrumClass> drums;
    public string name;

    public bool isFavorite;
}

[System.Serializable]
public class DrumClass
{
    public string type;
    public string name;
    public float volume;
    public float size;
    public float panStereo;
    public Vector3 position;

}

