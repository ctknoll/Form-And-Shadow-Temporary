﻿using UnityEngine;

public class PushCubeMoveReference : MonoBehaviour
{
	private PushCube pushCube;
	private GameObject moveCubeMesh;
    public GameObject acrossReference;
    
    public bool blocked;

	void Start()
	{
		pushCube = GetComponentInParent<PushCube>();
		moveCubeMesh = transform.parent.GetComponentInChildren<ShadowCast>().gameObject;
	}

    void Update()
    {
        if (!PlayerMovement.in3DSpace)
        {
            pushCube.canInteract = false;
            GameController.ToggleInteractTooltip(false);
        }
    }

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && PlayerMovement.in3DSpace)
		{
			pushCube.canInteract = true;
			pushCube.directionAwayFromPlayer = (moveCubeMesh.transform.position - transform.position).normalized;
            if (acrossReference.GetComponent<PushCubeMoveReference>().blocked)
                pushCube.blockedAhead = true;
            else
                pushCube.blockedAhead = false;
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
            pushCube.canInteract = false;
			pushCube.grabbed = false;
            pushCube.transform.parent = null;
			PlayerMovement.isGrabbing = false;
			PlayerMovement.grabbedObject = null;
		}
        blocked = false;
	}
}

