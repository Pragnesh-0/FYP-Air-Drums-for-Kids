using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
public class MinigamePlayerGui : MonoBehaviour
{

    
    public Action backCallback;
    public HoldButton quitButton;
    public GuiMenuSelector gms;
    public GameObject libraryPop;


    public TextMeshProUGUI scoreLabel;
    public TextMeshProUGUI readyLabel;
    public Button close;


    public GameObject beatPanel;


    int count;

    void Start()
    {
        quitButton.addHoldListener(delegate{closeGui();},"");
        close.onClick.AddListener(delegate{closeGui();});
    }


    void closeGui()
    {
        gms.selectGui("Minigame");
        backCallback();
        cleanup();
        libraryPop.SetActive(true);
    }

    public async void finishedEffect()
    {
        await finishEffect();
        close.gameObject.SetActive(true);
    }

    public async Awaitable finishEffect()
    {
        RectTransform st = scoreLabel.gameObject.gameObject.GetComponent<RectTransform>();
        float timeElapsed2 = 0;
        while (timeElapsed2 < .8f)
        {
            timeElapsed2 += Time.deltaTime;
            st.localScale = (Vector2.one) + (Vector2.one * (timeElapsed2/.8f));
            await Awaitable.NextFrameAsync();
        }
    }
    

    void cleanup()
    {
        scoreLabel.SetText("");
        readyLabel.SetText("");
        RectTransform sLT = scoreLabel.GetComponent<RectTransform>();
        RectTransform rLT = readyLabel.GetComponent<RectTransform>();
        sLT.localScale = Vector2.one;
        rLT.localScale = Vector2.one;
        scoreLabel.gameObject.SetActive(false);
        readyLabel.gameObject.SetActive(false);
        close.gameObject.SetActive(false);
    }

    public async void addScore(string score)
    {
        scoreLabel.SetText(score);
        count += count;
        await addScoreEffect(count);
    }

    async Awaitable addScoreEffect(int curCount)
    {
        float timeElapsed = 0;
        RectTransform srt = scoreLabel.GetComponent<RectTransform>();
        srt.localScale = Vector2.one;
        while(timeElapsed < 0.1f)
        {
            timeElapsed += Time.deltaTime;
            if(count != curCount)
            {
                return;
            }
            srt.localScale = Vector2.one + (Vector2.one * 0.5f * (timeElapsed/.1f));
            await Awaitable.NextFrameAsync();
        }
        float timeElapsed2 = 0;
        while(timeElapsed2 < 0.1f)
        {
            timeElapsed2 += Time.deltaTime;
            if(count != curCount)
            {
                return;
            }
            srt.localScale = (Vector2.one*1.5f) - (Vector2.one * 0.5f * (timeElapsed/.1f));
            await Awaitable.NextFrameAsync();
        }
        srt.localScale = Vector2.one;
    }

    public void addBeatObjects(Dictionary<GameObject, Vector2> bs, float timer)
    {

        foreach(GameObject b in new List<GameObject>(bs.Keys))
        {
            b.transform.SetParent(beatPanel.transform, false);
            b.GetComponent<RectTransform>().position = bs[b];
            b.SetActive(true);
            b.GetComponent<CircleClosing>().activateObj(timer);
        }
    }

    public async Awaitable readyUI()
    {
        scoreLabel.SetText("0");
        readyLabel.SetText("Get Ready...");
        readyLabel.gameObject.SetActive(true);
        RectTransform rt = readyLabel.gameObject.gameObject.GetComponent<RectTransform>();
        Vector2 sp = rt.localPosition;
        float timeElapsed2 = 0;
        while (timeElapsed2 < 1.5f)
        {
            timeElapsed2 += Time.deltaTime;
            rt.localScale = (Vector2.one*1.75f) - (Vector2.one * 1.65f * (timeElapsed2/1.5f));
            await Awaitable.NextFrameAsync();
        }
        readyLabel.gameObject.SetActive(false);
        scoreLabel.gameObject.SetActive(true);
    }

    public void setCloseCallback(Action callback)
    {
        backCallback = callback;
    }
}
