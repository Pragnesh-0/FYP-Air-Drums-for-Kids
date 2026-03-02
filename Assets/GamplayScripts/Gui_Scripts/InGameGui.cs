using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class InGameGui : MonoBehaviour
{

    public Button backButton;
    public PlaneKit gameplayObject;
    public GuiMenuSelector menuSelector;

    public Button recordButton;

    public TextMeshProUGUI musicName;
    public Button musicLib;
    public GameObject library;


    public Button replayButton;
    public Button ppButtonMusic;
    public Image buttonSprite;
    public Sprite playSprite;
    public Sprite pauseSprite;

    public RectTransform progressBar;

    public AudioSource musicSource;


    bool isCapturing = false;
    Color recording = Color.red;
    Color playback = Color.green;
    void Start()
    {
        musicLib.onClick.AddListener(delegate {showLibrary();});
        recordButton.onClick.AddListener(async delegate{ await captureData(); });
        replayButton.onClick.AddListener(delegate{ musicReplay(); });
        ppButtonMusic.onClick.AddListener(delegate{ musicPlayer(); });
        backButton.onClick.AddListener(delegate{gameplayObject.resetValues(); isCapturing = false; menuSelector.selectGui("MainMenu");});
    }


    void Update()
    {
        if(musicSource.isPlaying)
        {
            float scaleVal = musicSource.time/musicSource.clip.length;
            progressBar.localScale = new Vector2(scaleVal,1);
            if (scaleVal > 0.95f)
            {
                buttonSprite.sprite = playSprite;
                progressBar.localScale = new Vector2(0,1);
            }
        }
    }

    public void setMusic(string mName)
    {
        mName = Path.GetFileNameWithoutExtension(mName);
        musicReplay();
        musicSource.clip = null;
        musicName.SetText(mName);
    }



    void showLibrary()
    {
        if (!library.activeSelf)
        {
            library.GetComponent<LibraryPopGui>().loadFromFiles(delegate(string mName){ setMusic(mName); });
        }
        library.SetActive(!library.activeSelf);
    }

    async Awaitable captureData()
    {
        if(!Camera.main.GetComponent<AudioCapture>()){ isCapturing = false; return; }
        if (isCapturing) 
        {
            recordButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Record");
            isCapturing = false;
            return;
        }
        recordButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Stop");
        isCapturing = true;
        if(musicSource.isPlaying){cleanPlayer();}
        Camera.main.GetComponent<AudioCapture>().startCapture();
        ppButtonMusic.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
        progressBar.localScale = new Vector2(0,1);
        progressBar.GetComponent<Image>().color = recording;
        float timeElapsed = 0;
        while (timeElapsed < 120f)
        {
            timeElapsed += Time.deltaTime;
            if (isCapturing == false)
            {
                break;
            }
            musicName.SetText(Mathf.RoundToInt(timeElapsed)+"s"+" / 120s");
            progressBar.localScale = new Vector2(timeElapsed/120f,1);
            await Awaitable.NextFrameAsync();
        }
        isCapturing = false;
        List<float> data = Camera.main.GetComponent<AudioCapture>().getCapture();
        recordButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Record");
        if(data.Count < 1) return;
        AudioClip ac = AudioClip.Create("clip", data.Count/2, 2, AudioSettings.outputSampleRate, false);
        ac.SetData(data.ToArray(),0);
        musicSource.clip = ac;
        ppButtonMusic.gameObject.SetActive(true);
        replayButton.gameObject.SetActive(true);
        progressBar.localScale = new Vector2(0,1);
        progressBar.GetComponent<Image>().color = playback;
        musicName.SetText("Recording Playback");
        return;
    }

    void musicReplay()
    {
        if (musicSource.clip == null) return;
        musicSource.Stop();
        buttonSprite.sprite = playSprite;
        progressBar.localScale = new Vector2(0,1);
    }
    void musicPlayer()
    {
        if (musicSource.clip == null) return;
        if (musicSource.isPlaying)
        {
            buttonSprite.sprite = playSprite;
            musicSource.Pause();

        }
        else
        {
            buttonSprite.sprite = pauseSprite;
            if (musicSource.time > 0)
            {
                musicSource.UnPause();
            }
            else
            {
                progressBar.localScale = new Vector2(0,1);
                musicSource.Play();
            }
        }
    }

    void cleanPlayer()
    {
        musicName.SetText("");
        library.SetActive(false);
        musicReplay();
        musicSource.clip = null;
    }


    void OnDisable()
    {
        cleanPlayer();
    }
}
