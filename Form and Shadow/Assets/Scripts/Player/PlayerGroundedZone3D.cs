using UnityEngine;

public class PlayerGroundedZone3D : MonoBehaviour
{
	void OnTriggerStay(Collider other)
	{
		PlayerMovement.grounded3D = true;
	}

	void OnTriggerExit(Collider other)
	{
		PlayerMovement.grounded3D = false;
	}
}

