using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class Editing : MonoBehaviour
{
    
    GameObject selectedDrum;
    //bool wasMoving = false;

    public GameObject currentKit;

    public GameObject drumCreator;
    public GameObject propertyEditingGui;
    public GameObject drumSelectionGui;
    public Button selectionButton;
    public Button deleteItem;


    public GuiMenuSelector menuSelector;
    public Button cancel;
    public KitData kitScript;
    public AlertBox alertBox;


    bool interactedWUI = false;
    bool moving;

    void Start()
    {
        selectionButton.onClick.AddListener(selectionButtonClicked);

        cancel.GetComponent<HoldButton>().addHoldListener(delegate{ 
            menuSelector.selectGui("MainMenu"); 
            toggleEditing(); 
            kitScript.loadKit(kitScript.kitName);
        }, "Hold to Cancel!");

        deleteItem.GetComponent<HoldButton>().addHoldListener(delegate{
            removeDrum();
        }, "Hold to Delete!");
    }

    void Update()
    {

        if (!drumCreator.activeSelf)
        {
            return;
        }

        Vector2 p = Mouse.current.position.ReadValue();
        
        if (Mouse.current.leftButton.isPressed)
        {
            if (EventSystem.current.IsPointerOverGameObject() && !moving) 
            {
                interactedWUI = true;
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(p);
            if(Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject.transform.parent.gameObject == currentKit && !interactedWUI)
                {
                    selectedDrum = hitInfo.collider.gameObject;
                    propertyEditingGui.GetComponent<PropertyEditing>().updateProperties(selectedDrum);
                    propertyEditingGui.SetActive(true);
                }
            }
            else
            {
                if(interactedWUI) { return; }
                selectedDrum = null;
                propertyEditingGui.SetActive(false);
            }
        }

        if (Mouse.current.leftButton.isPressed && selectedDrum)
        {
            if(EventSystem.current.currentSelectedGameObject != null || EventSystem.current.IsPointerOverGameObject() && !moving)
            {
                interactedWUI = true;
                return;
            }
            moving = true;
            selectedDrum.GetComponent<Transform>().position = Camera.main.ScreenToWorldPoint(new Vector3(p.x, p.y, 6f));
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (selectedDrum && moving)
            {
                moving = false;
                selectedDrum.GetComponent<Transform>().position = Camera.main.ScreenToWorldPoint(new Vector3(p.x, p.y, 8f));
            }
            interactedWUI = false;
        }
        
    }

    public void toggleEditing()
    {
        drumSelectionGui.SetActive(false);
        propertyEditingGui.SetActive(false);
        selectionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "View drum Selection";
        drumCreator.SetActive(!drumCreator.activeSelf);
    }


    public void selectionButtonClicked()
    {
        drumSelectionGui.SetActive(!drumSelectionGui.activeSelf);
        string txt = drumSelectionGui.activeSelf ? "Hide" : "View drum Selection";
        selectionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = txt;
    }


    

    public void changeSize(float value)
    {
        if (selectedDrum)
        {
            selectedDrum.GetComponent<Transform>().localScale = Vector3.one * value;
        }
    }

    public void changeVolume(float value)
    {
        if (selectedDrum)
        {
            selectedDrum.GetComponent<AudioSource>().volume = value;
        }
    }

    public void changePan(float value)
    {
        if (selectedDrum)
        {
            selectedDrum.GetComponent<AudioSource>().panStereo = value;
        }
    }

    public void changeName(string value)
    {
        if (selectedDrum)
        {
            selectedDrum.name = value;
            selectedDrum.GetComponent<DrumGameObj>().setDrumText(value);
        }
    }

    public void playDrum()
    {
        if (selectedDrum)
        {
            selectedDrum.GetComponent<AudioSource>().PlayOneShot(selectedDrum.GetComponent<AudioSource>().clip);
        }
    }

    public void removeDrum()
    {
        propertyEditingGui.SetActive(false);
        string n = selectedDrum.name;
        if (selectedDrum)
        {
            Destroy(selectedDrum);
            selectedDrum = null;
            alertBox.alert(n + " has been deleted!");
        }
    }

    public void addDrum(GameObject drum_type)
    {
        if (currentKit.transform.childCount > 12)
        {
            alertBox.alert("Drum limits Reached!");
            return;
        }
        GameObject copy = Instantiate(drum_type);
        copy.name = drum_type.name;
        copy.GetComponent<DrumGameObj>().setDrumText(drum_type.name);
        copy.transform.SetParent(currentKit.transform);
        copy.transform.position = currentKit.transform.position;
    }


    
}
