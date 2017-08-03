using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_OnTrigger : MonoBehaviour {
	public enum ShiftDirection {X, Y, Z};
	public ShiftDirection moveDirection;
	public float moveDistance;
	public float moveSpeed;
	public float slowValue = 0;
	public float triggerTime = 0;
	public bool activatePlatform = false;

	private float personalTime;
	public float distanceTraveled = 0;

	public bool playerChildedIn3D;
	public bool playerChildedIn2D;

	private Vector3 startPosition;

	void Start()
	{
		personalTime = 0;
		distanceTraveled = 0;
		startPosition = transform.position;
		GetComponent<BoxCollider>().size = new Vector3(gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.x, 0.5f, 
			gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.z);
	}

	public IEnumerator waitForPlatform(float timeToWait)
	{
		yield return new WaitForSeconds(timeToWait);
		slowValue = 1;
		activatePlatform = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			playerChildedIn3D = true;
			other.gameObject.transform.parent = gameObject.transform;
			StartCoroutine (waitForPlatform (triggerTime));
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

	void FixedUpdate () 
	{
		if (distanceTraveled < (moveDistance - 0.1)) {
			if (activatePlatform == true){			
				if (PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PLAYERSTATE.SHIFTING) {
					personalTime += slowValue * Time.deltaTime;
				}

				switch (moveDirection) {
				case ShiftDirection.X:
					transform.position = new Vector3 (startPosition.x - Mathf.Sin (personalTime * moveSpeed) * moveDistance, transform.position.y, transform.position.z);
					distanceTraveled = startPosition.x - transform.position.x;
					break;
				case ShiftDirection.Y:
					transform.position = new Vector3 (transform.position.x, startPosition.y - Mathf.Sin (personalTime * moveSpeed) * moveDistance, transform.position.z);
					distanceTraveled = startPosition.y - transform.position.y;
					break;
				case ShiftDirection.Z:
					transform.position = new Vector3 (transform.position.x, transform.position.y, startPosition.z - Mathf.Sin (personalTime * moveSpeed) * moveDistance);
					distanceTraveled = startPosition.z - transform.position.z;
					break;
				default:
					break;
				}
			}
		}
	}
}
