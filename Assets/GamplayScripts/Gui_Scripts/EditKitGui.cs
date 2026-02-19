using UnityEngine;
using UnityEngine.UI;

public class EditKitGui : MonoBehaviour
{
    public Button saveButton;
    public Button deleteButton;


    public KitData kitStuff;
    public Editing kitEditing;
    public GuiMenuSelector guiSelector;
    

    void Start()
    {
        saveButton.onClick.AddListener(delegate {saveKit();});
        deleteButton.GetComponent<HoldButton>().addHoldListener(delegate {deleteKit();}, "Hold to Delete!");
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


}
