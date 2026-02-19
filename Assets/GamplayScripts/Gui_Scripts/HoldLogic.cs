using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;



public class HoldLogic: MonoBehaviour
{
    public Image radial;
    public RectTransform panel;


    public AudioSource holdSuccess;
    public AudioSource charge;

    bool hasDenied = false;

    public async void hold(Action action, float timer, string data)
    {
        radial.color = Color.white;
        panel.gameObject.SetActive(true);
        await logic(action, timer);
    }

    public void deny()
    {
        hasDenied = true;
    }

    public async Awaitable logic(Action action, float timer)
    {
        float timeElapsed = 0;
        charge.Play();
        float deniedElapsed = 0;
        radial.fillAmount = 0;
        Vector2 startPos = radial.gameObject.GetComponent<RectTransform>().localPosition;
        while (timeElapsed < timer)
        {
            timeElapsed += Time.deltaTime;
            if (hasDenied)
            {
                break;
            }
            radial.color = Color.Lerp(Color.white, Color.red, timeElapsed/timer);
            radial.fillAmount = timeElapsed/timer;
            radial.gameObject.GetComponent<RectTransform>().localScale = Vector2.one + (Vector2.one*0.2f*timeElapsed/timer);
            radial.gameObject.GetComponent<RectTransform>().localPosition = startPos + Vector2.one*UnityEngine.Random.Range(-5f,5f)*timeElapsed*(MathF.Sin(10*timeElapsed*timeElapsed));
            await Awaitable.NextFrameAsync();
        }
        radial.gameObject.GetComponent<RectTransform>().localPosition = startPos;
        if (hasDenied)
        {
            float curFillAmount = radial.fillAmount;
            Vector2 curScale = radial.gameObject.GetComponent<RectTransform>().localScale;
            while (deniedElapsed < 0.1f)
            {
                deniedElapsed += Time.deltaTime;
                radial.fillAmount = curFillAmount - (curFillAmount * (deniedElapsed/0.1f));
                radial.gameObject.GetComponent<RectTransform>().localScale = curScale - (curScale * (deniedElapsed/0.1f));
                charge.volume = 0.5f - (0.5f * (deniedElapsed/0.1f));
                await Awaitable.NextFrameAsync();
            }
            hasDenied = false;
        }
        else
        {
            action();
            holdSuccess.PlayOneShot(holdSuccess.clip);
        }
        charge.Stop();
        panel.gameObject.SetActive(false);
        radial.gameObject.GetComponent<RectTransform>().localScale = Vector2.one;
        charge.volume = 0.5f;
    }

}