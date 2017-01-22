using UnityEngine;
using System.Collections;

public class RaycastToCamera : MonoBehaviour {
	public float distance = 8f;
	public Transform target;
	
	void Update () 
	{
		transform.LookAt(target);

		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
		{
			distance = hit.distance;
		}
	}
}
