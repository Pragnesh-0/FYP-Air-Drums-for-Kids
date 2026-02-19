using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateKitGui : MonoBehaviour
{
    
    public Button createButton;
    public TMP_InputField drumName;


    public AlertBox alertBox;

    public KitData kitStuff;
    public Editing kitEditing;
    public GuiMenuSelector guiSelector;

    void Start()
    {
        createButton.onClick.AddListener(delegate {createKit();});
    }


    public void createKit()
    {
        string d = kitStuff.saveKit(drumName.text, false);
        if (d != "")
        {
            alertBox.alert(d);
            return;
        }
        guiSelector.selectGui("MainMenu");
        kitEditing.toggleEditing();
    }
}
