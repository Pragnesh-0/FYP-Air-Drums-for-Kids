using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public static class MusicData
{

    public static List<string> getAllMusic()
    {
        List<string> musicList = new List<string>();
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "MusicData"));
        string[] paths = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "MusicData"));
        
        foreach (string path in paths)
        {
            if(Path.GetExtension(path).ToLower() != ".mp3") continue;
            string fName = Path.GetFileName(path);
            musicList.Add(fName);
        }
        return musicList;
    }

    public static async Awaitable<AudioClip> getMusicData(string fname)
    {
        try
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "MusicData"));
            string dirName = Path.Combine(Application.persistentDataPath,"MusicData");
            string path = Path.Combine(dirName, fname);
            if (!File.Exists(path))
            {
                return null;
            }
            AudioClip ac;
            using UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG);
            await req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                return null;
            }
            ac = DownloadHandlerAudioClip.GetContent(req);
            return ac;
        }
        catch
        {
            return null;
        }
    }


    public static void deleteMusic(string fname)
    {
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "MusicData"));
        string dirName = Path.Combine(Application.persistentDataPath,"MusicData");
        string path = Path.Combine(dirName, fname+".mp3");
        if (!File.Exists(path)) return;
        File.Delete(path);
    }
    
}
