using UnityEngine;
using System.Collections;

public class PlayerGroundedZone : MonoBehaviour
{
	void OnTriggerStay(Collider other)
	{
		PlayerMovement.grounded = true;
	}

	void OnTriggerExit(Collider other)
	{
		PlayerMovement.grounded = false;
	}
}

