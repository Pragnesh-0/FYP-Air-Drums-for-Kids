using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertBox : MonoBehaviour
{
    public Button confirmation;
    public TextMeshProUGUI message;

    public GameObject alertBox;

    void Start()
    {
        confirmation.onClick.AddListener(async delegate{await showTransition(true); gameObject.SetActive(false);});
    }

    
    public async void alert(string msg)
    {
        message.text = msg;
        alertBox.GetComponent<RectTransform>().localScale = Vector2.zero;
        gameObject.SetActive(true);
        await showTransition(false);
    }
    async Awaitable showTransition(bool reverse)
    {
        float elapsedTime = 0;
        while (elapsedTime < 0.15f)
        {
            elapsedTime += Time.deltaTime;
            alertBox.GetComponent<RectTransform>().localScale = reverse?  Vector2.one - (Vector2.one * (elapsedTime/0.15f)) : Vector2.one * (elapsedTime/0.15f);
            await Awaitable.NextFrameAsync();
        }
        alertBox.GetComponent<RectTransform>().localScale = reverse? Vector2.zero : Vector2.one;
    }
}
