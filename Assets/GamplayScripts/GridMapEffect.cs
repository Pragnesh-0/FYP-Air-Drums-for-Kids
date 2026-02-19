using UnityEngine;

public class GridMapEffect : MonoBehaviour
{


    public GameObject floor;
    public GameObject wall;

    
    bool isSpiked = false;
    float intensity = 0f;
    float elapsedTime = 0f;
    bool wasHit = false;    
    public Color currentColor = Color.cyan;


    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (isSpiked)
        {
            isSpiked = false;
            intensity = 10f;
            elapsedTime = 0f;
            wasHit = true;
        }
        else
        {
            if (10f *  Mathf.Cos(5*elapsedTime) > 3f + (2f * Mathf.Sin(2*Time.time)) && wasHit)
            {
                intensity = 10f *  Mathf.Cos(5*elapsedTime);
            }
            else
            {
                wasHit = false;
                intensity = 3f + (2f * Mathf.Sin(2*Time.time));
            }
        }

        floor.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", currentColor * intensity);
        wall.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", currentColor * intensity);
        
    }

    public void spikeBloom()
    {
        isSpiked = true;
    }

    public void changeColor(Color c)
    {
        currentColor = c;
    }
}
