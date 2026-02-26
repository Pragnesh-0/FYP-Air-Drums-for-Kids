using UnityEngine;

public class DrumGameObj : MonoBehaviour
{
    public string drumType;
    public string drumDescription;

    public GameObject actualDrum;


    void Update()
    {
        actualDrum.transform.LookAt(Camera.main.transform.position, Vector2.up);
    }

    public void onHit()
    {
        //hit animation
    }
}
