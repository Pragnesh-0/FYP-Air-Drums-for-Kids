using UnityEngine;
using TMPro;
using System;

public class PlaytimeTimer : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public float timeTracked = 0;
    void Update()
    {
        timeTracked += Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeTracked);
        timer.SetText("Playtime: "+timeSpan.ToString(@"hh\:mm\:ss"));
    }
}
