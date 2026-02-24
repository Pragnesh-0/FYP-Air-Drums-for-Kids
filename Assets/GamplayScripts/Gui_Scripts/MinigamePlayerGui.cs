using UnityEngine;
using UnityEditor.UI;
public class MinigamePlayerGui : MonoBehaviour
{
    
    public HoldButton button;
    public GuiMenuSelector gms;
    public GameObject libraryPop;

    void Start()
    {
        button.addHoldListener(delegate{
            gms.selectGui("Minigame");
            libraryPop.SetActive(true);
        },"");
    }
}
