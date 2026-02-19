using UnityEngine;

public class BetterDetection : MonoBehaviour
{

    public ComputerVisionDetection cv;
    public Vector2 stickpos = Vector2.zero;
    public Vector2 lastStickPos;
    public float hitSpeed;
    public bool active = false;
    public float delay;

    public int stickId;

    public float reboundThreshold;


    public float abnormalityTimer;


    
    public RectTransform stick;


    public PlaneKit pk;

    void Update()
    {
        float curDeltaY = getScaledYDelta();
        float speed = getScaledSpeed();
        lastStickPos = stickpos;
        if (curDeltaY < 0 && speed > 500)
        {
            if (abnormalityTimer < 250)
            {
                active = true;
                delay = 0;
                if (speed > hitSpeed)
                {
                    hitSpeed = speed;
                }
            }
            abnormalityTimer = 0;
        }
        else if (curDeltaY > reboundThreshold)
        {
            if (active)
            {
                playSound(stickpos);
            }
            active = false;
            delay = 0;
            hitSpeed = 0;
        }

        if (active)
        {
            delay += Time.deltaTime * 1000;
            if (delay > 130)
            {
                delay = 0;
                active = false;
            }
        }
        

        stickpos = cv.getStickPos(stickId);
        if (stickpos == lastStickPos)
        {
            abnormalityTimer += Time.deltaTime * 1000;
            return;
        }
        stick.position = stickpos * new Vector2(Screen.width, Screen.height);
    }

 

    float getScaledYDelta()
    {
        Vector2 nextp = cv.getStickPos(stickId);
        return (nextp.y - stickpos.y) / Time.deltaTime * 1000;
    }

    float getScaledSpeed()
    {
        Vector2 p = cv.getStickPos(stickId);
        return (p - stickpos).magnitude / Time.deltaTime * 1000;
    }

    void playSound(Vector2 pos)
    {
        pk.playSound(pos);
    }


    public void setReboundThreshold(float val)
    {
        reboundThreshold = 1000 * val;
    }
}
