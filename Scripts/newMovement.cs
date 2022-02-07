using UnityEngine.InputSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newMovement : MonoBehaviour
{

    public Vector3 finalVelocity = Vector3.zero;
    // [SerializeField]
    Vector2 inputVel;
    // [SerializeField]
    float targetDirection;
    bool whistlePressed = false, oldWhistle = false, grabPressed = false, oldGrab = false, carrotPressed = false, oldCarrot, catPressed = false, oldCatPressed = false, catNow = false;
    public bool canMoveCat = false;
    Transform grabbed = null, currentCarrot = null;
    InputAction movement, whistle, grab, carrot, cat, catMove;
    GameObject scale = null;

    Animator anim;

    static Variables settings;



    public void Awake()
    {
        settings = GameObject.Find("Controller").GetComponent<Variables>();
        anim = GetComponent<Animator>();
        setUpControls();

    }
    public void FixedUpdate()
    {
        //able to move catapults
        canMoveCat = Mathf.Approximately(catMove.ReadValue<float>(), 1);

        //whistling
        whistlePressed = Mathf.Approximately(whistle.ReadValue<float>(), 1);
        if (whistlePressed != oldWhistle)
        {
            oldWhistle = whistlePressed;
            if (whistlePressed)
            {
                Whistle();
            }
        }

        //picking up and launching pigs
        grabPressed = Mathf.Approximately(grab.ReadValue<float>(), 1);
        if (grabPressed != oldGrab)
        {
            oldGrab = grabPressed;
            if (grabPressed)
            {
                if (grabbed == null)
                {
                    pickUp();
                }
                else
                {
                    letGo();
                }
            }

        }

        //picking up and dropping Carrot
        carrotPressed = Mathf.Approximately(carrot.ReadValue<float>(), 1);
        if (carrotPressed != oldCarrot)
        {
            oldCarrot = carrotPressed;
            if (carrotPressed)
            {
                if (currentCarrot == null)
                {
                    findCarrot();
                }
                else
                {
                    dropCarrot();
                }
            }

        }


        // movement
        inputVel = movement.ReadValue<Vector2>();
        if (inputVel.magnitude != 0)
        {
            targetDirection = 90 - Mathf.Rad2Deg * Mathf.Atan2(inputVel.y, inputVel.x);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetDirection, 0), Time.deltaTime * settings.PL_TURN_DAMP);

        // all things catapults

        catPressed = Mathf.Approximately(cat.ReadValue<float>(), 1);
        catNow = false;
        if (catPressed != oldCatPressed)
        {
            oldCatPressed = catPressed;
            if (catPressed)
            {
                catNow = true;
            }
        }
        if (scale == null)
        {
            if (catNow)
            {
                float standardRad = GetComponent<SphereCollider>().radius;
                GetComponent<SphereCollider>().radius = settings.PL_CAT_PICK_DIST;
                scale = Physics.checkCatapult(gameObject, transform.position + finalVelocity);
                GetComponent<SphereCollider>().radius = standardRad;
                if (scale != null)
                {
                    // makes sure that the scale is down
                    if (scale.transform.parent.parent.GetComponent<launcher>().currentState == (launcher.State)int.Parse(scale.name))
                    {
                        claimScale(scale);
                    }
                    else
                    {
                        scale = null;
                    }
                }
            }
            else
            {
                scale = Physics.checkCatapult(gameObject, transform.position + finalVelocity);
                if (scale != null)
                {
                    Vector3 posTest = scale.transform.TransformPoint(scale.GetComponent<SphereCollider>().center);
                    Vector3 playerPos = gameObject.transform.TransformPoint(gameObject.GetComponent<SphereCollider>().center);
                    posTest.y = 0;
                    playerPos.y = 0;
                    float totalRadii = gameObject.GetComponent<SphereCollider>().radius * gameObject.transform.localScale.x + scale.GetComponent<SphereCollider>().radius * scale.transform.localScale.x;
                    Vector3 offset = (playerPos + finalVelocity) - posTest;
                    offset.y = 0;
                    if (offset.magnitude < totalRadii)
                    {
                        if ((int)(scale.transform.parent.parent.GetComponent<launcher>().currentState) * int.Parse(scale.name) != -1)
                        {
                            offset = offset.normalized * totalRadii;
                            Vector3 newVel = posTest + offset - playerPos;
                            finalVelocity.x = newVel.x;
                            finalVelocity.z = newVel.z;
                        }
                    }
                    scale = null;
                }
            }
        }
        else if (catNow)
        {
            scale.transform.parent.parent.GetComponent<launcher>().unClaim(scale);
            kick();

        }

        if (scale == null)
        {
            finalVelocity = Physics.move(inputVel, finalVelocity, settings.PL_SPEED_UP, settings.PL_SLOW_DOWN, settings.PL_MAX_VEL);
            finalVelocity = Physics.checkCircles(transform.position, finalVelocity, settings.PL_RAD);
            anim.SetBool("isMoving", finalVelocity.magnitude > 0.01);
            transform.position += finalVelocity;
        }
        else
        {
            finalVelocity = Vector3.zero;
        }
        anim.SetBool("isMoving", finalVelocity.magnitude > 0.01);
    }
    public void kick()
    {
        transform.position = scale.transform.TransformPoint(scale.GetComponent<SphereCollider>().center) + transform.forward * scale.GetComponent<SphereCollider>().radius;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        transform.parent = settings.currentController;
        scale = null;
    }

    private void Whistle()
    {
        foreach (Transform pig in settings.pigs)
        {
            pig.GetComponent<pigController>().whistled();
        }
    }

    void pickUp()
    {
        if (settings.pigs.childCount > 0)
        {
            Transform closest = settings.pigs.GetChild(0);
            float minDistance = (closest.position - transform.position).magnitude;
            foreach (Transform pig in settings.pigs)
            {
                if ((pig.position - transform.position).magnitude < minDistance)
                {
                    closest = pig;
                    minDistance = (closest.transform.position - transform.position).magnitude;
                }
            }
            if (minDistance < settings.PL_PIG_GRAB_DIST)
            {
                anim.SetTrigger("PickUp");
                grabbed = closest;
                grabbed.GetComponent<pigController>().pickUp();
            }
        }
    }

    void letGo()
    {
        anim.SetTrigger("Throw");
        grabbed.GetComponent<pigController>().launch(transform.forward, settings.PIG_LAUNCH_DIST, settings.PIG_LAUNCH_HEIGHT);
        grabbed = null;
    }

    void findCarrot()
    {
        if (settings.carrots.childCount > 0)
        {
            float min = float.MaxValue;
            Transform closest = settings.carrots.GetChild(0);
            foreach (Transform carrot in settings.carrots)
            {
                if ((carrot.transform.position - transform.position).magnitude < min)
                {
                    closest = carrot;
                    min = (closest.transform.position - transform.position).magnitude;
                }
            }

            if (min < settings.PL_PIG_GRAB_DIST)
            {
                anim.SetTrigger("PickUp");
                currentCarrot = closest;
                closest.GetComponent<Carrot>().pickUp();
            }
        }
    }
    void dropCarrot()
    {
        currentCarrot.GetComponent<Carrot>().drop();
        currentCarrot = null;
        anim.SetTrigger("Throw");
    }

    public void launch()
    {
        anim.SetTrigger("Fly");
        transform.parent = settings.currentController;
        scale = null;
    }

    public void landed()
    {
        anim.SetTrigger("Land");
    }

    public void claimScale(GameObject _scale)
    {
        anim.SetTrigger("Land");
        scale = _scale;
        transform.position = scale.transform.position;
        scale.transform.parent.parent.GetComponent<launcher>().claim(gameObject, scale);
        transform.parent = scale.transform;
    }


    void setUpControls()
    {
        //set up keybinds for controls
        movement = new InputAction("PlayerMovement", binding: "<Gamepad>/leftStick");
        movement.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");


        carrot = new InputAction("Carrot", binding: "<Mouse>/leftButton");

        whistle = new InputAction("Player whistle", binding: "<Mouse>/rightButton");

        grab = new InputAction("Grab", binding: "<Keyboard>/space");

        cat = new InputAction("cat", binding: "<Keyboard>/e");

        catMove = new InputAction("catMove", binding: "<Keyboard>/shift");

        movement.Enable();
        carrot.Enable();
        whistle.Enable();
        grab.Enable();
        cat.Enable();
        catMove.Enable();
    }
}



