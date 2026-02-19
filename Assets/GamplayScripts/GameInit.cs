using UnityEngine;
using UnityEngine.UI;
public class GameInit : MonoBehaviour
{

    public Button initButton;
    public GameObject panel;
    public KitData kitStuff;
    public GuiMenuSelector guiSelector;
    public AudioSource introAudio;



    public RectTransform initLogo;
    public RectTransform menuLogo;


    public VolumeSettings volumeSettings;
    public StickSettings calibrationSettings;

    void Start()
    {
        Settings settings = SettingsData.getSettings();
        kitStuff.loadKits();
        kitStuff.loadKit(settings.equippedKit);
        volumeSettings.setSettings(settings.gameAudioData);
        calibrationSettings.setSettings(settings.calibrationSettings);
        initButton.onClick.AddListener(initialise);
    }

    
    public async void initialise()
    {
        initButton.gameObject.SetActive(false);
        guiSelector.selectGui("MainMenu");
        menuLogo.gameObject.SetActive(false);
        await initTransition();
        Color c = panel.GetComponent<Image>().color;
        c.a = 0;
        panel.GetComponent<Image>().color = c;
        await lastTransition();
        panel.SetActive(false);
    }


    public async Awaitable initTransition()
    {
        introAudio.Play();
        float ypos = initLogo.localPosition.y;
        initLogo.pivot = Vector2.one * .5f;
        float timeElapsed = 0;
        while (timeElapsed < .25f)
        {
            timeElapsed += Time.deltaTime;
            initLogo.localPosition = new Vector2(0, ypos - (ypos * timeElapsed/.25f));
            initLogo.localScale = Vector2.one + (Vector2.one * (1f * (timeElapsed/.25f)));
            await Awaitable.NextFrameAsync();
        }
        initLogo.localScale = Vector2.one * 2f;
        await Awaitable.WaitForSecondsAsync(.4f);
    }

    public async Awaitable lastTransition()
    {
        Vector2 diff = menuLogo.localPosition - initLogo.localPosition;
        Vector2 startPos = initLogo.localPosition;
        float timeElapsed = 0;
        while (timeElapsed < .1f)
        {
            timeElapsed += Time.deltaTime;
            initLogo.localPosition = startPos + (diff * (timeElapsed/.1f));   
            initLogo.localScale = (Vector2.one*2f) - (Vector2.one * (1f * (timeElapsed/.1f)));
            await Awaitable.NextFrameAsync();
        }
        initLogo.localScale = Vector2.one * 1f;
        initLogo.localPosition = menuLogo.localPosition;
        menuLogo.gameObject.SetActive(true);
    }

    
}
