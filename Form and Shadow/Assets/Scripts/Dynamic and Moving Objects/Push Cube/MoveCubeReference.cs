using UnityEngine;

public class MoveCubeReference : MonoBehaviour
{
	private MoveCube moveCube;
	private GameObject moveCubeMesh;
    public GameObject acrossReference;
    
    public bool blocked;

	void Start()
	{
		moveCube = GetComponentInParent<MoveCube>();
		moveCubeMesh = transform.parent.GetComponentInChildren<ShadowCast>().gameObject;
	}

    void Update()
    {
        if (!PlayerMovement.in3DSpace)
        {
            moveCube.canInteract = false;
            GameController.ToggleInteractTooltip(false);
        }
    }

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && PlayerMovement.in3DSpace)
		{
			moveCube.canInteract = true;
			moveCube.directionAwayFromPlayer = (moveCubeMesh.transform.position - transform.position).normalized;
            if (acrossReference.GetComponent<MoveCubeReference>().blocked)
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
		if(other.gameObject.tag == "Player" && !PlayerMovement.isGrabbing)
		{
            GameController.ToggleInteractTooltip(false);
            moveCube.canInteract = false;
			moveCube.grabbed = false;
            moveCube.transform.parent = null;
			PlayerMovement.isGrabbing = false;
			PlayerMovement.grabbedObject = null;
		}
        blocked = false;
	}
}

