using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PulleySystemPlatform : MonoBehaviour {
	public float moveSpeed;
	public float destroyDelayTime;


	public Transform pulleyStart;
	public Transform pulleyEnd;

	[HideInInspector]
	public bool playerChildedIn3D;
	[HideInInspector]
	public bool playerChildedIn2D;
	[HideInInspector]
	public bool atEndOfRoute;

	private float startTime;
	private Vector3 moveDirection;

	void Start()
	{
		moveDirection = (pulleyEnd.transform.position - pulleyStart.transform.position).normalized;
		GetComponent<BoxCollider>().size = new Vector3(gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.x, 0.5f, 
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

	void FixedUpdate () 
	{
		if(!PlayerMovement.shiftingIn && !PlayerMovement.shiftingOut)
		{
			transform.position += moveSpeed * moveDirection;

			if(atEndOfRoute)
			{
				StartCoroutine(DestroyPlatform());
			}
		}
	}

	public IEnumerator DestroyPlatform()
	{
		yield return new WaitForSeconds(destroyDelayTime);
		if(playerChildedIn3D)
			GameObject.Find("Player_Character").transform.parent = null;
		else if(playerChildedIn2D)
			GameObject.Find("Player_Shadow").transform.parent = null;
		Destroy(gameObject);
	}
}