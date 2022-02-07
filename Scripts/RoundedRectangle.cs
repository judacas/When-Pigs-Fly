using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundedRectangle : MonoBehaviour
{

    float x, z, y, w, h, r, p, vertical, curve, horizontal;
    bool isFlat;
    // xyz coordinate,  width and height, radius of curved edges, total circumference;
    public RoundedRectangle(float _x, float _z, float _y, float _w, float _h, float _r)
    {
        x = _x;
        z = _z;
        y = _y;
        w = Mathf.Max(_w, 0);
        h = Mathf.Max(_h, 0);
        r = Mathf.Max(0f, Mathf.Min(_r, Mathf.Min(_w/2f, _h/2f)));
        if (_r > Mathf.Min(_w, _h))
        {
            Debug.Log("AYO ILLEGAL RADIUS");
        }
        //perimeter
        vertical = (h / 2) - r;
        curve = vertical + (Mathf.PI * r / 2);
        horizontal = curve + (w / 2) - r;
        // Debug.Log("vertical " + vertical);
        // Debug.Log("curve " + curve);
        // Debug.Log("horizontal " + horizontal);
        p = (2 * (w - (2 * r))) + (2 * (h - (2 * r))) + (2 * Mathf.PI * r);

        if (r < 0 || w < 0 || h < 0 || r > w || r > h)
        {
            Debug.Log("Rounded Rectangle has been given some invalid parameters");
        }

        isFlat = (r == 0);

    }

    //this serves as basically a parametric function to get a point on the rounded rectangle
    //give it a float t in the range [0,1] and it wil output a vector3 on the rounded rectangle
    //that is a t percentage from the right middle of the rr clockwise
    public Vector3 getPoint(float t)
    {
        Vector3 point;
        bool yFlip = false, xFlip = false;
        //distance traveled from right middle
        float d = t * p;
        // Debug.Log("D-Value: " + d);

        t = t%1;

            if (t > 0.5f)
            {
                t = 1 - t;
                d = t * p;
                yFlip = true;
            }
            if (d > horizontal)
            {
                d = (2 * horizontal) - d;
                xFlip = true;
            }
            if (yFlip)
            {
                // d = (2 * horizontal) - d;
            }
            //in right vertical line
            if (d <= vertical)
            {
                point = new Vector3((w / 2), y, d);
            }
            //adding the circumference of a fourth of a circle
            else if (d <= curve)
            {
                float temp = d - curve;
                point = new Vector3(((w / 2) - r) + (r * Mathf.Cos((d - vertical) / r)), y, ((h / 2) - r) + (r * Mathf.Sin((d - vertical) / r)));
            }
            else if (d <= horizontal)
            {
                point = new Vector3((w / 2) - r - (d - curve), y, h / 2);
            }
            else
            {
                Debug.Log("UHMMMM well this shouldn't have happened lol but rounded rectangle problem");
                // point = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                point = Vector3.zero;
            }
            if (xFlip) { point.x *= -1; }
            if (yFlip) { point.z *= -1; }
            point += (Vector3.right * x) + (Vector3.forward * z) + Vector3.forward * y;
            return point;
        }
    
    public float getPerimeter() { return p; }
}
