﻿using UnityEngine;

public class PushCube : MonoBehaviour {
    public bool canInteract;
    public bool grabbed;
    public bool blockedAhead;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Vector3 directionAwayFromPlayer;
    [HideInInspector]
	public Vector3 startPos;

	void Start()
	{
		player = GameObject.Find("Player_Character");
		startPos = transform.position;
	}

	void Update()
	{
        if (canInteract)
        {
            if (!grabbed)
            {
                GameController.CheckInteractToolip(true, true);
                if (Input.GetButton("Grab"))
                {
                    if (!GameController.e_Grab_First_Time_Used)
                        GameController.e_Grab_First_Time_Used = true;
                    GameController.CheckInteractToolip(false, true);
                    grabbed = true;
                    player.transform.rotation = Quaternion.LookRotation(directionAwayFromPlayer, Vector3.up);
                    transform.parent = player.transform;
                    PlayerMovement.isGrabbing = true;
                    PlayerMovement.grabbedObject = gameObject;
                }
            }

            if (Input.GetButtonUp("Grab"))
            {
                grabbed = false;
                transform.parent = null;
                PlayerMovement.isGrabbing = false;
                PlayerMovement.grabbedObject = null;
            }
        }

		if(GameController.resetting)
		{
            GameController.CheckInteractToolip(false, true);
            transform.position = startPos;
			grabbed = false;
            transform.parent = null;
			PlayerMovement.isGrabbing = false;
			PlayerMovement.grabbedObject = null;
		}
	}
}
