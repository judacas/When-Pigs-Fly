using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public BoxCollider[] colliders;
    Variables settings;

    void Awake()
    {
        settings = GameObject.Find("Controller").GetComponent<Variables>();
        colliders = GetComponents<BoxCollider>();
        float rad = settings.Tree.GetComponent<SphereCollider>().radius;
        foreach (BoxCollider col in colliders)
        {
            float perimeter = col.bounds.size.x * 2 + col.bounds.size.z * 2;
            int numTrees = (int) (perimeter / rad);
            float dist = perimeter/numTrees;
            for(int i = 0; i < numTrees; i++){
                
            }
        }
    }
}


