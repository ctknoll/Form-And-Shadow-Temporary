using UnityEngine;

public class PlayerGroundedZone3D : MonoBehaviour
{
	void OnTriggerStay(Collider other)
	{
		if(other.GetComponent<Collider>().isTrigger !=true)
		PlayerMovement.grounded3D = true;
	}

	void OnTriggerExit(Collider other)
	{
		if(other.GetComponent<Collider>().isTrigger !=true)
		PlayerMovement.grounded3D = false;
	}
}

