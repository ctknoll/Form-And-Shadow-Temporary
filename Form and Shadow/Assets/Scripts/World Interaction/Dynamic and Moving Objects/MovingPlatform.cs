using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
	public enum ShiftDirection {X, Y, Z};
	public ShiftDirection moveDirection;
	public float moveDistance;
	public float moveSpeed;

	public bool playerChildedIn3D;
	public bool playerChildedIn2D;

	private Vector3 startPosition;

	void Start()
	{
		startPosition = transform.position;
		GetComponent<BoxCollider>().size = new Vector3(gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.x, 1f, 
			gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.z);
	}

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			playerChildedIn3D = true;
			other.gameObject.transform.parent = gameObject.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			playerChildedIn3D = false;
			other.gameObject.transform.parent = null;
		}
	}

	void Update () 
	{
		switch(moveDirection)
		{
		case ShiftDirection.X:
			transform.position = new Vector3(startPosition.x + Mathf.Sin(Time.time * moveSpeed) * moveDistance, transform.position.y, transform.position.z);
			break;
		case ShiftDirection.Y:
			transform.position = new Vector3(transform.position.x, startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance, transform.position.z);
			break;
		case ShiftDirection.Z:
			transform.position = new Vector3(transform.position.x, transform.position.y, startPosition.z + Mathf.Sin(Time.time * moveSpeed) * moveDistance);
			break;
		default:
			break;
		}
	}
}
