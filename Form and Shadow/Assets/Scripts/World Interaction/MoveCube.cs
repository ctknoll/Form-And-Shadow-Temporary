using UnityEngine;

public class MoveCube : MonoBehaviour {
	public bool canInteract;
	public bool grabbed;

	private GameObject player;

	void Update()
	{
		if(canInteract)
		{
			if(Input.GetButtonDown("Grab"))
			{
				if(!grabbed)
				{
					grabbed = true;
					transform.parent = player.transform;
					PlayerMovement.isGrabbing = true;
				}
				else
				{
					grabbed = false;
					transform.parent = null;
					PlayerMovement.isGrabbing = false;
				}
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			player = other.gameObject;
			canInteract = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			player = null;
			canInteract = false;
		}
	}
}
