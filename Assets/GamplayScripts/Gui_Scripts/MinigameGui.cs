using UnityEngine;
using UnityEngine.UI;
public class MinigameGui : MonoBehaviour
{
    
    public Button back;
    public Button connection;
    public Button library;

    public RectTransform libraryPop;


    public RectTransform connectionPanel;
    public RectTransform musicPanel;



    public GuiMenuSelector gms;

    void Start()
    {
        back.onClick.AddListener(delegate{backUp();});
        connection.onClick.AddListener(delegate{connectionGui();});
        library.onClick.AddListener(delegate{musicSelectorGui();});
    }


    public void connectionGui()
    {
        musicPanel.gameObject.SetActive(false);
        connectionPanel.gameObject.SetActive(true);
        libraryPop.GetComponent<LibraryPopGui>().resetGui();
        libraryPop.gameObject.SetActive(true);
    }

    public void musicSelectorGui()
    {
        connectionPanel.gameObject.SetActive(false);
        connectionPanel.GetComponent<ConnectionGui>().cancelOperations();
        libraryPop.gameObject.GetComponent<LibraryPopGui>().loadFromFiles(delegate(string val){addToSelector(val);});
        libraryPop.gameObject.SetActive(true);
    }

    public void addToSelector(string val)
    {
        bool value = musicPanel.GetComponent<SelectorGui>().selectSong(val, delegate(bool v) {
            if (v)
            {
                musicPanel.gameObject.SetActive(false); 
            }
            
            musicSelectorGui();
        });
        musicPanel.gameObject.SetActive(value);
    }

    void backUp()
    {
        musicPanel.gameObject.SetActive(false);
        connectionPanel.gameObject.SetActive(false);
        libraryPop.gameObject.SetActive(false); 
        connectionPanel.GetComponent<ConnectionGui>().cancelOperations();
        gms.selectGui("MainMenu");
    }
}
