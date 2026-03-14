using UnityEngine;
using UnityEngine.UI;

public class EditKitGui : MonoBehaviour
{
    public Button saveButton;
    public Button deleteButton;
    public Button getCodeButton;


    public KitData kitStuff;
    public Editing kitEditing;
    public GuiMenuSelector guiSelector;
    public AlertBox alert;
    

    void Start()
    {
        saveButton.onClick.AddListener(delegate {saveKit();});
        deleteButton.GetComponent<HoldButton>().addHoldListener(delegate {deleteKit();}, "Hold to Delete!");
        getCodeButton.onClick.AddListener(delegate {getCodeCopy();});
    }


    public void saveKit()
    {
        kitStuff.saveKit(kitStuff.kitName, true);
        guiSelector.selectGui("MainMenu");
        kitEditing.toggleEditing();
    }

    public void deleteKit()
    {
        kitStuff.deleteKit();
        guiSelector.selectGui("MainMenu");
        kitEditing.toggleEditing();
    }


    public void getCodeCopy()
    {
        GUIUtility.systemCopyBuffer = kitStuff.getCode();
        alert.alert("Kit code copied to Clipboard");
    }


}
