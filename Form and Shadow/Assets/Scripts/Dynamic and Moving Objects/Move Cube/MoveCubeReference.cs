using UnityEngine;

public class MoveCubeReference : MonoBehaviour
{
	public GameObject acrossReference;
	public bool blocked;
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
			moveCube.directionAwayFromPlayer = (moveCubeMesh.transform.position - transform.position);
			if(acrossReference.GetComponent<MoveCubeReference>().blocked)
				moveCube.blockedAhead = true;
			else
				moveCube.blockedAhead = false;
		}
		else
		{
			blocked = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
            GameController.ResetInteractText();
            moveCube.canInteract = false;
			moveCube.gameObject.transform.parent = null;
			moveCube.grabbed = false;
			moveCube.transform.parent = null;
			PlayerMovement.isGrabbing = false;
			PlayerMovement.grabbedObject = null;
		}
		else
			blocked = false;
	}
}

