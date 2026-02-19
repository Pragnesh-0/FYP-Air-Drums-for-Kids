using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.IO.Compression;



public static class NetworkingStuff
{

    private static readonly HttpClient client = new HttpClient();

    public static async Awaitable<string> GetLibrary(string ipAddress)
    {
        try
        {   
            var responseString = await client.GetStringAsync("http://"+ipAddress+"/get_library");
            if (responseString == null) return "";
            return responseString;
        }
        catch(HttpRequestException)
        {
            return null;
        }
    }

    public static async Awaitable<bool> GetData(string ipAddress, string name)
    {
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "MusicData"));
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "BeatmapData"));
        try
        {   
            var response = await client.GetAsync("http://"+ipAddress+"/get_music/"+name);
            if (response == null) return false;
            Stream stream = await response.Content.ReadAsStreamAsync();
            ZipArchive archive = new(stream, ZipArchiveMode.Read, true);
            if (archive == null){ stream.Close(); return false; }
            if (archive.Entries.Count != 2)
            {
                archive.Dispose();
                stream.Close();
                return false;
            }
            ZipArchiveEntry musicEntry = null;
            ZipArchiveEntry beatmapEntry = null;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.FullName.EndsWith(".mp3")) musicEntry = entry;
                if (entry.FullName.EndsWith(".json")) beatmapEntry = entry;
            }

            if (musicEntry != null && beatmapEntry != null)
            {
                musicEntry.ExtractToFile(Path.Combine(Application.persistentDataPath,"MusicData/"+musicEntry.FullName),true);
                beatmapEntry.ExtractToFile(Path.Combine(Application.persistentDataPath,"BeatmapData/"+beatmapEntry.FullName), true);
            }
            stream.Close();
            archive.Dispose();
        }
        catch(HttpRequestException e) {
            Debug.Log(e);
            return false;
        }
        return true;
    }

}