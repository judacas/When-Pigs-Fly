using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predictor : MonoBehaviour
{
    Variables settings;
    // Start is called before the first frame update
    void Awake()
    {
        settings = GameObject.Find("Controller").GetComponent<Variables>();
    }

    // Update is called once per frame
    void Fixed()
    {

    }

    public void setUp(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Instantiate(settings.Predictor, transform).GetComponent<MeshRenderer>().enabled = false;
        }

    }
    public void drawPrediction(Vector3 launchFrom, Vector3 launchDirection, float dist, float height, float timePicked, float xMax)
    {
        float offset = ((Time.time * (settings.PIGPREDSPEED * dist)) - timePicked) % ((1f / (transform.childCount)) * dist);
        // % ((1f/(settings.PIGPREDACC-1f)) * dist);
        // Debug.Log(launchDirection);
        // Debug.Log("a " + dist);
        // Debug.Log("aa " + height);
        // Debug.Log("aaa " +xMax);
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = Physics.getPosition(launchFrom, launchDirection, dist, height, (((i - 1f) / transform.childCount) * dist) + offset);
            transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled = ((((i - 1f) / transform.childCount) * dist) + offset < xMax);
        }
        // should havea variable for how far up to go to be visible but this is temporary fix anyways
        // ik there is a way to make this always cull in front of everything else
        // don't know how to do that rn but don't have time to figure it out
        transform.GetChild(0).position = Physics.getPosition(launchFrom, launchDirection, dist, height, xMax) + new Vector3(0f, 0.05f, 0f);
        if (transform.GetChild(0).position.y < 0.05f)
        {
            transform.GetChild(0).position = new Vector3(transform.GetChild(0).position.x, 0.05f, transform.GetChild(0).position.z);
        }
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;

    }

    public void working(bool work)
    {
        foreach (Transform current in transform)
        {
            current.gameObject.GetComponent<MeshRenderer>().enabled = work;
        }
    }
}
