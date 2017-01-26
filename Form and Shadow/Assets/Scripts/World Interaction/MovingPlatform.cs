using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
	public enum ShiftDirection {X, Y, Z};
	public ShiftDirection moveDirection;
	public float moveDistance;
	public float moveSpeed;

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.gameObject.transform.parent = gameObject.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.gameObject.transform.parent = null;
		}
	}

	void Update () 
	{
		switch(moveDirection)
		{
		case ShiftDirection.X:
			transform.position = new Vector3(Mathf.PingPong(Time.time * moveSpeed, moveDistance), transform.position.y, transform.position.z);
			break;
		case ShiftDirection.Y:
			transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time * moveSpeed, moveDistance), transform.position.z);
			break;
		case ShiftDirection.Z:
			transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.PingPong(Time.time * moveSpeed, moveDistance));
			break;
		default:
			break;
		}
	}
}
