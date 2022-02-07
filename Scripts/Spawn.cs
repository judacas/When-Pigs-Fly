using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    Variables settings;
    public enum SpawnType
    {
        Pig,
        Carrot,
        Catapult,
        Tree,
        TreeSection
    }
    public enum Orientation
    {
        Horizontal,
        Vertical,
    }

    public SpawnType type;

    [Header("If its a catapult then extra settings")]
    public float Length, strengthOffset;
    public Orientation catapultDirection;
    public Orientation railDirection;

    [Header("If its a tree section then extra settings")]
    public float width, height, radius;
    Quaternion catDir;
    Vector3 railDir;
    // Start is called before the first frame update
    void Awake()
    {
        settings = GameObject.Find("Controller").GetComponent<Variables>();
    }

    public void Generate()
    {
        switch (type)
        {
            // AYO AM I DUMB???? Can't I just do transform.parent and leave it at that????? I'll check later 
            case SpawnType.Pig:
                Instantiate(settings.Pig, transform.position, transform.rotation, transform.parent.parent.parent.Find("Pigs"));
                break;
            case SpawnType.Carrot:
                Instantiate(settings.Carrot, transform.position, transform.rotation, transform.parent.parent.parent.Find("Carrots"));
                break;
            case SpawnType.Tree:
                Instantiate(settings.Tree, transform.position, Quaternion.Euler(-90, Random.Range(0, 359), 0), transform.parent.parent.parent.Find("Trees"));
                break;
            case SpawnType.TreeSection:
                treeSectionSpawn();
                break;
            case SpawnType.Catapult:
                catSpawn();

                break;
        }
    }

    public void catSpawn()
    {
        switch (catapultDirection)
        {
            case Orientation.Horizontal:
                catDir = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case Orientation.Vertical:
                catDir = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
        }
        switch (railDirection)
        {
            case Orientation.Horizontal:
                railDir = Vector3.right;
                break;
            case Orientation.Vertical:
                railDir = Vector3.forward;
                break;
        }
        Instantiate(settings.Catapult, transform.position, catDir, transform.parent.parent.parent.Find("Catapults")).GetComponent<launcher>().setUp(railDir, Length, transform.position, strengthOffset);
        float railLength = settings.Rail.GetComponent<BoxCollider>().size.x / 2;
        for (int i = 0; i < (Length / railLength); i++)
        {
            Instantiate(settings.Rail, transform.position - ((float)(i - (((Length / railLength)) / 2f)) * railDir), Quaternion.Euler(new Vector3(90, 0, 0)), transform.parent.parent.parent.Find("Rails"));
        }
    }
    public void treeSectionSpawn()
    {
        RoundedRectangle rect = new RoundedRectangle(transform.position.x, transform.position.z, transform.position.y, width, height, radius);
        float perimeter = rect.getPerimeter();
        float increment = (settings.Tree.GetComponent<SphereCollider>().radius * settings.Tree.transform.localScale.x) * 2;
        Debug.Log(settings.Tree.GetComponent<SphereCollider>().radius * settings.Tree.transform.localScale.x);
        for (float i = 0; i < perimeter; i += increment)
        {
            Instantiate(settings.Tree, rect.getPoint(i / perimeter), Quaternion.Euler(-90, Random.Range(0, 359), 0), transform.parent.parent.parent.Find("Trees"));
        }

    }
    private void OnDrawGizmos()
    {
        if(settings == null){
            settings = GameObject.Find("Controller").GetComponent<Variables>();
        }
        if (type!= null && type == SpawnType.TreeSection)
        {
            RoundedRectangle rect = new RoundedRectangle(transform.position.x, transform.position.z, transform.position.y, width, height, radius);
            float perimeter = rect.getPerimeter();
            float increment = (settings.Tree.GetComponent<SphereCollider>().radius * settings.Tree.transform.localScale.x) * 2;
            for (float i = 0; i < perimeter; i += increment)
            {
                Gizmos.DrawSphere(rect.getPoint(i / perimeter), 1);
            }
        }
    }
}
