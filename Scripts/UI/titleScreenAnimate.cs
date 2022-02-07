using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class titleScreenAnimate : MonoBehaviour
{
    // Start is called before the first frame update
    int frameCount;
    float frameLength;
    int frame=0;

    float timeInFrame;

    void Start()
    {
        frameCount = transform.childCount;
        frameLength = 0.08f;    
    }

    // Update is called once per frame
    void Update()
    {
        timeInFrame += Time.deltaTime;

        if (timeInFrame > frameLength){
            frame = (frame + 1) % (frameCount);
            timeInFrame = 0;
        }
        //Debug.Log("TitleScreen Frame:" + frame);
        for (int y = 0; y < frameCount; y++) //y is frame count
        {
            transform.GetChild(y).gameObject.SetActive(y == frame);
        }
    }
}
