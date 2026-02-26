using UnityEngine;
using UnityEngine.UI;
using  TMPro;
using System.Threading.Tasks;
public class CircleClosing : MonoBehaviour
{

    public GameObject circle;
    public RectTransform ring;
    public TextMeshProUGUI score;

    bool triggered = false;

    public async void scoreNotif(string s)
    {
        if (triggered) return;
        score.SetText(s);
        triggered = true;
        await showValue();
        Destroy(gameObject);
    }

    public async void activateObj(float timer)
    {
        await closeCircle(timer);
        if (triggered) return;
        scoreNotif("missed");
    }

    async Awaitable closeCircle(float timer)
    {
        float timeElapsed = 0;
        while (timeElapsed < timer)
        {
            timeElapsed += Time.deltaTime;
            ring.localScale = (Vector3.one * 2f) - (Vector3.one * (timeElapsed/timer));
            await Awaitable.NextFrameAsync();
        }
    }

    async Awaitable showValue()
    {
        float timeElapsed = 0;
        float timeElapsed2 = 0;
        RectTransform circleT =  circle.GetComponent<RectTransform>();
        RectTransform label = score.gameObject.GetComponent<RectTransform>();
        Vector2 startPos = label.localPosition;
        while (timeElapsed2 < 0.1f)
        {
            timeElapsed2 += Time.deltaTime;
            circleT.localScale = Vector3.one - (Vector3.one * (timeElapsed2/0.1f));
            await Awaitable.NextFrameAsync();
        }
        circle.SetActive(false);
        score.gameObject.SetActive(true);
        while (timeElapsed < 0.8f)
        {
            timeElapsed += Time.deltaTime;
            label.localPosition = startPos + (new Vector2(0, timeElapsed/0.8f)*150f);
            label.localScale = (Vector3.one * 2f) - (Vector3.one * 1.75f * (timeElapsed/0.8f));
            await Awaitable.NextFrameAsync();
        }
    }

}
