using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialGuiScript : MonoBehaviour
{
    public List<Button> panelButtons;   
    public List<GameObject> panels;
    public Dictionary<string, GameObject> panelsDict = new Dictionary<string, GameObject>();



    public Button backButton;
    public GuiMenuSelector gms;


    public Button cycleLeft;
    public Button cycleRight;

    public GameObject scrim;


    
    public GameObject defaultPanel;
    public GameObject curPanel;


    int valCount = 0;

    void Start()
    {
        int index = -1;
        foreach (GameObject go in panels)
        {
            index += 1;
            panelsDict.Add(go.name, go);
            panelButtons[index].onClick.AddListener(delegate{switchPanel(go.name);});
        }

        backButton.onClick.AddListener(delegate{
            gms.selectGui("MainMenu");
            switchPanel(defaultPanel.name);
        });

        cycleLeft.onClick.AddListener(delegate{valCount-=1; cycleTutPanel(false);});
        cycleRight.onClick.AddListener(delegate{valCount+=1; cycleTutPanel(false);});
    }


    public async void switchPanel(string panelName)
    {
        if(curPanel.name == panelName){ return; }
        valCount = 0;
        scrim.SetActive(true);
        await transition(false,curPanel);
        foreach(GameObject go in panels)
        {
            go.SetActive(false);
            go.GetComponent<RectTransform>().localScale = Vector3.zero;
        }
        panelsDict[panelName].SetActive(true);
        curPanel = panelsDict[panelName];
        cycleTutPanel(true);
        await transition(true,panelsDict[panelName]);
        scrim.SetActive(false);
    }

    public async void cycleTutPanel(bool reset)
    {
        int objIndexCount = curPanel.transform.childCount-1;
        if(valCount < 0){valCount = 0; return;}
        if(valCount > objIndexCount){valCount = objIndexCount; return;}
        scrim.SetActive(true);
        if(!reset) await transition(false, curPanel);
        foreach (Transform t in curPanel.transform)
        {
            t.gameObject.SetActive(false);
        }
        curPanel.transform.GetChild(valCount).gameObject.SetActive(true);
        if(reset){return;}
        await transition(true, curPanel);
        scrim.SetActive(false);
    }

    async Awaitable transition(bool reverse, GameObject panelObj)
    {
        float elapsedTime = 0;
        while (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime;
            panelObj.GetComponent<RectTransform>().localScale = reverse? Vector3.zero + Vector3.one*(elapsedTime/0.1f): Vector3.one - Vector3.one*(elapsedTime/0.1f);
            await Awaitable.NextFrameAsync();
        }
        panelObj.GetComponent<RectTransform>().localScale = reverse? Vector3.one: Vector3.zero;
    }

}
