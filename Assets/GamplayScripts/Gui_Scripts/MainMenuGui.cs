using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuGui : MonoBehaviour
{
    public GameObject mainKit;


    public GuiMenuSelector guiSelector;
    public Editing editObject;
    public KitData kitData;

    public Button editButton;
    public Button createButton;
    public Button cycleLeft;
    public Button cycleRight;
    public Button settingsButton;
    public Button minigameButton;

    public Button playBytton;



    public PlaneKit loadHitboxes;

    
    float cycleDebounce = 0;


    public AlertBox alertBox;

    void Start()
    {
        editButton.onClick.AddListener( delegate{edit();});
        createButton.onClick.AddListener(delegate{create();});
        cycleLeft.onClick.AddListener(delegate{if(Time.time < cycleDebounce){return;} kitData.cycleKit(false);  cycleDebounce = Time.time + 0.5f;});
        cycleRight.onClick.AddListener(delegate{if(Time.time < cycleDebounce){return;} kitData.cycleKit(true);  cycleDebounce = Time.time + 0.5f;});
        playBytton.onClick.AddListener(delegate{playMain();});

        settingsButton.onClick.AddListener(delegate{enableGui("Settings");});
        minigameButton.onClick.AddListener(delegate{enableGui("Minigame");});
    }


    public void edit()
    {
        if (kitData.kitId == 0)
        {
            alertBox.alert("You can not edit the default kit!");
            return;
        }
        guiSelector.selectGui("KitEditor");
        editObject.toggleEditing();
    }


    public void create()
    {
        guiSelector.selectGui("KitCreator");
        editObject.toggleEditing();
    }


    public void playMain()
    {
        guiSelector.selectGui("FreeMode");
        loadHitboxes.setValues(false);
    }

    public void enableGui(string v)
    {
        guiSelector.selectGui(v);
    }
}
