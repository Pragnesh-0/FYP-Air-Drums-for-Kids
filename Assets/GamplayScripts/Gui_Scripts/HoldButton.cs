using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public HoldLogic hl;

    private Action listener;
    private string text;
    

    

    public void OnPointerDown(PointerEventData ed)
    {
        hl.hold(listener, 2f, text);
    }

    public void OnPointerUp(PointerEventData ed)
    {
        hl.deny();
    }


    public void addHoldListener(Action todo, string data)
    {
        listener = todo;
        text = data;
    }
}
