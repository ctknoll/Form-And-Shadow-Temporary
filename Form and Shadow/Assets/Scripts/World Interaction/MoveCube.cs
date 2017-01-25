using UnityEngine;

public class MoveCube : MonoBehaviour {
	public bool grabbed;
	public PlayerMovement playerMovement;

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(Input.GetButtonDown("Grab"))
			{
				if(!grabbed)
					transform.parent = other.gameObject.transform;
				else if(grabbed)
					transform.parent = null;
				grabbed = !grabbed;
				PlayerMovement.isGrabbing = !PlayerMovement.isGrabbing;
			}
		}
	}
}
