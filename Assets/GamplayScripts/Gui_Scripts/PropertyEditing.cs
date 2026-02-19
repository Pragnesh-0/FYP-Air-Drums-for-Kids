using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PropertyEditing : MonoBehaviour
{
    public Editing editingObj;

    public Slider sizeChanger;
    public Slider panChanger;
    public Slider volChanger;
    public TMP_InputField nameChanger;
    public TextMeshProUGUI drumType;
    public TextMeshProUGUI drumDescription;
    public Button playButton;


    
    void Start()
    {
        sizeChanger.onValueChanged.AddListener(delegate {editingObj.changeSize(sizeChanger.value);});
        volChanger.onValueChanged.AddListener(delegate {editingObj.changeVolume(volChanger.value);});
        panChanger.onValueChanged.AddListener(delegate {editingObj.changePan(panChanger.value);});
        nameChanger.onValueChanged.AddListener (delegate {editingObj.changeName(nameChanger.text);});
        playButton.onClick.AddListener(delegate {editingObj.playDrum();});
    }

    public void updateProperties(GameObject go)
    {
        sizeChanger.value = go.transform.localScale.x;
        volChanger.value = go.GetComponent<AudioSource>().volume;
        panChanger.value = go.GetComponent<AudioSource>().panStereo;
        nameChanger.text = go.name;
        drumType.text = go.GetComponent<DrumGameObj>().drumType;
        drumDescription.text = go.GetComponent<DrumGameObj>().drumDescription;
    }
}
