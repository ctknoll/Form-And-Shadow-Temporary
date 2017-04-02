using UnityEngine;

public class PlayerGroundedZone2D : MonoBehaviour{

	void OnTriggerStay(Collider other)
	{
		if(other.GetComponent<Collider>().isTrigger != true)
		    PlayerMovement.grounded2D = true;
	}

	void OnTriggerExit(Collider other)
	{
		if(other.GetComponent<Collider>().isTrigger !=true)
		    PlayerMovement.grounded2D = false;
	}
}

