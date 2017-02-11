using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceControl : MonoBehaviour 
{
	public static Vector3 lightSourceDirection;

	public static bool zAxisMovement;
	public static bool xAxisMovement;

	void Start ()
	{
		lightSourceDirection = transform.forward;
		CheckLightingDirection();
	}

	void Update () 
	{
        //transform.Rotate(0, 90, 0);
        lightSourceDirection = transform.forward;

    }

	public void CheckLightingDirection()
	{
        Debug.Log(lightSourceDirection + ", " + GameObject.Find("Light Reference").transform.forward);
        Debug.Log(transform.forward);
        if (lightSourceDirection == GameObject.Find("Light Reference").transform.forward || 
			-1 * lightSourceDirection == GameObject.Find("Light Reference").transform.forward) 
		{
            zAxisMovement = true;
			xAxisMovement = false;
            Debug.Log("z");
        }
		else if(LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.right || 
			-1 * LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.right) 
		{
            zAxisMovement = false;
			xAxisMovement = true;
            Debug.Log("x");
		}
    }

    //This function is suboptimal -- it casts EVERY time, creating a lot of unnecesary overhead
    public void turnLightSource()
    {
        GameObject.Find("/Directional light").transform.Rotate(0, -90, 0);
        lightSourceDirection = GameObject.Find("/Directional light").transform.forward;
        CheckLightingDirection();
        Object[] listOfObjs = Object.FindObjectsOfType(typeof(ShadowCast));
        Debug.Log(listOfObjs.Length);
        foreach(ShadowCast shadow in listOfObjs)
        {
            //if(shadow.shadowCollider.Count < 5)
            shadow.CastShadow();
        }
    }
}
