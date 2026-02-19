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
        back.onClick.AddListener(delegate{libraryPop.gameObject.SetActive(false); connectionPanel.GetComponent<ConnectionGui>().cancelConnection(); gms.selectGui("MainMenu");});
        connection.onClick.AddListener(delegate{connectionGui();});
        library.onClick.AddListener(delegate{musicSelectorGui();});
    }


    public void connectionGui()
    {
        musicPanel.gameObject.SetActive(false);
        connectionPanel.gameObject.SetActive(true);
        connectionPanel.GetComponent<ConnectionGui>().localConnect();
        libraryPop.gameObject.SetActive(true);
    }

    public void musicSelectorGui()
    {
        connectionPanel.gameObject.SetActive(false);
        connectionPanel.GetComponent<ConnectionGui>().cancelConnection();
        musicPanel.gameObject.SetActive(true);
        libraryPop.gameObject.GetComponent<LibraryPopGui>().loadFromFiles(delegate(string val){addToSelector(val);});
        libraryPop.gameObject.SetActive(true);
    }

    void addToSelector(string val)
    {
        print("Do thing with "+val);
    }
}
