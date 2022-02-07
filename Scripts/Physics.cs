using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics
{
    static Variables settings;



    // Since this is a purely static method class it will not be attached to any game object
    // Thus need this to initialize since Start() requires it to be attached to gameobject
    [RuntimeInitializeOnLoadMethod]
    static void initialize()
    {
        settings = GameObject.FindGameObjectsWithTag("Controller")[0].GetComponent<Variables>();
    }

    // can be used to check player or pigs against the pigs (or anything else that has spherical collider in future)
    //in comments we will pretend that it is player checking against all pigs
    public static Vector3 checkCircles(Vector3 initialPosition, Vector3 velocity, float rad)
    {
        float totalRadii;
        Vector3 offset;
        Vector3 position = initialPosition;
        Vector3 finalVel = velocity;

        foreach (Transform type in settings.circles)
        {

            foreach (Transform test in type)
            {
                Vector3 posTest = test.TransformPoint(test.gameObject.GetComponent<SphereCollider>().center);
                totalRadii = rad + test.gameObject.GetComponent<SphereCollider>().radius * test.localScale.x;
                offset = (position + finalVel) - posTest;

                if (offset.y > Mathf.Epsilon)
                {
                    offset.y = 0;
                }
                if (offset.magnitude < totalRadii && initialPosition != posTest)
                {
                    offset = offset.normalized * totalRadii;
                    finalVel = posTest + offset - initialPosition;
                    finalVel.y = 0;
                }
            }
        }
        return finalVel;
    }

    public static Vector3 checkCircles(Vector3 initialPosition, Vector3 velocity, float rad, Transform avoid)
    {
        float totalRadii;
        Vector3 offset;
        Vector3 position = initialPosition;
        Vector3 finalVel = velocity;

        foreach (Transform type in settings.circles)
        {

            foreach (Transform test in type)
            {
                if (test != avoid)
                {
                    Vector3 posTest = test.TransformPoint(test.gameObject.GetComponent<SphereCollider>().center);
                    totalRadii = rad + test.gameObject.GetComponent<SphereCollider>().radius * test.localScale.x;
                    offset = (position + finalVel) - posTest;

                    if (offset.y > Mathf.Epsilon)
                    {
                        offset.y = 0;
                    }
                    if (offset.magnitude < totalRadii && initialPosition != posTest)
                    {
                        offset = offset.normalized * totalRadii;
                        finalVel = posTest + offset - initialPosition;
                        finalVel.y = 0;
                    }
                }
            }
        }
        return finalVel;
    }


    public static Vector3 move(Vector2 newVel, Vector3 startingVel, float speedUp, float slowDown, float maxSpeed)
    {
        Vector3 finalVel = startingVel;
        if (newVel.x != 0)
        {
            if (Mathf.Abs(newVel.x) / newVel.x == Mathf.Abs(startingVel.x) / startingVel.x)
            {
                finalVel.x += newVel.x * (1 / speedUp) * Time.deltaTime;
            }
            else
            {
                finalVel.x += newVel.x * ((1 / speedUp) + (1 / slowDown)) * Time.deltaTime;
            }
        }
        else if (Mathf.Abs(startingVel.x) - (1 / slowDown) * Time.deltaTime > 0)
        {
            finalVel.x -= (startingVel.x / Mathf.Abs(startingVel.x)) * (1 / slowDown) * Time.deltaTime;
        }
        else
        {
            finalVel.x = 0;
        }

        if (newVel.y != 0)
        {
            if (Mathf.Abs(newVel.y) / newVel.y == Mathf.Abs(startingVel.z) / startingVel.z)
            {
                finalVel.z += newVel.y * (1 / speedUp) * Time.deltaTime;
            }
            else
            {
                finalVel.z += newVel.y * ((1 / speedUp) + (1 / slowDown)) * Time.deltaTime;
            }
        }
        else if (Mathf.Abs(startingVel.z) - (1 / slowDown) * Time.deltaTime > 0)
        {
            finalVel.z -= (startingVel.z / Mathf.Abs(startingVel.z)) * (1 / slowDown) * Time.deltaTime;
        }
        else
        {
            finalVel.z = 0;
        }

        float speed = finalVel.magnitude;
        if (speed > maxSpeed)
        {
            finalVel *= (maxSpeed / speed);
        }
        return finalVel;


    }

    //in this one speed up and slow down is the distance until max speed and stoping
    public static Vector3 moveTo(Vector3 start, Vector3 end, Vector3 current, float SpeedUp, float SlowDown, float maxSpeed, float minSpeed)
    {
        float speedUp = SpeedUp, slowDown = SlowDown;
        if ((end - start).magnitude < speedUp + slowDown)
        {
            float factor = (end - start).magnitude / (speedUp + slowDown);
            speedUp *= factor;
            slowDown *= factor;
        }

        if ((current - start).magnitude < speedUp)
        {
            // Debug.Log("speeding up");
            return ((end - current).normalized * Mathf.Lerp(minSpeed, maxSpeed, (current - start).magnitude / speedUp));
        }
        else if ((current - end).magnitude > slowDown)
        {
            // Debug.Log("max speed");
            return ((end - current).normalized * maxSpeed);
        }
        else
        {
            // Debug.Log("slowing down");
            return ((end - current).normalized * Mathf.Lerp(0, maxSpeed, (end - current).magnitude / slowDown));
        }

    }

    public static GameObject checkCatapult(GameObject obj, Vector3 pos)
    {
        foreach (Transform catapult in settings.catapults)
        {
            foreach (Transform scale in catapult.GetChild(catapult.childCount - 1))
            {
                Vector3 posTest = (obj.transform.TransformPoint(obj.GetComponent<SphereCollider>().center) - obj.transform.position) + pos;
                Vector3 scalePos = scale.transform.TransformPoint(scale.gameObject.GetComponent<SphereCollider>().center);
                float rad = obj.GetComponent<SphereCollider>().radius * obj.transform.localScale.x + scale.gameObject.GetComponent<SphereCollider>().radius * scale.localScale.x;
                Vector3 offset = (scalePos) - posTest;
                offset.y = 0;

                if (offset.magnitude < rad)
                {
                    return scale.gameObject;
                }
            }

        }
        return null;
    }


    // using some math you can make a quadratic formula where the first zero is at the origin
    // and the second zero is some given distance away and the vertex is at some given height
    // this allows us to determine the pigs motion in its parabolic movement when getting launched
    // formula for quadratic formula is ax^2 + bx + c
    // C controlls the y offset of the formula
    // a controls the width of the parabola (more specifically the second derivative)
    // b controls the angle of the parabola (more specifically the first derivative)
    // both derivatives are at x = 0
    // what we can do is make sure the origin is the vertex, thus make the first derivate zero so it is a maximum
    // thus b = 0 
    // but we need the origin to be the first zero (since at time = 0 the starting height is 0)
    // thus we can do a(x-b)^2 + c
    // now b is the x offset
    // we now need a way to define the distance the pig will move
    // height is determined by c as that will determine the y value of the vertex
    // now we need the vertex to be at the halway point, so it must be the distance covered/2
    // now we need a way for the vertex zeros to be at 0 and the distance covered
    // solving the quadratic formula for a we get a=-(db+c)/(d^2)   
    // in this case b is the b from the first equation, thus 0 so we are left with c/(d^2)
    // since c and d are given as parameters this works except for the fact that this will make the zero at d
    // but this equation had its vertex at 0, so instead we need to put it at d/2 so that when it is offset by
    // d/2 with the new definition of b then the zero will be at d
    // giving us a final equation of a = c/((d/2)^2)
    // now we can substitute everything to give us an equation of 
    // y = (height/((distance/2)^2))(x-(distance/2))^2 + height

    // why did I explain all of this lol now this gonna set the standard for how I have to comment out the rest of the code :(
    // anyways I hope that is clear I kinda only wrote it once and then haven't checked it for clarity or anything so yeah :)

    public static Vector3 getPosition(Vector3 launchFrom, Vector3 launchDirection, float launchDist, float height, float x)
    {
        return launchFrom + (launchDirection.normalized * x) + new Vector3(0, -height / (Mathf.Pow((launchDist / 2), 2)) * Mathf.Pow((x - (launchDist / 2)), 2) + height, 0);
    }

    // alright im sorry but im not commenting out as much as I did before, literally all I did was solve for the inverse of the above equation. 
    // allows for finding the distance x needed if you want to fall onto a height y (used for when snapping on to catapults)
    public static float invGetPosition(float dist, float height, float y)
    {
        float a = height / (Mathf.Pow(dist / 2f, 2));
        float b = dist / 2f;
        return Mathf.Sqrt((height - y) / a) + b;
        // return Mathf.Sqrt(((height - y) / (height / (Mathf.Pow(dist / 2f, 2)))) + (dist / 2f));
    }

}




