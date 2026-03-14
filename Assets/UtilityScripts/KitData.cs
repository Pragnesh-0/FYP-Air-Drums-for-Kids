using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Text;

public class KitData : MonoBehaviour
{
    List<KitClass> kitClasses = new List<KitClass>();
    public int kitId = -1;
    public string kitName = "";

    public GameObject equippedKit;
    public GameObject availableDrums;

    public TextMeshProUGUI kitNameE;
    public TextMeshProUGUI kitNameM;
    public RectTransform favStar;

    Color notFaved = Color.white;
    Color faved = Color.yellow;

    public AudioMixerGroup am;

    void rename(string v)
    {
        kitNameE.text = v;
        kitNameM.text = v;
        kitName = v;
    }

    public KitClass getDefaultKit()
    {
        List<DrumClass> drums =  new List<DrumClass>();
        foreach(Transform kd in availableDrums.GetComponent<Transform>())
        {
            DrumClass g = new DrumClass()
            {
                name = kd.gameObject.name,
                type = kd.GetComponent<DrumGameObj>().drumType,
                size = kd.localScale.x,
                position = kd.localPosition,
                volume = kd.GetComponent<AudioSource>().volume,
                panStereo = kd.GetComponent<AudioSource>().panStereo,
            };
            drums.Add(g);
        }

        KitClass saveKit = new KitClass();
        saveKit.drums = drums;
        saveKit.name = "Default";
        saveKit.isFavorite = false;

        return saveKit;
    }
    public void loadKits()
    {
        kitClasses.Clear();
        kitClasses.Add(getDefaultKit());
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "DrumKitData"));
        string[] paths = Directory.GetFiles(Path.Combine(Application.persistentDataPath,"DrumKitData"));

        foreach (string p in paths)
        {
            if(Path.GetExtension(p).ToLower() != ".json") continue;
            string jsonData = File.ReadAllText(p);
            KitClass data = JsonUtility.FromJson<KitClass>(jsonData);
            kitClasses.Add(data);
        }
    }

    public bool loadFromCode(string code)
    {
        KitClass kit = new KitClass();
        try
        {
            byte[] bytes_data = System.Convert.FromBase64String(code);
            string json_data = Encoding.UTF8.GetString(bytes_data);
            kit = JsonUtility.FromJson<KitClass>(json_data);
        }
        catch
        {
            return false;
        }
            
        kit.name = "";
        kit.isFavorite = false;
        foreach(Transform kd in equippedKit.GetComponent<Transform>())
        {
            Destroy(kd.gameObject);
        }
        foreach(DrumClass drum in kit.drums)
        {
            foreach(Transform availableDrum in availableDrums.GetComponent<Transform>())
            {
                if (drum.type == availableDrum.gameObject.name)
                {
                    GameObject drumCopy = Instantiate(availableDrum.gameObject);
                    drumCopy.transform.SetParent(equippedKit.transform);
                    drumCopy.GetComponent<AudioSource>().outputAudioMixerGroup = am;
                    drumCopy.transform.localPosition = drum.position;
                    drumCopy.name = drum.name;
                    drumCopy.GetComponent<DrumGameObj>().setDrumText(drum.name);
                    drumCopy.GetComponent<Transform>().localScale = Vector3.one * drum.size;
                    drumCopy.GetComponent<AudioSource>().volume = drum.volume;
                    drumCopy.GetComponent<AudioSource>().panStereo = drum.panStereo;
                    break;
                }
            }
        }
        return true;
    }

    public string getCode()
    {
        KitClass curKit = new KitClass();
        foreach (KitClass data in kitClasses)
        {
            if (data.name == kitName)
            {
                curKit = data;
            }
        }
        string json_data = JsonUtility.ToJson(curKit);
        byte[] bytes_data = Encoding.UTF8.GetBytes(json_data);
        string code = System.Convert.ToBase64String(bytes_data);
        return code;
    }

    public string saveKit(string kName, bool bsave)
    {
        if (string.IsNullOrWhiteSpace(kName))
        {
            print("Needs A Name");
            return "Please give the Kit A Name!";
        }

        if (!bsave)
        {
           foreach (KitClass k in kitClasses)
            {
                if (k.name == kName)
                {
                    print("Kit Exists Already!");
                    return "A Kit with that Name already exists!";
                }
            } 
        }

        if (kName == "Default")
        {
            print("Default Cannot Exist!");
            return "A Kit with that Name already exists!";
        }
            
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "DrumKitData"));
        string path = Path.Combine(Application.persistentDataPath, "DrumKitData");

        List<DrumClass> drums =  new List<DrumClass>();

        foreach(Transform kd in equippedKit.GetComponent<Transform>())
        {
            DrumClass g = new DrumClass()
            {
                name = kd.gameObject.name,
                type = kd.GetComponent<DrumGameObj>().drumType,
                size = kd.localScale.x,
                position = kd.localPosition,
                volume = kd.GetComponent<AudioSource>().volume,
                panStereo = kd.GetComponent<AudioSource>().panStereo,
            };
            drums.Add(g);
        }

        KitClass saveKit = new KitClass();
        saveKit.drums = drums;
        saveKit.name = kName;
        saveKit.isFavorite = false;
        foreach (KitClass k in kitClasses)
        {
            if (k.name == kName)
            {
                saveKit.isFavorite = k.isFavorite;
            }
        }
        string data = JsonUtility.ToJson(saveKit);
        File.WriteAllText(Path.Combine(path,kName+".json"), data);
        loadKits();
        loadKit(kName);
        return "";
    }

    public void deleteKit()
    {
        if (kitId == -1 || kitId == 0)
        {
            print("Cannot Remove");
            return;
        }
        string path = Path.Combine(Application.persistentDataPath, "DrumKitData");
        string kName = kitClasses[kitId].name;
        if(!File.Exists(Path.Combine(path,kName+".json"))) return;
        File.Delete(Path.Combine(path,kName+".json"));
        loadKits();
        loadKit("Default");
    }

    public void loadKit(string kName)
    {
        rename(kName);
        kitId = -1;
        foreach(Transform kd in equippedKit.GetComponent<Transform>())
        {
            Destroy(kd.gameObject);
        }
        foreach(KitClass kit in kitClasses)
        {
            kitId += 1;
            if (kit.name == kName)
            {
                foreach(DrumClass drum in kit.drums)
                {
                    foreach(Transform availableDrum in availableDrums.GetComponent<Transform>())
                    {
                        if (drum.type == availableDrum.gameObject.name)
                        {
                            GameObject drumCopy = Instantiate(availableDrum.gameObject);
                            drumCopy.transform.SetParent(equippedKit.transform);
                            drumCopy.GetComponent<AudioSource>().outputAudioMixerGroup = am;
                            drumCopy.transform.localPosition = drum.position;
                            drumCopy.name = drum.name;
                            drumCopy.GetComponent<DrumGameObj>().setDrumText(drum.name);
                            drumCopy.GetComponent<Transform>().localScale = Vector3.one * drum.size;
                            drumCopy.GetComponent<AudioSource>().volume = drum.volume;
                            drumCopy.GetComponent<AudioSource>().panStereo = drum.panStereo;
                            break;
                        }
                    }
                }
                if(kName != "Default")
                {
                    favStar.gameObject.SetActive(true);
                    favStar.GetComponent<Image>().color = kit.isFavorite ? faved : notFaved;
                }
                else
                {
                    favStar.gameObject.SetActive(false);
                }
                Settings userSettings = SettingsData.getSettings();
                userSettings.equippedKit = kName;
                SettingsData.saveSettings(userSettings);
                return;
            }
        }
    }

    public void setFav()
    {
        if (kitName == "Default")
        {
            return;
        }
        foreach (KitClass kit in kitClasses)
        {
            if (kit.name == kitName)
            {
                kit.isFavorite = !kit.isFavorite;
            }
        }
        saveKit(kitName, true);
    }

    public void cycleKit(bool add)
    {

        if (add)
        {
            if(kitId == kitClasses.Count-1)
            {
                kitId = 0;
            }
            else
            {
                kitId += 1;
            }
        }
        else
        {
            if(kitId == 0)
            {
                kitId = kitClasses.Count-1;
            }
            else
            {
                kitId -= 1;
            }
        }
        string kName = kitClasses[kitId].name;
        loadKit(kName);
    }
}
