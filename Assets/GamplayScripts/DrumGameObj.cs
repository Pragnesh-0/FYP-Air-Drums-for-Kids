using UnityEngine;

public class DrumGameObj : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string drumType;
    public string drumDescription;

    public GameObject actualDrum;

    // Update is called once per frame
    void Update()
    {
        actualDrum.transform.LookAt(Camera.main.transform.position, Vector2.up);
    }

    public void onHit()
    {
        //hit animation
    }
}
