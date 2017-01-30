﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public static Vector3 playerStartPosition;
	public static bool in3DSpace;
	public static bool shadowMelded;

	[Header("Object References")]
	public GameObject playerShadow;
	public Transform cameraTransform;

	[Header("Movement Variables")]
	public float movementSpeed;
	public float grabMovementSpeed;
	public float jumpSpeed;
	private float jumpSpeedCurrent;
	public float jumpTime;
	public float gravity;

	[Header("Interaction Variables")]
	public static bool isGrabbing;
	public GameObject grabbedObject;


	private Vector3 rotationDirection;
	private float jumpHeldTime;
    public Vector3 distanceFromShadow;

	public CharacterController controller;

	void Start()
	{
		playerStartPosition = transform.position;
		in3DSpace = true;
		shadowMelded = false;
		controller = GetComponent<CharacterController>();
		jumpSpeedCurrent = jumpSpeed;
	}

	void Update() 
	{
        // Movement Methods
        PlayerJumpandGravity();

		// In the case that the player is controlling their shadow, force the base player character
		// to follow the shadow in the wall
		if(in3DSpace && !shadowMelded)
		{
			PlayerMovement3D();
		}
		else
		{
			PlayerMovement2D();
			FollowShadow();
		}
		// Shadow shift Methods
		if(Input.GetButtonDown("Fire3"))
			CheckShadowShift();
	}

    public void PlayerJumpandGravity()
    {
        if (Input.GetButton("Jump") && jumpHeldTime < jumpTime)
        {
			if (jumpSpeedCurrent <= ((jumpSpeed - gravity) / jumpTime) * Time.deltaTime)
				jumpSpeedCurrent = 0;
			else jumpSpeedCurrent -= ((jumpSpeed - gravity) / jumpTime) * Time.deltaTime;
			controller.Move(new Vector3(0, jumpSpeedCurrent * Time.deltaTime, 0));
            jumpHeldTime += Time.deltaTime;
        }

		controller.Move(new Vector3(0, -gravity * Time.deltaTime, 0));

        RaycastHit hit;
        if (in3DSpace)
        {
			if (Physics.Raycast (new Vector3 (transform.position.x, transform.position.y - (transform.lossyScale.y), transform.position.z), -Vector3.up, out hit, .1f)) 
			{
				jumpHeldTime = 0;
				jumpSpeedCurrent = jumpSpeed;
			}

        }
        else
        {
            Debug.DrawLine(playerShadow.transform.position + Vector3.down * transform.lossyScale.y, playerShadow.transform.position + Vector3.down * (transform.lossyScale.y+.1f), Color.red, .25f);
            if (Physics.Raycast(new Vector3(playerShadow.transform.position.x, playerShadow.transform.position.y - (playerShadow.transform.lossyScale.y), playerShadow.transform.position.z), -Vector3.up, out hit, .1f))
			{
				jumpHeldTime = 0;
				jumpSpeedCurrent = jumpSpeed;
			}
        }
	}

	public void PlayerMovement2D()
	{
		if (Input.GetAxis("Horizontal") > 0)
		{
			controller.Move(-playerShadow.transform.right * Time.deltaTime * movementSpeed);
		}
		if (Input.GetAxis("Horizontal") < 0)
		{
			controller.Move(playerShadow.transform.right * Time.deltaTime * movementSpeed);
		}

	}
	public void PlayerMovement3D()
	{
		if(!isGrabbing)
		{
			rotationDirection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
			transform.rotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
			if (Input.GetAxis("Horizontal") > 0)
			{
				controller.Move(cameraTransform.right * Time.deltaTime * movementSpeed);
			}
			if (Input.GetAxis("Horizontal") < 0)
			{
				controller.Move(-cameraTransform.right * Time.deltaTime * movementSpeed);
			}
			if (Input.GetAxis("Vertical") > 0)
			{
				controller.Move(cameraTransform.forward * Time.deltaTime * movementSpeed);
			}
			if (Input.GetAxis("Vertical") < 0)
			{
				controller.Move(-cameraTransform.forward * Time.deltaTime * movementSpeed);
			}
		}
		else
		{
			if (Input.GetAxis("Horizontal") > 0)
			{
				controller.Move(cameraTransform.right * Time.deltaTime * grabMovementSpeed);
			}
			if (Input.GetAxis("Horizontal") < 0)
			{
				controller.Move(-cameraTransform.right * Time.deltaTime * grabMovementSpeed);
			}
			if (Input.GetAxis("Vertical") > 0)
			{
				controller.Move(cameraTransform.forward * Time.deltaTime * grabMovementSpeed);
			}
			if (Input.GetAxis("Vertical") < 0)
			{
				controller.Move(-cameraTransform.forward * Time.deltaTime * grabMovementSpeed);
			}
		}
	}

	public void FollowShadow()
	{
		transform.position = playerShadow.transform.position + distanceFromShadow;
	}

	public void CheckShadowShift()
	{
		RaycastHit hit;

		if(in3DSpace)
		{
			if (Physics.SphereCast(transform.position, 1f, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity))
			{
				if(hit.collider.gameObject.tag == "Shadow Wall")
				{	
					in3DSpace = false;
					GetComponent<CharacterController>().enabled = false;
					playerShadow.GetComponent<CharacterController>().enabled = true;
					controller = playerShadow.GetComponent<CharacterController>();
					GetComponent<PlayerShadowCast>().CastShadow();
                    distanceFromShadow = transform.position - playerShadow.transform.position;

					Debug.Log("In Wall");
				}
				else
					Debug.Log("You can't transfer!");
			}
			else
				Debug.Log("You can't transfer!");
		}
		else
		{
			if(!Physics.Linecast(playerShadow.transform.position, gameObject.transform.position, out hit))
			{
				in3DSpace = true;
				playerShadow.GetComponent<CharacterController>().enabled = false;
				GetComponent<CharacterController>().enabled = true;
				controller = GetComponent<CharacterController>();
				Debug.Log("Out of wall");
			}
			else
				Debug.Log("You can't transfer! You've hit " + hit.collider.gameObject);
		}
	}
}

