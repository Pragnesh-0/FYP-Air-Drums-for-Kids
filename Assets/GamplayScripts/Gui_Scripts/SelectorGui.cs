using UnityEngine;
using System.IO;

public class SelectorGui : MonoBehaviour
{
    


    public void selectSong(string n)
    {
        string file_name = Path.GetFileNameWithoutExtension(n);
        //get beatmap data, load beatmap, load minigame ingame gui, play minigame...
    }

}
