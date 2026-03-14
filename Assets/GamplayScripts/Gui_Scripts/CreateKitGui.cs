using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateKitGui : MonoBehaviour
{
    
    public Button createButton;
    public Button pasteCodeButton;
    public TMP_InputField drumName;


    public AlertBox alertBox;

    public KitData kitStuff;
    public Editing kitEditing;
    public GuiMenuSelector guiSelector;

    void Start()
    {
        createButton.onClick.AddListener(delegate {createKit();});
        pasteCodeButton.onClick.AddListener(delegate {pasteCode();});
    }


    public void createKit()
    {
        string d = kitStuff.saveKit(drumName.text, false);
        if (d != "")
        {
            alertBox.alert(d);
            return;
        }
        drumName.text = "";
        guiSelector.selectGui("MainMenu");
        kitEditing.toggleEditing();
    }

    public void pasteCode()
    {
        if (!kitStuff.loadFromCode(GUIUtility.systemCopyBuffer))
        {
            alertBox.alert("Invalid code was given!");
        }
        ;
    }
}
