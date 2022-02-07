using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTarg : MonoBehaviour
{
    Transform player;
    newMovement movement;
    public float lookAhead, targetDist;
    Vector3 target;
    Variables settings;

    InputAction zoomOut;
    GameObject blur;
    public float dampening, currentDist;
    bool oldZoomOut, zoomOutPressed;
    // Start is called before the first frame update
    void Awake()
    {
        settings = GameObject.Find("Controller").GetComponent<Variables>();
        targetDist = settings.CAM_DIST;
        blur = GameObject.Find("Box Volume");
        zoomOut = new InputAction("zoomout", binding: "<Keyboard>/alt");
        zoomOut.Enable();
        player = settings.PLAYER;
        movement = player.GetComponent<newMovement>();
        transform.position = player.position;
        target = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        zoomOutPressed = Mathf.Approximately(zoomOut.ReadValue<float>(), 1);
        if (zoomOutPressed != oldZoomOut)
        {
            oldZoomOut = zoomOutPressed;
            if(zoomOutPressed){
                targetDist = settings.CAM_ZOOM_DIST;
            }
            else{
                targetDist = settings.CAM_DIST;
            }
        }
        currentDist = Mathf.Lerp(currentDist, targetDist, settings.CAM_ZOOM_DAMP);
        target = player.position + movement.finalVelocity * lookAhead;
        transform.position = (transform.position + (target - transform.position) / dampening);
        updateCam();
    }

    void updateCam()
    {
        settings.CAM.transform.position = transform.position + new Vector3(0, Mathf.Sin(settings.CAM_ANGLE * Mathf.Deg2Rad), -Mathf.Cos(settings.CAM_ANGLE * Mathf.Deg2Rad)) * currentDist;
        settings.CAM.transform.rotation = Quaternion.Euler(new Vector3(settings.CAM_ANGLE + settings.CAM_OFFSET_ANGLE, 0, 0));
    }


}
