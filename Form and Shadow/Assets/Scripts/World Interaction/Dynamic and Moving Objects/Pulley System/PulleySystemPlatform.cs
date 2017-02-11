using UnityEngine;

public class PulleySystemPlatform : MonoBehaviour {
	public float moveSpeed;

	public Transform pulleyStart;
	public Transform pulleyEnd;

	public bool playerChildedIn3D;
	public bool playerChildedIn2D;

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

			if(transform.position == pulleyEnd.position)
			{
				if(playerChildedIn3D)
					GameObject.Find("Player_Character").transform.parent = null;
				else if(playerChildedIn2D)
					GameObject.Find("Player_Shadow").transform.parent = null;
				Destroy(gameObject);
			}
		}
	}
}