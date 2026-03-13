using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class BeatmapModels
{
    public int score;
    public bool fav;
    public List<BeatData> beats;
}

[System.Serializable]
public class BeatData
{
    public float time;
    public string type;
}
