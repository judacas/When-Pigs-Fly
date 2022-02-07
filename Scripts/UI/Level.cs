using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    // Start is called before the first frame update
    public int level;
    public int totalLevels;
    void Start()
    {
        level = 1;
        totalLevels=transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 1; i<=totalLevels; i++)
        {
            if(i == level)
            {
                transform.GetChild(i-1).gameObject.SetActive(true);
            }else
            {
                transform.GetChild(i-1).gameObject.SetActive(false);
            }
        }
    }
}
