using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System;

public class LibraryPopGui : MonoBehaviour
{
    public GameObject downloadable;
    public GameObject selector;

    bool connecting = false;
    bool downloading = false;
    int connectionId = 0;


    public AudioSource musicSource;
    public AlertBox alertB;
    public GameObject downloadNotif;


    public AudioSource downloadDone;

    public GameObject content;


    private List<GameObject> currentList = new List<GameObject>();


    private async Awaitable addSelectionAction(string n, Action<string> setMusicCallback)
    {
        AudioClip ac = await MusicData.getMusicData(n);
        if (ac == null)
        {
            alertB.alert("An error occured loading the audio clip!");
            setMusicCallback("");
            return;
        }
        setMusicCallback(n);
        musicSource.clip = ac;
    }


    private async Awaitable addDownloadAction(string n, string ipAddress)
    {
        if(downloading || connecting)
        {
            alertB.alert("A request is already being processed!");
            return;
        } 
        downloading = true;
        downloadNotif.SetActive(true);
        bool val = await NetworkingStuff.GetData(ipAddress,n);
        downloading = false;
        downloadDone.Play();
        if (!val)
        {
            alertB.alert("Something went wrong..");
        }
        downloadNotif.SetActive(false);
        reqServer(ipAddress);
    }

    private void populateUI(List<string> l, bool isDownloadable, string ipAddress, Action<string> setMusicCallback)
    {
        resetGui();
        GameObject t = isDownloadable ? downloadable : selector;
        int index = 0;
        float x1 = content.GetComponent<RectTransform>().sizeDelta.x;
        float y1 = l.Count * 100;
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(x1, y1);
        foreach (string n in l)
        {
            GameObject copy = Instantiate(t, content.transform);
            copy.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (y1/2) - 50 - (index * 100));
            string n2 = Path.GetFileNameWithoutExtension(n);
            copy.name = n; copy.GetComponentInChildren<TextMeshProUGUI>().SetText(n2);
            currentList.Add(copy);
            copy.SetActive(true);
            if (!isDownloadable) copy.GetComponentInChildren<Button>().onClick.AddListener(async delegate{await addSelectionAction(n, setMusicCallback);});
            if (isDownloadable) copy.GetComponentInChildren<Button>().onClick.AddListener(async delegate{await addDownloadAction(n, ipAddress);});
            index+=1;
        }
    }

    

    public void loadFromFiles(Action<string> setMusicCallback)
    {
        List<string> data = MusicData.getAllMusic();
        populateUI(data, false, "", setMusicCallback);
    }

    public void resetGui()
    {
        if(currentList.Count>0){foreach(GameObject gm in currentList) Destroy(gm);};
    }



    public async void reqServer(string ipAddress)
    {
        if(connecting || downloading)
        {
            print("Already connecting!");
            if(downloading){alertB.alert("Please wait until download is finished before reconnecting!");} 
            return;
        } 
        resetGui();
        connecting = true;
        connectionId +=1;
        int thisConnection = connectionId;
        string data = await NetworkingStuff.GetLibrary(ipAddress);
        connecting = false;
        if (thisConnection != connectionId){ print("not the same!!"); return;};
        if(data == null) { alertB.alert("Couldn't get data!"); return;}
        //print(data);
        NetworkLibraryData list = JsonUtility.FromJson<NetworkLibraryData>(data);
        List<string> listData = list.names;
        HashSet<string> nodup = new HashSet<string>(MusicData.getAllMusic());
        HashSet<string> datas = new HashSet<string>();
        foreach (string n in listData)
        {
            print(n);
            if(!nodup.Contains(n)){datas.Add(n);}
        }
        populateUI(new List<string>(datas), true, ipAddress,null);
    }


    public void cancelConnection()
    {
        connectionId += 1;
        connecting = false;
    }


}
