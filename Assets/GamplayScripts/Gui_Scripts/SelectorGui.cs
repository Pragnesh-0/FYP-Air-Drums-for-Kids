using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading.Tasks;

public class SelectorGui : MonoBehaviour
{
    public HoldButton deleteButton;
    public Button playButton;
    public Button fav;
    public AlertBox ab;

    public TextMeshProUGUI score;
    public TextMeshProUGUI musicName;

    public GameObject libraryPop;
    public GuiMenuSelector gms;


    public MinigameLogic mgl;


    string currentSelectedSong;
    Action<bool> reloadCallback;

    void Start()
    {
        playButton.onClick.AddListener(delegate{pressedPlay();});
        deleteButton.addHoldListener(delegate{MusicData.deleteMusic(currentSelectedSong); reloadCallback(true);  currentSelectedSong = null; reloadCallback = null;}, "Delete");
        fav.onClick.AddListener(delegate{ BeatmapData.setFav(currentSelectedSong); reloadCallback(false); selectSong(currentSelectedSong , reloadCallback);});
    }


    public bool selectSong(string n, Action<bool> _reloadCallback)
    {
        string file_name = Path.GetFileNameWithoutExtension(n);
        currentSelectedSong = file_name;
        BeatmapModels bmp = BeatmapData.getBeatMap(file_name);
        if (bmp == null)
        {
            ab.alert("Beatmap Data doesn't exist for this audio.");
            return false;
        }
        score.SetText(bmp.score.ToString());
        musicName.SetText(file_name);
        reloadCallback = _reloadCallback;
        mgl.initalize(bmp, file_name , delegate{selectSong(n ,reloadCallback);});
        return true;
    }


    void pressedPlay()
    {
        libraryPop.SetActive(false);
        mgl.playMinigame();
        gms.selectGui("MinigamePlayer");
    }

}
