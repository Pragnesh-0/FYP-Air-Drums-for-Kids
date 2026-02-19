using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

public class GuiMenuSelector : MonoBehaviour
{
    
    public List<GameObject> panelList;
    public List<Color> backgroundColors;

    public Dictionary<string, GameObject> panelDictionary = new Dictionary<string, GameObject>();
    public Dictionary<string, Color> colorDictionary = new Dictionary<string, Color>();
    public GameObject Scrim;
    public GridMapEffect gridEffect;

    GameObject currentMenu;

    void Awake()
    {
        int index = -1;
        foreach (GameObject panel in panelList)
        {
            index += 1;
            panelDictionary.Add(panel.name, panel);
            colorDictionary.Add(panel.name, backgroundColors[index]);
        }
    }

    public async void selectGui(string n)
    {
        Scrim.SetActive(true);
        Vector2 pos = Mouse.current.position.ReadValue()/new Vector2(Screen.width, Screen.height);
        await transition(true, pos, .1f, gridEffect.currentColor, colorDictionary[n]);
        foreach (GameObject panel in panelList)
        {
            panel.SetActive(false);
            panel.GetComponent<RectTransform>().localScale = Vector3.zero;
        }
        currentMenu = null;
        if (panelDictionary.ContainsKey(n))
        {
            panelDictionary[n].SetActive(true);
            if (!currentMenu)
            {
                currentMenu = panelDictionary[n];
                await transition(false, pos, .15f, colorDictionary[n], colorDictionary[n]);
                currentMenu.GetComponent<RectTransform>().localScale = Vector3.one;
            } 
        }
        gridEffect.changeColor(colorDictionary[n]);
        Scrim.SetActive(false);
    }


    async Awaitable transition(bool reverse, Vector2 pivot, float targetTime, Color c1 , Color c2)
    {
        if(currentMenu == null){ return; }
        float elapsedTime = 0;
        currentMenu.GetComponent<RectTransform>().pivot = pivot;
        while (elapsedTime < targetTime)
        {
            if (reverse)
            {
                gridEffect.changeColor(Color.Lerp(c1,c2,elapsedTime/0.1f));
            }
            elapsedTime += Time.deltaTime;
            currentMenu.GetComponent<RectTransform>().localScale = reverse? Vector3.one - (((Vector3.one)*.9f) * (elapsedTime/targetTime)) : Vector3.zero + (Vector3.one * (elapsedTime/targetTime));
            await Awaitable.NextFrameAsync();
        }
        currentMenu.GetComponent<RectTransform>().pivot = Vector2.one * .5f;
    }


}
