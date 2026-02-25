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
            ring.localScale = (Vector2.one * 2f) - (Vector2.one * (timeElapsed/timer));
            await Awaitable.NextFrameAsync();
        }
    }

    async Awaitable showValue()
    {
        score.gameObject.SetActive(true);
        circle.SetActive(false);
        float timeElapsed = 0;
        RectTransform label = score.gameObject.GetComponent<RectTransform>();
        Vector2 startPos = label.localPosition;
        while (timeElapsed < 1f)
        {
            timeElapsed += Time.deltaTime;
            label.localPosition = startPos + (new Vector2(0, timeElapsed/1f)*150f);
            label.localScale = Vector2.one - (Vector2.one * 0.75f * (timeElapsed/1f));
            await Awaitable.NextFrameAsync();
        }
    }

}
