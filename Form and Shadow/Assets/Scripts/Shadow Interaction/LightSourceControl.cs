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
		lightSourceDirection = transform.forward;
	}

	public void CheckLightingDirection()
	{
		if (lightSourceDirection == GameObject.Find("Light Reference").transform.forward || 
			-1 * lightSourceDirection == GameObject.Find("Light Reference").transform.forward) 
		{
			zAxisMovement = true;
			xAxisMovement = false;
		}
		else if(LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.right || 
			-1 * LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.right) 
		{
			zAxisMovement = false;
			xAxisMovement = true;
		}
	}
}
