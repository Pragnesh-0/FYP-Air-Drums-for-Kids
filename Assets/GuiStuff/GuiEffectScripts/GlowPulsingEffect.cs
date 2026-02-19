using UnityEngine;
using UnityEngine.UI;

public class GlowPulsingEffect : MonoBehaviour
{
    public float middleValue = 0.3f;

    public float range = 0.3f;

    public float pulseSpeed = 1f;

    void Update()
    {
        Color c = gameObject.GetComponent<Image>().color;
        c.a = middleValue + (range * Mathf.Sin(pulseSpeed * Time.time));
        gameObject.GetComponent<Image>().color = c;
    }
}
