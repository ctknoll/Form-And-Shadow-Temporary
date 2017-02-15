using UnityEngine;

public class PlayerGroundedZone2D : MonoBehaviour
{
	void OnTriggerStay(Collider other)
	{
		PlayerMovement.grounded2D = true;
	}

	void OnTriggerExit(Collider other)
	{
		PlayerMovement.grounded2D = false;
	}
}

