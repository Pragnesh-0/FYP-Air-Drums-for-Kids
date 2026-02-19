using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameTest : MonoBehaviour
{

    public RectTransform drumKit2d;
    public GameObject equippedKit;
    public KitData kitStuff;

    public GuiMenuSelector selector;
    //test
    //public Button but;
    //test

    public List<Transform> hitboxesTest = new List<Transform>{};

    void Start()
    {
        kitStuff.loadKits();
        kitStuff.loadKit("Default");
        //but.onClick.AddListener(SetUp);
    }

    public void SetUp()
    {
        //check current drumkit game object here
        
        //test
        for (int i = 0; i < hitboxesTest.Count; i++)
        {
            Transform theObj = hitboxesTest[i];
            Destroy(theObj.gameObject);
        }
        hitboxesTest.Clear();
        //test

        List<Vector4> rectangles = new List<Vector4>();


        rectangles = DrumKit2dData.planeData(equippedKit);

        List<GameObject> items = new List<GameObject>();


        //to remove
        for(int i = 0; i < rectangles.Count; i++)
        {
            Vector2 mdpt = new Vector2((rectangles[i].x + rectangles[i].z)/2, (rectangles[i].y + rectangles[i].w)/2);

            GameObject rw = new GameObject(i+"D");
            rw.transform.SetParent(drumKit2d.gameObject.transform);
            RawImage rm = rw.AddComponent<RawImage>();

            rm.rectTransform.position = mdpt;
            rm.rectTransform.sizeDelta = new Vector2(Mathf.Abs(rectangles[i].x - rectangles[i].z), Mathf.Abs(rectangles[i].y - rectangles[i].w));
            
            hitboxesTest.Add(rw.GetComponent<Transform>());
        }

        foreach(Transform obj in equippedKit.GetComponent<Transform>())
        {
            items.Add(obj.gameObject);
        }

        //drumKit2d.GetComponent<PlaneKit>().setValues(items,rectangles,false);

    }
}
