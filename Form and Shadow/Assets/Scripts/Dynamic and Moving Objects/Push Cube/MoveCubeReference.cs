using UnityEngine;

public class MoveCubeReference : MonoBehaviour
{
	private MoveCube moveCube;
	private GameObject moveCubeMesh;

	void Start()
	{
		moveCube = GetComponentInParent<MoveCube>();
		moveCubeMesh = transform.parent.GetComponentInChildren<ShadowCast>().gameObject;
	}
		
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			moveCube.canInteract = true;
			moveCube.directionAwayFromPlayer = (moveCubeMesh.transform.position - transform.position).normalized;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
            GameController.ResetInteractText();
            moveCube.canInteract = false;
			moveCube.grabbed = false;
			PlayerMovement.isGrabbing = false;
			PlayerMovement.grabbedObject = null;
		}
	}
}

