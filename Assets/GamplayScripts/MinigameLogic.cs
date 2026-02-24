using UnityEngine;
using System.Collections.Generic;
using System;
public class MinigameLogic : MonoBehaviour
{
    public bool isActive = false;

    Action resetBeats;

    public string songName;
    public int currentScore = 0;
    public List<BeatData> beats = new List<BeatData>();
    public Dictionary<BeatData,List<GameObject>> beatsDict = new Dictionary<BeatData, List<GameObject>>();



    public MinigamePlayerGui playerGui;
    public PlaneKit drumPlayer;
    public AudioSource musicPlayer;

    void Start()
    {
        playerGui.setCloseCallback(delegate{resetVals();});
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        int index = -1;
        int indexToRemove = -1;

        foreach (var beat in beats)
        {
            index += 1;
            if (getAudioSourceTime() >= beat.time - 0.8f)
            {
                indexToRemove = index;
                print(getAudioSourceTime());
                List<GameObject> gms = new List<GameObject>();
                //spawn item
                beatsDict.Add(beat, gms);
                break;
            }
        }

        if (indexToRemove != -1)
        {
            beats.RemoveAt(indexToRemove);
        }
            

        if (!musicPlayer.isPlaying && beats.Count == 0)
        {
            isActive = false;
            saveScore();
            playerGui.finished();
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
        if(resetBeats != null)
        {
            resetBeats();
            resetBeats = null;
        }
    }

    public void initalize(BeatmapModels bmp, string name , Action resetBeat)
    {
        beats = bmp.beats;
        songName = name;
        //scoreLabel.SetText("");

        resetBeats = resetBeat;
    }

    public async void playMinigame()
    {
        await playerGui.countDown();
        //drumPlayer.setValues(true);
        musicPlayer.Play();
        isActive = true;
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
