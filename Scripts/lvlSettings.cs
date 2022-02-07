using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvlSettings : MonoBehaviour
{
    public float PIG_LAUNCH_DIST;
    public float PIG_LAUNCH_HEIGHT;
    public float PIG_LAUNCH_TIME;
    public float CAT_LAUNCH_DIST;
    public float CAT_LAUNCH_HEIGHT;
    // Start is called before the first frame update
    public void changeSettings()
    {
        Variables settings = GameObject.Find("Controller").GetComponent<Variables>();
        if (PIG_LAUNCH_DIST != 0) { settings.PIG_LAUNCH_DIST = PIG_LAUNCH_DIST; }
        if (PIG_LAUNCH_HEIGHT != 0) { settings.PIG_LAUNCH_HEIGHT = PIG_LAUNCH_HEIGHT; }
        if (PIG_LAUNCH_TIME != 0) { settings.PIG_LAUNCH_TIME = PIG_LAUNCH_TIME; }
        if (CAT_LAUNCH_DIST != 0) { settings.CAT_LAUNCH_DIST = CAT_LAUNCH_DIST; }
        if (CAT_LAUNCH_HEIGHT != 0) { settings.CAT_LAUNCH_HEIGHT = CAT_LAUNCH_HEIGHT; }

    }
}
