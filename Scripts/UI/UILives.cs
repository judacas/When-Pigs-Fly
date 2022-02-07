using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILives : MonoBehaviour
{
    // Start is called before the first frame update
    int[] frameCount;
    float[] frameLength;
    int frame = 0;
    float timeInFrame;

    public int lives;
    void Start()
    {
        frameCount = new int[] { 8, 2, 4 };
        frameLength = new float[] { 0.1f, 0.2f, 0.2f };
    }

    // Update is called once per frame
    void Update()
    {

        for (int x = 0; x < 3; x++) //x is heart count
        {
            if (x + 1 == lives)
            {
                transform.GetChild(x).gameObject.SetActive(true);
                timeInFrame += Time.deltaTime;
                if (timeInFrame > frameLength[x]){
                    frame = (frame + 1) % (frameCount[x]);
                    timeInFrame = 0;
                }
                for (int y = 0; y < transform.GetChild(x).childCount; y++) //y is frame count
                {
                    transform.GetChild(x).GetChild(y).gameObject.SetActive(y == frame);
                }
            }
            else
            {
                transform.GetChild(x).gameObject.SetActive(false);
            }
        }
    }

}