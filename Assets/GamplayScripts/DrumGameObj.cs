using UnityEngine;
using TMPro;

public class DrumGameObj : MonoBehaviour
{
    public string drumType;
    public string drumDescription;

    public GameObject actualDrum;

    public TextMeshPro drumText;


    public bool isCymbal = false;

    int count;

    void Update()
    {
        actualDrum.transform.LookAt(Camera.main.transform.position, Vector2.up);
    }

    public async void onHit()
    {
        await hitEffect();
    }

    async Awaitable hitEffect()
    {
        count += 1;
        int lCount = count;
        float timeElapsed = 0;
        while (timeElapsed < 1f)
        {
            timeElapsed += Time.deltaTime;
            float scaleVal = Mathf.Pow(2, -20*timeElapsed)*Mathf.Sin((timeElapsed*10f-7.5f)*((2*Mathf.PI)/3)) + 1f;
            if (lCount != count)
            {
                break;
            }
            actualDrum.transform.localScale = new Vector3(1, 1, 1) * scaleVal;
            await Awaitable.NextFrameAsync();
        }
        actualDrum.transform.localScale = new Vector3(1, 1, 1);
    }

    public void setDrumText(string dText)
    {
        drumText.SetText(dText);
    }
}
