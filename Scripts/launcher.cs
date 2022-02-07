using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launcher : MonoBehaviour
{
    public GameObject loaded = null, launching = null, scale = null, lScale, rScale;
    GameObject left = null, right = null;
    public enum State
    {
        Left = -1,
        Right = 1,
        None = 0,
    }
    public State currentState;
    Transform lever, activeScale;
    Variables settings;
    float angle, targetAngle, timeClaimed, timeLaunched, launchDist, launchHeight, xCurrent, pDist, pXNeeded, pHeight, moveDis, defDis;
    Vector3 pFrom, pDir, launchDir, launchStart, moveDir, center;
    // Start is called before the first frame update

    // Update is called once per frame
    void FixedUpdate()
    {
        checkPlayer();
        // Debug.Log("In left: " + (left != null));
        // Debug.Log("In right: " + (right != null));
        // GameObject loaded = null;
        if (left != null && right != null)
        {
            GameObject upScale;
            bool isRightUp;
            if (right.transform.position.y == left.transform.position.y)
            {
                // Debug.Log("equal");
                if (Physics.checkCircles(new Vector3(lScale.transform.position.x, 0, lScale.transform.position.z), Vector3.zero, lScale.GetComponent<SphereCollider>().radius * lScale.transform.localScale.x * 1.0f, left.transform) == Vector3.zero)
                {
                    isRightUp = true;
                }
                else
                {
                    isRightUp = false;
                }

            }
            else
            {
                isRightUp = right.transform.position.y > left.transform.position.y;
            }
            GameObject up = (isRightUp) ? right : left;
            loaded = (isRightUp) ? left : right;
            upScale = (isRightUp) ? rScale : lScale;
            targetAngle = (isRightUp) ? settings.CAT_MAX_ANGLE : -settings.CAT_MAX_ANGLE;
            // Debug.Log(Physics.checkCircles(new Vector3(upScale.transform.position.x, 0, upScale.transform.position.z), Vector3.zero, upScale.GetComponent<SphereCollider>().radius * upScale.transform.localScale.x * 1.0f, up.transform));
            if (Physics.checkCircles(new Vector3(upScale.transform.position.x, 0, upScale.transform.position.z), Vector3.zero, upScale.GetComponent<SphereCollider>().radius * upScale.transform.localScale.x * 1.0f, up.transform) == Vector3.zero)
            {
                // Debug.Log("free to go");
                launching = (isRightUp) ? left : right;
                launch();
                launchStart = launching.transform.position;
                timeLaunched = Time.time;
                if (launching == left)
                {
                    left = null;
                    currentState = State.Right;
                }
                else
                {
                    right = null;
                    currentState = State.Left;
                }

            }
            else
            {
                // Debug.Log("Ive been stopped");
                launching = null;
                targetAngle = (isRightUp) ? -settings.CAT_MAX_ANGLE : settings.CAT_MAX_ANGLE;
            }
        }
        else if (left == null && right == null)
        {
            currentState = State.None;
            targetAngle = 0;
            loaded = null;
        }
        else if (left != null)
        {
            if (Physics.checkCircles(new Vector3(lScale.transform.position.x, 0, lScale.transform.position.z), Vector3.zero, lScale.GetComponent<SphereCollider>().radius * lScale.transform.localScale.x * 1.0f, left.transform) == Vector3.zero)
            {
                loaded = left;
                targetAngle = -settings.CAT_MAX_ANGLE;
                currentState = State.Left;
            }
            else
            {
                targetAngle = 0;
                currentState = State.None;
            }
        }
        else if (right != null)
        {
            if (Physics.checkCircles(new Vector3(rScale.transform.position.x, 0, rScale.transform.position.z), Vector3.zero, rScale.GetComponent<SphereCollider>().radius * rScale.transform.localScale.x * 1.0f, right.transform) == Vector3.zero)
            {
                loaded = right;
                targetAngle = settings.CAT_MAX_ANGLE;
                currentState = State.Right;
            }
            else
            {
                targetAngle = 0;
                currentState = State.None;
            }
        }
        angle = Mathf.Lerp(angle, targetAngle, settings.CAT_ROTATE_SPEED);

        lever.rotation = Quaternion.Euler(0, transform.eulerAngles.y, angle);
        if (loaded != null)
        {
            // Debug.Log(loaded.name);
            setUpLaunch(loaded.transform.position, transform.right * (int)(currentState) * 1, defDis, settings.CAT_LAUNCH_HEIGHT);
            transform.GetChild(0).GetComponent<Predictor>().drawPrediction(pFrom, pDir, pDist, pHeight, timeClaimed, pXNeeded);
        }
        if (launching != null)
        {
            setUpLaunch(launchStart, transform.right * (int)(currentState) * -1, defDis, settings.CAT_LAUNCH_HEIGHT);
            xCurrent = pDist * ((Time.time - timeLaunched) / settings.PIG_LAUNCH_TIME);
            // Debug.Log("X Current " + xCurrent);
            launching.transform.position = Physics.getPosition(pFrom, pDir, pDist, pHeight, xCurrent);
            // temporary fix since y is negative for first frame, maybe it is because of launch from scale not from position
            // don't have time to code permanent fix now
            if (launching.transform.position.y < -0.1)
            {
                land();
            }
            else
            {
                if (xCurrent > pDist / 2)
                {
                    GameObject scaleInPath = Physics.checkCatapult(launching, launching.transform.position);
                    if (scaleInPath != null)
                    {
                        if (launching.transform.position.y < scaleInPath.transform.position.y + 0.2f)
                        {
                            _claim(scaleInPath);
                        }
                    }
                }
            }
        }
    }

    public void claim(GameObject obj, GameObject _scale)
    {
        activeScale = _scale.transform;
        timeClaimed = Time.time;
        State newState = (State)(int.Parse(_scale.name));
        if (_scale == lScale)
        {
            if (left != null)
            {
                kick(left, obj);
            }
            left = obj;
        }
        else if (_scale == rScale)
        {
            if (right != null)
            {
                kick(right, obj);
            }
            right = obj;
        }
        else
        {
            // Debug.Log("Im dumb and messed up :(");
        }
    }
    private void _claim(GameObject scaleInPath)
    {
        if (launching.GetComponent<pigController>() != null)
        {
            // Debug.Log("Pig fell on catapult");
            launching.GetComponent<pigController>().claimScale(scaleInPath);
        }
        else if (launching.GetComponent<Carrot>() != null)
        {
            // Debug.Log("Carrot fell on catapult");
            launching.GetComponent<Carrot>().claimScale(scaleInPath);
        }
        else if (launching.GetComponent<newMovement>() != null)
        {
            // Debug.Log("Carrot fell on catapult");
            launching.GetComponent<newMovement>().claimScale(scaleInPath);
        }
        launching = null;
    }
    public void unClaim(GameObject obj)
    {
        // Debug.Log("unclaiming");
        // Debug.Log(obj.name);
        currentState = State.None;
        if (obj == lScale)
        {
            // Debug.Log("leaving left");
            left = null;
        }
        else
        {
            // Debug.Log("leaving right");
            right = null;
        }
        loaded = null;
        targetAngle = 0;
        transform.GetChild(0).GetComponent<Predictor>().working(false);
    }

    // flaw with this desing is it cant launch a second object while first is still in its trajectory



    public void setUpLaunch(Vector3 launchFrom, Vector3 dir, float dist, float height)
    {
        pDir = dir;
        pDist = dist;
        pHeight = height;
        pFrom = launchFrom;
        Vector3 predictedLand = launchFrom + pDir.normalized * dist;
        scale = null;
        // potential way to save proccessing power is if it was snapped to a scale before then check only that scale first
        // most likely it will stay on that scale, if it doesn't snap to it, then it won't snap to anything else
        // this is because no two scales are that close that with one frame it will go from one scale to the next
        // and in the case that it somehow is, oh well it will just not snap for one frame whatever that shouldn't happen anyways
        // now if it was snapped that is when I can check for all scales
        for (int i = 0; i < settings.PIG_CAT_ACC; i++)
        {
            scale = Physics.checkCatapult(loaded, Physics.getPosition(launchFrom, pDir, pDist, height, ((i / settings.PIG_CAT_ACC) * pDist / 2f) + pDist / 2f));
            if (scale != null)
            {
                Vector3 pos = scale.transform.TransformPoint(scale.GetComponent<SphereCollider>().center);
                if (pos.y - launchFrom.y < height)
                {
                    pXNeeded = Physics.invGetPosition(pDist, height, pos.y);
                    // Debug.Log("X Needed " + pXNeeded);
                    Vector3 offset = pos - (launchFrom + pDir * pXNeeded);
                    offset.y = 0;
                    pDist = ((pDist * pDir) + offset).magnitude;
                    // Debug.Log(pDist);
                    pDir = (pos - launchFrom);
                    pDir.y = 0;
                    pDir = pDir.normalized;
                    pXNeeded = Physics.invGetPosition(pDist, height, pos.y);
                    return;
                }
            }
        }
        Vector3 correction = Physics.checkCircles(predictedLand, Vector3.zero, settings.PIG_RAD);
        if (correction.magnitude != 0)
        {
            pDir = ((predictedLand + correction) - launchFrom);
            pDist = ((predictedLand + correction) - launchFrom).magnitude;
        }
        pXNeeded = pDist;
    }

    void checkPlayer()
    {
        Vector3 offset = settings.PLAYER.position - transform.TransformPoint(gameObject.GetComponent<SphereCollider>().center);
        float totalRad = gameObject.GetComponent<SphereCollider>().radius * transform.localScale.x * 1.1f + settings.PLAYER.gameObject.GetComponent<SphereCollider>().radius * settings.PLAYER.localScale.x;
        if (offset.magnitude < totalRad)
        {
            if (settings.PLAYER.gameObject.GetComponent<newMovement>().canMoveCat)
            {
                offset -= (transform.position - transform.TransformPoint(gameObject.GetComponent<SphereCollider>().center));
                offset = offset.normalized * totalRad;
                if (moveDir == Vector3.forward)
                {
                    float old = transform.position.x;
                    transform.position = settings.PLAYER.position - offset;
                    transform.position = new Vector3(old, 0, transform.position.z);
                }
                else
                {
                    float old = transform.position.z;
                    transform.position = settings.PLAYER.position - offset;
                    transform.position = new Vector3(transform.position.x, 0, old);
                }
                if ((transform.position - center).magnitude > moveDis / 2f)
                {
                    transform.position = center + (transform.position - center).normalized * (moveDis / 2f);
                }
            }
        }
    }
    public void launch()
    {
        launching = loaded;
        launchStart = loaded.transform.position;
        timeLaunched = Time.time;
        transform.GetChild(0).GetComponent<Predictor>().working(false);
        if (loaded.GetComponent<pigController>() != null)
        {
            // loaded.GetComponent<pigController>().launch();
        }
        else if (loaded.GetComponent<Carrot>() != null)
        {
            loaded.GetComponent<Carrot>().launch();
        }
        else if (loaded.GetComponent<newMovement>() != null)
        {
            loaded.GetComponent<newMovement>().launch();
        }

    }

    void kick(GameObject obj, GameObject possiblePig)
    {
        if (obj.GetComponent<pigController>() != null)
        {
            obj.GetComponent<pigController>().Kick();
        }
        else if (obj.GetComponent<Carrot>() != null)
        {
            if (possiblePig.GetComponent<pigController>() != null)
            {
                obj.GetComponent<Carrot>().eat();
            }
            else
            {
                obj.GetComponent<Carrot>().kick();
            }
        }
        else if (obj.GetComponent<newMovement>() != null)
        {
            obj.GetComponent<newMovement>().kick();
        }
    }

    public void land()
    {
        // change to the invposition thing to find where it should land
        launching.transform.position = new Vector3(launching.transform.position.x, 0, launching.transform.position.z);
        if (launching.GetComponent<pigController>() != null)
        {
            launching.GetComponent<pigController>().landed();
        }
        else if (launching.GetComponent<Carrot>() != null)
        {
            launching.GetComponent<Carrot>().landed();
        }
        else if (launching.GetComponent<newMovement>() != null)
        {
            launching.GetComponent<newMovement>().landed();
        }
        launching = null;
    }


    public void setUp(Vector3 moveDirection, float moveDistance, Vector3 catCenter, float strengthOffset)
    {
        settings = GameObject.Find("Controller").GetComponent<Variables>();
        defDis = settings.CAT_LAUNCH_DIST + strengthOffset;
        center = catCenter;
        moveDir = moveDirection;
        moveDis = moveDistance;
        currentState = State.None;
        lever = transform.Find("Lever");
        lever.rotation = Quaternion.Euler(Vector3.zero);
        loaded = null;
        lScale = transform.GetChild(transform.childCount - 1).GetChild(0).gameObject;
        rScale = transform.GetChild(transform.childCount - 1).GetChild(1).gameObject;
        // Debug.Log(lScale.name);
        // Debug.Log(rScale.name);
        transform.GetChild(0).GetComponent<Predictor>().setUp(settings.CAT_PRED_ACC);

    }
}
