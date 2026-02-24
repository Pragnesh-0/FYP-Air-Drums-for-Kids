using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class MinigamePlayerGui : MonoBehaviour
{

    
    public Action backCallback;
    public HoldButton quitButton;
    public GuiMenuSelector gms;
    public GameObject libraryPop;


    public TextMeshProUGUI scoreLabel;
    public Button close;

    void Start()
    {
        quitButton.addHoldListener(delegate{closeGui();},"");
        //close.onClick.AddListener(delegate{closeGui();});
    }


    void closeGui()
    {
        gms.selectGui("Minigame");
        libraryPop.SetActive(true);
        backCallback();
    }

    public void finished()
    {
        print("im finished!");
    }
    

    public void cleanup()
    {
        
    }

    public async Awaitable addScore(string score)
    {
        
    }

    public async Awaitable countDown()
    {
        float timeElapsed = 0;
        while (timeElapsed < 3.1f)
        {
            timeElapsed += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }
    }

    public void setCloseCallback(Action callback)
    {
        backCallback = callback;
    }
}
