using UnityEngine;
using System.Collections.Generic;
public static class DrumKit2dData
{
    
    public static List<Vector4> planeData(GameObject drumKit)
    {
        int index = -1;
        List<Vector4> rectangles = new List<Vector4>();
        Camera cam = Camera.main;

        foreach(Transform t in drumKit.GetComponent<Transform>())
        {
            index += 1;
            Vector3[] vertices = new Vector3[8];
            Vector3 l = t.GetComponent<BoxCollider>().size/2f;
            vertices[0] = new Vector3(l.x, l.y, l.z);
            vertices[1] = new Vector3(-l.x, -l.y, -l.z);
            vertices[2] = new Vector3(-l.x, -l.y, l.z);
            vertices[3] = new Vector3(-l.x, l.y, -l.z);
            vertices[4] = new Vector3(l.x, -l.y, -l.z);
            vertices[5] = new Vector3(l.x, -l.y, l.z);
            vertices[6] = new Vector3(-l.x, l.y, l.z);
            vertices[7] = new Vector3(l.x, l.y, -l.z);

            rectangles.Add(new Vector4(1000000,1000000,-1000000,-1000000));

            for(int i = 0; i < 8; i++)
            {
                Vector3 pos = t.TransformPoint(vertices[i]);
                Vector2 v = cam.WorldToScreenPoint(pos);
                v.x = v.x/Screen.width;
                v.y = v.y/Screen.height;
                if(v.x < rectangles[index].x)
                {
                    rectangles[index] = new Vector4(v.x, rectangles[index].y, rectangles[index].z, rectangles[index].w);
                }
                if(v.y < rectangles[index].y)
                {
                    rectangles[index] = new Vector4(rectangles[index].x, v.y, rectangles[index].z, rectangles[index].w);
                }
                if(v.x > rectangles[index].z)
                {
                    rectangles[index] = new Vector4(rectangles[index].x, rectangles[index].y, v.x, rectangles[index].w);
                }
                if(v.y > rectangles[index].w)
                {
                    rectangles[index] = new Vector4(rectangles[index].x, rectangles[index].y, rectangles[index].z, v.y);
                }
            }
        }

        return rectangles;
    }

    


}
