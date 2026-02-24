using UnityEngine;
using System.Collections.Generic;
using System.IO;
public static class BeatmapData
{
    public static BeatmapModels getBeatMap(string fname)
    {
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "BeatmapData"));
        string dirName = Path.Combine(Application.persistentDataPath, "BeatmapData");
        string filePath = Path.Combine(dirName, fname+".json");
        BeatmapModels bmp;
        if (!File.Exists(filePath))
        {
            return null;
        }
        if(new FileInfo(filePath).Length == 0) return null;
        string data = File.ReadAllText(filePath);
        bmp = JsonUtility.FromJson<BeatmapModels>(data);
        return bmp;
    }


    public static void saveScore(string fname, int score)
    {
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "BeatmapData"));
        string dirName = Path.Combine(Application.persistentDataPath, "BeatmapData");
        string filePath = Path.Combine(dirName, fname+".json");
        BeatmapModels bmp;
        if (!File.Exists(filePath))
        {
            return;
        }
        string data = File.ReadAllText(filePath);
        bmp = JsonUtility.FromJson<BeatmapModels>(data);
        if (bmp.score > score)
        {
            return;
        }
        bmp.score = score;

        File.WriteAllText(filePath, JsonUtility.ToJson(bmp));
    }
}
