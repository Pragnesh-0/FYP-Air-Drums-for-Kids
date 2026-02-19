using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SettingsMenu : MonoBehaviour
{
    public List<GameObject> panels;
    public List<Button> panelButtons;
    public GuiMenuSelector GuiMenuSelector;
    Dictionary<string, GameObject> panelsDict = new Dictionary<string,GameObject>();

    public Button backButton;
    public GameObject scrim;
    public GameObject curPanel;
    public GameObject defaultPanel;

    public StickSettings stickSettings;
    public VolumeSettings volumeSettings;

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
            Settings gameSettings = SettingsData.getSettings();
            gameSettings.calibrationSettings = stickSettings.getSettings();
            gameSettings.gameAudioData = volumeSettings.getSettings();
            SettingsData.saveSettings(gameSettings);
            stickSettings.showCase(false);
            GuiMenuSelector.selectGui("MainMenu");
            switchPanel(defaultPanel.name);
        });
    }


    public async void switchPanel(string panelName)
    {
        if(curPanel.name == panelName){ return; }
        stickSettings.showCase(false);
        scrim.SetActive(true);
        await transition(false,curPanel);
        foreach(GameObject go in panels)
        {
            go.SetActive(false);
            go.GetComponent<RectTransform>().localScale = Vector3.zero;
        }
        panelsDict[panelName].SetActive(true);
        curPanel = panelsDict[panelName];
        await transition(true,panelsDict[panelName]);
        scrim.SetActive(false);
    }

    public async Awaitable transition(bool reverse, GameObject panel)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < .1f)
        {
            elapsedTime += Time.deltaTime;
            panel.GetComponent<RectTransform>().localScale = reverse? Vector3.zero + Vector3.one*(elapsedTime/0.1f): Vector3.one - Vector3.one*(elapsedTime/0.1f);
            await Awaitable.NextFrameAsync();
        }
        panel.GetComponent<RectTransform>().localScale = reverse? Vector3.one: Vector3.zero;
    }


}
