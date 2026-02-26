using UnityEngine;

public class StickObject3D : MonoBehaviour
{
    
    Transform stickObject;

    void Start()
    {
        stickObject = gameObject.transform;
    }

    void Update()
    {
        Quaternion targetRotation = Quaternion.LookRotation(stickObject.position, (stickObject.position - Camera.main.transform.position).normalized * 1000f);
        stickObject.rotation = Quaternion.Slerp(stickObject.rotation, targetRotation, 7f * Time.deltaTime);
    }



    public void changePosition(Vector2 normVals)
    {
        if(!stickObject) return;
        Vector2 screenPos = normVals * new Vector2(Screen.width, Screen.height);
        Vector3 _3dPosition = new Vector3(screenPos.x, screenPos.y, 4);
        stickObject.position = Camera.main.ScreenToWorldPoint(_3dPosition);
    }
}
