using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{


    public AudioSource clickSound;

    public float normalScale = 1f;
    public RectTransform icon;
    public bool playSound = true;

    bool isHovering = false;

    void Start()
    {
        clickSound = GameObject.FindGameObjectWithTag("ButtonClick").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isHovering)
        {
            icon.rotation = Quaternion.Euler(0,0,15*Mathf.Sin(Time.time * 2));
        }
    }


    public void OnPointerEnter(PointerEventData pdata)
    {
        gameObject.GetComponent<RectTransform>().localScale = (Vector3.one * normalScale) + (Vector3.one*.1f);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData pdata)
    {
        gameObject.GetComponent<RectTransform>().localScale = Vector3.one * normalScale;
        isHovering = false;
        icon.rotation = Quaternion.Euler(0,0,0);
    }

    public void OnPointerClick(PointerEventData pdata)
    {
        if (playSound)
        {
            clickSound.pitch = 1.5f + Random.Range(-0.15f,0.15f);
            clickSound.PlayOneShot(clickSound.clip);
        }
        OnPointerExit(null);
    }

    public void OnPointerDown(PointerEventData pdata)
    {
        OnPointerEnter(null);
    }


}
