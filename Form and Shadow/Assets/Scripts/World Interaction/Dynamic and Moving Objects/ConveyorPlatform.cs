using System.Collections;
using UnityEngine;

public class ConveyorPlatform : MonoBehaviour {
	public float moveSpeed;

	public Transform conveyorStart;
	public Transform conveyorEnd;

	public bool playerChildedIn3D;
	public bool playerChildedIn2D;

	private float startTime;
	private float journeyLength;

	void Start()
	{
		startTime = Time.time;
		journeyLength = Vector3.Distance(conveyorStart.position, conveyorEnd.position);
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
		float distCovered = (Time.time - startTime) * moveSpeed;
		float fracJourney = distCovered / journeyLength;
		transform.position = Vector3.Lerp(conveyorStart.transform.position, conveyorEnd.transform.position, fracJourney);

		if(transform.position == conveyorEnd.position)
		{
			if(playerChildedIn3D)
				transform.GetChild(1).transform.parent = null;
			else if(playerChildedIn2D)
				gameObject.GetComponentInChildren<MovingShadowPlatform>().gameObject.transform.GetChild(0).transform.parent = null;
			Destroy(gameObject);
		}
	}
}