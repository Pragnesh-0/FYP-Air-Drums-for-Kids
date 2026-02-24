using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class MinigameLogic : MonoBehaviour
{

    public bool isActive = false;

    public string songName;

    public int currentScore = 0;
    public List<BeatData> beats = new List<BeatData>();

    public Dictionary<BeatData,List<GameObject>> beatsDict = new Dictionary<BeatData, List<GameObject>>();

    public TextMeshProUGUI scoreLabel;


    public AudioSource musicPlayer;

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        int index = 0;

        foreach (var beat in beats)
        {
            index += 1;
            if (getAudioSourceTime() >= beat.time - 0.8f)
            {
                List<GameObject> gms = new List<GameObject>();
                //spawn item
                beatsDict.Add(beat, gms);
                break;
            }
        }

        beats.RemoveAt(index);

        if (!musicPlayer.isPlaying)
        {
            isActive = false;
            saveScore();
            //transition
        }
    }

    void saveScore()
    {
        BeatmapData.saveScore(songName, currentScore);
    }

    public void resetVals()
    {
        musicPlayer.Stop();
        isActive = false;
        beats = new List<BeatData>();
        beatsDict = new Dictionary<BeatData, List<GameObject>>();
        currentScore = 0;
    }

    public void initalize(BeatmapModels bmp, string name)
    {
        beats = bmp.beats;
        songName = name;
        //scoreLabel.SetText("");
    }

    public void drumHit(string dType)
    {
        List<BeatData> keyList = new List<BeatData>(beatsDict.Keys);
        foreach (BeatData key in keyList)
        {
            if (key.time+0.05 < getAudioSourceTime())
            {
                beatsDict.Remove(key);
            }
            if (key.type == dType)
            {
                if (getAudioSourceTime() < key.time+0.05)
                {
                    currentScore += 100 - (int)((key.time+0.05 - getAudioSourceTime())* 125);
                    //effects
                    //destroy objects
                    beatsDict.Remove(key);
                }
            }
        }
    }

    float getAudioSourceTime()
    {
        return (float)musicPlayer.timeSamples / musicPlayer.clip.frequency;
    }
}
