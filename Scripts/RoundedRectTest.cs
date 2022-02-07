using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundedRectTest : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float t = 1;
    public float x, y, z, w, h, r;
    public int amount;
    RoundedRectangle rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = new RoundedRectangle(x, y, z, w, h, r);
    }

    // Update is called once per frame
    void Update()
    {
        // t = (t + Time.deltaTime) % 1;
        // rect = new RoundedRectangle(x, y, z, w, h, r);
        // Vector3 newPoint = rect.getPoint(t);
        // if (newPoint.magnitude < 50000)
        // {
        //     transform.position = newPoint;
        // }
        // else
        // {
        //     Debug.Log("BRUHHHHHH");
        // }
    }

    void OnDrawGizmos()
    {
        // if (rect != null)
        // {
            rect = new RoundedRectangle(x, y, z, w, h, r);
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.red;
            for (int i = 0; i < amount; i++)
            {
                float newT = (((float)i / amount) + Time.time / 20) % 1;
                Gizmos.DrawSphere(rect.getPoint(newT), 1);
            }
        }
    // }
}
