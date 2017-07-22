﻿using UnityEngine;

public class PropellorPlatform : MonoBehaviour {
	public float rotationSpeed;
    public bool rotateClockwise;

    public bool playerChildedIn3D;
	public bool playerChildedIn2D;

	void Start()
	{
		GetComponent<BoxCollider>().size = new Vector3(gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.x, 0.5f, 
			gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.z);
	}

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			playerChildedIn3D = true;
			other.gameObject.transform.parent = gameObject.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			playerChildedIn3D = false;
			other.gameObject.transform.parent = null;
		}
	}

	void FixedUpdate () 
	{
		if(!PlayerShadowInteraction.shadowShiftingIn && !PlayerShadowInteraction.shadowShiftingOut && !GameController.resetting && !GameController.paused)
        {
            if(rotateClockwise)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
            }
            else
            {
                transform.Rotate(Vector3.up, Time.deltaTime * -rotationSpeed);
            }
        }
    }
}