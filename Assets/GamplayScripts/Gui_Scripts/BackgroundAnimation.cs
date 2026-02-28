using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{

    Vector2 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }
    void Update()
    {
        transform.localPosition = startPos + (Vector2.one*new Vector2(12.5f*Mathf.Sin(Time.time),12.5f*Mathf.Cos(Time.time)));
    }
}
