using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    Variables settings;
    public GameObject claimer = null, scale = null;
    public bool active = true;
    private Animator anim;
    void Awake()
    {
        active = true;
        settings = GameObject.Find("Controller").GetComponent<Variables>();
        anim = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        anim.SetBool("isActive", active);
        if (scale != null)
        {
            transform.position = scale.transform.position;
        }
    }

    public void pickUp()
    {
        active = false;
        if(scale != null){
            scale.transform.parent.parent.GetComponent<launcher>().unClaim(scale);
            kick();
        }
        transform.SetParent(settings.HAND.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 45);
    }

    public void drop()
    {
        active = true;
        transform.SetParent(settings.carrots.transform);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.position = settings.PLAYER.transform.position;
        scale = Physics.checkCatapult(gameObject, transform.position);
        if (scale != null)
        {
            if (scale.transform.parent.parent.GetComponent<launcher>().currentState == (launcher.State)int.Parse(scale.name))
            {
                claimScale(scale);
                // scale.transform.parent.parent.GetComponent<launcher>().claim(gameObject, scale.name);
            }
            else
            {
                scale = null;
            }
        }
    }

    public void claimScale(GameObject _scale)
    {
        // Debug.Log("carrot claiming scale");
        scale = _scale;
        scale.transform.parent.parent.GetComponent<launcher>().claim(gameObject, _scale);
        active = true;
    }

    public void claim(GameObject pig)
    {
        if (claimer == null || (pig.transform.position - transform.position).magnitude < (claimer.transform.position - transform.position).magnitude)
        {
            pig.GetComponent<pigController>().found(gameObject);
            if (claimer != null)
            {
                claimer.GetComponent<pigController>().loseCarrot();
            }
            claimer = pig;

        }
    }

    public void kick()
    {
        Vector3 scalePos = scale.transform.TransformPoint(scale.GetComponent<SphereCollider>().center);
        transform.position = scalePos + transform.forward * (scale.GetComponent<SphereCollider>().radius * scale.transform.localScale.x + GetComponent<SphereCollider>().radius * transform.localScale.x);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        scale = null;
    }
    public void launch()
    {
        scale = null;
        active = false;
    }
    public void landed(){
        active = true;
    }

    public void eat()
    {
        Destroy(gameObject);
    }
}
