using UnityEngine;
using TMPro;
public class DownloadingScript : MonoBehaviour
{

    public TextMeshProUGUI label;
    int count = 0;

    float timeElapsed = 0;
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed < 0.5f) return;
        timeElapsed = 0;
        count += 1;
        label.SetText("Downloading"+ new string('.',count));
        if (count > 2)
        {
            count = 0;
        }
    }
}
