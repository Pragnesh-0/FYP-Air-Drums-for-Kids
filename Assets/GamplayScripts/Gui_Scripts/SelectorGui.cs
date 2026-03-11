using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System;

public class SelectorGui : MonoBehaviour
{
    public HoldButton deleteButton;
    public Button playButton;
    public AlertBox ab;

    public TextMeshProUGUI score;
    public TextMeshProUGUI musicName;

    public GameObject libraryPop;
    public GuiMenuSelector gms;


    public MinigameLogic mgl;


    string currentSelectedSong;
    Action deleteCallback;

    void Start()
    {
        playButton.onClick.AddListener(delegate{pressedPlay();});
        deleteButton.addHoldListener(delegate{deleteCallback(); MusicData.deleteMusic(currentSelectedSong); currentSelectedSong = null; deleteCallback = null;}, "Delete");
    }


    public bool selectSong(string n, Action callback)
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
        deleteCallback = callback;
        mgl.initalize(bmp, file_name , delegate{selectSong(n ,callback);});
        return true;
    }


    void pressedPlay()
    {
        libraryPop.SetActive(false);
        mgl.playMinigame();
        gms.selectGui("MinigamePlayer");
    }

}
