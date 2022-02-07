using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// for future when I have time make a method in variables that will find closest and parameters you give it the list
//no use in rewriting the code for carrots and pigs and the multiple times it needs to be done in the future
public class pigController : MonoBehaviour
{

    float launchedTime, launchDist, launchHeight, timePicked, xNeeded, xCurrent, targetDist, targetDir;
    Vector3 moveTo, moveFrom, velocity = Vector3.zero, launchFrom, launchDirection;
    GameObject activeCarrot = null, scale = null;
    static Variables settings;

    private Animator anim;
    public static GameObject[] predictors = null;
    public enum PigState
    {
        still,
        carroting,
        kicked,
        launching,
        pickedUp,
        onCatapult
    }
    PigState state = PigState.still;

    void Awake()
    {
        settings = GameObject.Find("Controller").GetComponent<Variables>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case PigState.still:
                anim.SetBool("isMoving", false);
                transform.position += Physics.checkCircles(transform.position, Vector3.zero, settings.PIG_RAD);
                // scale = Physics.checkCatapult(gameObject, transform.position + velocity);
                // if (scale != null)
                // {
                //     // makes sure that the scale is down
                //     if (scale.transform.parent.parent.GetComponent<launcher>().currentState == (launcher.State)int.Parse(scale.name))
                //     {
                //         Vector3 posTest = scale.transform.TransformPoint(scale.GetComponent<SphereCollider>().center);
                //         Vector3 playerPos = gameObject.transform.TransformPoint(gameObject.GetComponent<SphereCollider>().center);
                //         posTest.y = 0;
                //         playerPos.y = 0;
                //         float totalRadii = gameObject.GetComponent<SphereCollider>().radius * gameObject.transform.localScale.x + scale.GetComponent<SphereCollider>().radius * scale.transform.localScale.x;
                //         Vector3 sOffset =   playerPos - posTest;
                //         sOffset.y = 0;
                //         if ((int)(scale.transform.parent.parent.GetComponent<launcher>().currentState) * int.Parse(scale.name) != -1)
                //         {
                //             sOffset = sOffset.normalized * totalRadii;
                //             Vector3 newVel = posTest + sOffset - playerPos;
                //             newVel.y = 0;
                //             transform.position += newVel;
                //         }
                //     }
                // }
                // scale = null;
                break;
            case PigState.carroting:
                anim.SetBool("isMoving", true);
                velocity = Physics.moveTo(moveFrom, moveTo, transform.position, settings.PIG_SPEED_UP, settings.PIG_SLOW_DOWN, settings.PIG_MAX_VEL, 1.1f * settings.PIG_MIN_VEL);
                scale = Physics.checkCatapult(gameObject, transform.position + velocity);
                if (scale != null)
                {
                    // makes sure that the scale is down
                    if (scale.transform.parent.parent.GetComponent<launcher>().currentState == (launcher.State)int.Parse(scale.name))
                    {
                        // Debug.Log(Vector3.Dot((moveTo - transform.position).normalized, (scale.transform.position - transform.position).normalized));
                        if (Vector3.Dot((moveTo - transform.position).normalized, (scale.transform.position - transform.position).normalized) > 0f)
                        {
                            loseCarrot();
                            claimScale(scale);
                            return;
                        }
                    }
                }
                Vector3 offset = (moveTo - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90 - Mathf.Rad2Deg * Mathf.Atan2(offset.z, offset.x), 0), Time.deltaTime * settings.PIG_TURN_DAMP);
                //this is before correcting velocity due to obstacles, thus if the if is true that means the pig reached the carrot
                if (velocity.magnitude < settings.PIG_MIN_VEL)
                {
                    activeCarrot.GetComponent<Carrot>().eat();
                    activeCarrot = null;
                    state = PigState.still;
                }
                velocity = Physics.checkCircles(transform.position, velocity, settings.PL_RAD);
                if (velocity.magnitude < settings.PIG_MIN_VEL)
                {
                    velocity = Vector3.zero;
                    loseCarrot();
                }
                transform.position += velocity;
                break;
            case PigState.kicked:
                anim.SetBool("isMoving", true);
                velocity = Physics.moveTo(moveFrom, moveTo, transform.position, settings.PIG_SPEED_UP / 5, settings.PIG_SLOW_DOWN, settings.PIG_MAX_VEL * 2, 1.1f * settings.PIG_MIN_VEL);
                Vector3 Offset = (moveTo - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90 - Mathf.Rad2Deg * Mathf.Atan2(Offset.z, Offset.x), 0), Time.deltaTime * settings.PIG_TURN_DAMP);
                if (velocity.magnitude < settings.PIG_MIN_VEL)
                {
                    state = PigState.still;
                }
                transform.position += velocity;
                break;
            case PigState.launching:
                anim.SetBool("isMoving", true);
                xCurrent = launchDist * ((Time.time - launchedTime) / settings.PIG_LAUNCH_TIME);
                // Debug.Log("X Current " + xCurrent);
                transform.position = Physics.getPosition(launchFrom, launchDirection, launchDist, launchHeight, xCurrent);
                if (xCurrent > xNeeded)
                {
                    transform.position = Physics.getPosition(launchFrom, launchDirection, launchDist, launchHeight, xNeeded);
                    if (scale == null)
                    {
                        state = PigState.still;
                        transform.parent = settings.pigs;
                    }
                    else
                    {
                        claimScale(scale);
                    }
                }
                break;
            case PigState.pickedUp:
                anim.SetBool("isMoving", false);
                setUpLaunch(transform.forward, settings.PIG_LAUNCH_DIST, settings.PIG_LAUNCH_HEIGHT);
                settings.predictors.GetComponent<Predictor>().drawPrediction(launchFrom, launchDirection, launchDist, settings.PIG_LAUNCH_HEIGHT, timePicked, xNeeded);
                break;
            case PigState.onCatapult:
                anim.SetBool("isMoving", false);
                Vector3 catDir = transform.parent.parent.parent.transform.right * (int)(transform.parent.parent.parent.GetComponent<launcher>().currentState);
                // setUpLaunch(catDir, settings.CAT_LAUNCH_DIST, settings.CAT_LAUNCH_HEIGHT);
                // transform.parent.parent.parent.GetChild(0).GetComponent<Predictor>().drawPrediction(transform.position, launchDirection, launchDist, settings.CAT_LAUNCH_HEIGHT, timePicked, xNeeded);
                break;
        }
    }

    public void whistled()
    {
        if (state == PigState.still)
        {
            Transform closest = null;
            // use count
            float minDistance = float.MaxValue;
            foreach (Transform test in settings.carrots)
            {
                if (test.gameObject.GetComponent<Carrot>().active)
                {
                    Vector3 offset = test.transform.position - transform.position;
                    if (Vector3.Angle(transform.forward, offset) < settings.PIG_FOV)
                    {
                        if (closest == null)
                        {
                            closest = test;
                            minDistance = offset.magnitude;
                        }
                        else if (offset.magnitude < minDistance)
                        {
                            closest = test;
                            minDistance = offset.magnitude;
                        }
                    }
                }
            }
            if (closest != null)
            {
                closest.GetComponent<Carrot>().claim(gameObject);
            }
        }
    }

    public void found(GameObject carrot)
    {
        state = PigState.carroting;
        moveTo = carrot.transform.position;
        moveFrom = transform.position;
        activeCarrot = carrot;
        scale = null;

    }

    public void loseCarrot()
    {
        state = PigState.still;
        // occasional error where it says active carrot is already null but idk why and it doesn't actually break anything so...
        if (activeCarrot != null)
        {
            activeCarrot.GetComponent<Carrot>().claimer = null;
        }
        activeCarrot = null;

    }

    public void pickUp()
    {
        // transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        state = PigState.pickedUp;
        transform.SetParent(settings.PLAYER);
        transform.localPosition = new Vector3(0, 0, settings.PIG_PICKED_DIST);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        timePicked = Time.time;
        settings.predictors.GetComponent<Predictor>().working(true);
    }

    public void launch(Vector3 dir, float dist, float height)
    {
        launchHeight = height;
        state = PigState.launching;
        setUpLaunch(dir, dist, height);
        transform.SetParent(settings.transform);
        launchedTime = Time.time;
        settings.predictors.GetComponent<Predictor>().working(false);

    }

    public void setUpLaunch(Vector3 dir, float dist, float height)
    {
        launchDirection = dir;
        launchFrom = transform.position;
        launchDist = dist;
        launchHeight = height;
        Vector3 predictedLand = launchFrom + launchDirection.normalized * dist;
        scale = null;
        // potential way to save proccessing power is if it was snapped to a scale before then check only that scale first
        // most likely it will stay on that scale, if it doesn't snap to it, then it won't snap to anything else
        // this is because no two scales are that close that with one frame it will go from one scale to the next
        // and in the case that it somehow is, oh well it will just not snap for one frame whatever that shouldn't happen anyways
        // now if it was snapped that is when I can check for all scales
        for (int i = 0; i < settings.PIG_CAT_ACC; i++)
        {
            scale = Physics.checkCatapult(gameObject, Physics.getPosition(launchFrom, launchDirection, launchDist, height, ((i / settings.PIG_CAT_ACC) * launchDist / 2f) + launchDist / 2f));
            if (scale != null)
            {
                Vector3 pos = scale.transform.TransformPoint(scale.GetComponent<SphereCollider>().center);
                if (pos.y - launchFrom.y < height)
                {
                    xNeeded = Physics.invGetPosition(launchDist, height, pos.y);
                    // Debug.Log("X Needed " + xNeeded);
                    Vector3 offset = pos - (launchFrom + launchDirection * xNeeded);
                    offset.y = 0;
                    launchDist = ((launchDist * launchDirection) + offset).magnitude;
                    // Debug.Log(launchDist);
                    launchDirection = (pos - launchFrom);
                    launchDirection.y = 0;
                    launchDirection = launchDirection.normalized;
                    xNeeded = Physics.invGetPosition(launchDist, height, pos.y);
                    return;
                }
            }
        }
        Vector3 correction = Physics.checkCircles(predictedLand, Vector3.zero, settings.PIG_RAD);
        if (correction.magnitude != 0)
        {
            launchDirection = ((predictedLand + correction) - transform.position);
            launchDist = ((predictedLand + correction) - transform.position).magnitude;
        }
        xNeeded = launchDist;
    }


    public void claimScale(GameObject scale)
    {
        transform.position = scale.transform.position;
        state = PigState.onCatapult;
        transform.parent = scale.transform;
        scale.transform.parent.parent.GetComponent<launcher>().claim(gameObject, scale);
        timePicked = Time.time;
    }
    public void Kick()
    {
        state = PigState.kicked;
        Vector3 scalePos = transform.parent.TransformPoint(transform.parent.gameObject.GetComponent<SphereCollider>().center);
        moveFrom = scalePos;
        moveTo = scalePos + transform.forward * ((transform.parent.gameObject.GetComponent<SphereCollider>().radius * transform.parent.transform.localScale.x + GetComponent<SphereCollider>().radius) * 1.1f);
        moveTo.y = 0;
        transform.parent = settings.pigs;
    }
    public void catLaunch()
    {
        transform.parent = settings.pigs;
    }
    public void landed()
    {
        state = PigState.still;
        transform.parent = settings.pigs;
        scale = null;
    }
}
