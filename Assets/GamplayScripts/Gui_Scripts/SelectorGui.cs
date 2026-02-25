using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class SelectorGui : MonoBehaviour
{
    public Button playButton;
    public AlertBox ab;

    public TextMeshProUGUI score;
    public TextMeshProUGUI musicName;

    public GameObject libraryPop;
    public GuiMenuSelector gms;


    public MinigameLogic mgl;


    string currentSelectedSong;

    void Start()
    {
        playButton.onClick.AddListener(delegate{pressedPlay();});
    }


    public bool selectSong(string n)
    {
        string file_name = Path.GetFileNameWithoutExtension(n);
        BeatmapModels bmp = BeatmapData.getBeatMap(file_name);
        if (bmp == null)
        {
            ab.alert("Beatmap Data doesn't exist for this audio.");
            return false;
        }
        score.SetText(bmp.score.ToString());
        musicName.SetText(file_name);
        mgl.initalize(bmp, file_name , delegate{selectSong(n);});
        return true;
    }


    void pressedPlay()
    {
        libraryPop.SetActive(false);
        mgl.playMinigame();
        gms.selectGui("MinigamePlayer");
    }

}
