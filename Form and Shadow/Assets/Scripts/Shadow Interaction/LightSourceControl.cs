using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceControl : MonoBehaviour 
{
	public static Vector3 lightSourceDirection;
	
	void Update () 
	{
		lightSourceDirection = transform.forward;
	}
}