// Old failed physics, keep here for safekeeping in case I ever need to go back to it


// public Vector3 checkPigs(Vector3 initialPosition, Vector3 velocity, bool isPlayer, float marginOfError){
//         float totalRadii = (isPlayer) ? Variables.playerRadius + Variables.PIG_RADius : Variables.PIG_RADius * 2;
//         Vector3 offset;
//         Vector3 position = initialPosition;
//         Vector3 finalVel = velocity;
//         //for safety make sure it is not already inside of a pig
//         foreach (Transform pig in pigs){
//             //get vector of the distance between pig and player
//             offset = position - pig.position;
//             //if the player is inside of the pig get it out
//             if(offset.magnitude < totalRadii){
//                 Debug.Log("Inside Pig");
//                 // find the direction that player is in from the pig then move to the position
//                 // that is in that direction and with a magnitude of the size of both colliders combined
//                 // this will position the player in the direction that it came from on a circle with a 
//                 radius of the combined collider radii
//                 offset = offset.normalized * totalRadii;
//                 position = pig.position + offset;
//             }
//         }

//         //see if it can move somewhere without pig in the way
//         foreach (Transform pig in pigs){
//             //get vector of the distance between the position the player will be and pig
//             offset = (position + finalVel) - pig.position;
//             // Debug.Log("Overall Offset is " + offset.magnitude);
//             if(offset.magnitude < totalRadii){
//                 // Debug.Log("moving Into Pig");
//                 // we will procedurally move the player along its velocity vector untill it is close to
//                 // the pig but still outside the collider
//                 // we can use an algorithm similar to the binary search to speed things up 
//                 Vector3 lower = Vector3.zero;
//                 Vector3 upper = finalVel;
//                 Vector3 middle;
//                 int safetyCount = 0;
//                 do{
//                     safetyCount++;
//                     middle = (lower + upper)/2;
//                     offset = (position + middle) - pig.position;
//                     Debug.Log(safetyCount);
//                     if(offset.magnitude < totalRadii){
//                         upper = middle;
//                     }
//                     else{
//                         lower = middle;
//                     }
//                 }
//                 while (!(offset.magnitude > totalRadii && offset.magnitude < totalRadii + marginOfError) && safetyCount < 50);
//                 finalVel = initialPosition - (pig.position + offset);
//             }
//         }
//         return position + finalVel;

//     }

