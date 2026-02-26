using UnityEngine;
using System.Collections.Generic;
using System;
public class MinigameLogic : MonoBehaviour
{
    public bool isActive = false;
    bool hasReset = false;

    Action resetBeats;

    public string songName;
    public int currentScore = 0;
    public List<BeatData> beats = new List<BeatData>();
    public Dictionary<BeatData,List<GameObject>> beatsDict = new Dictionary<BeatData, List<GameObject>>();
    public GameObject beatItem;


    public MinigamePlayerGui playerGui;
    public PlaneKit drumPlayer;
    public AudioSource musicPlayer;

    void Start()
    {
        playerGui.setCloseCallback(delegate{resetVals();});
        drumPlayer.setOnHitCallback(delegate(string value) {drumHit(value);});
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
                List<GameObject> gms = new List<GameObject>();
                List<Vector2> positions = drumPlayer.getPositions(beat.type);
                Dictionary<GameObject, Vector2> posStuff = new Dictionary<GameObject, Vector2>();
                foreach (Vector2 pos in positions)
                {
                    GameObject b = Instantiate(beatItem);
                    gms.Add(b);
                    posStuff[b] = pos;
                }
                playerGui.addBeatObjects(posStuff, beat.time+0.1f - getAudioSourceTime());
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
            playerGui.finishedEffect();
            print(currentScore);
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
        drumPlayer.resetValues();
        beats = new List<BeatData>();
        beatsDict = new Dictionary<BeatData, List<GameObject>>();
        currentScore = 0;
        if(resetBeats != null)
        {
            resetBeats();
        }
        hasReset = true;
    }

    public void initalize(BeatmapModels bmp, string name , Action resetBeat)
    {
        beats = bmp.beats;
        songName = name;
        resetBeats = resetBeat;
    }

    public async void playMinigame()
    {
        hasReset = false;
        drumPlayer.setValues(true);
        await playerGui.readyUI();
        if(hasReset){ hasReset = false; return;}
        musicPlayer.Play();
        isActive = true;
    }

    public void drumHit(string dType)
    {
        List<BeatData> keyList = new List<BeatData>(beatsDict.Keys);
        foreach (BeatData key in keyList)
        {
            if (key.time+0.1 < getAudioSourceTime())
            {
                beatsDict.Remove(key);
                continue;
            }
            if (key.type == dType)
            {
                if (getAudioSourceTime() < key.time+0.1)
                {
                    float timeDiff = key.time+0.1f - getAudioSourceTime();
                    int score;
                    List<GameObject> beats = beatsDict[key];
                    switch (timeDiff)
                    {
                        case <= 0.3f:
                            score = 100;
                            break;
                        case > 0.3f:
                            score = 75;
                            break;
                        default: 
                            score = 25;
                            break;
                    }
                    foreach(GameObject beatObj in beats)
                    {
                        if(beatObj){ beatObj.GetComponent<CircleClosing>().scoreNotif(score.ToString()); }
                    }
                    currentScore += score;
                    playerGui.addScore(currentScore.ToString());
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
