using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceControl : MonoBehaviour 
{
	public static Vector3 lightSourceDirection;
	public static Vector3 startForward;

	void Start ()
	{
		lightSourceDirection = transform.forward;
		startForward = lightSourceDirection;
	}

	void Update () 
	{
		lightSourceDirection = transform.forward;
	}

}
