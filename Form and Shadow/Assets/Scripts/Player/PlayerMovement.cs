﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public Transform cameraTransform;
	public float movementSpeed;
	public float jumpSpeed;
	public float jumpTime;
	public float gravity;

    public GameObject shiftedPlane = null;
    public int shiftTimer = 0;

    private Vector3 moveDirection = Vector3.zero;
	private Vector3 rotationDirection;
	private float jumpHeldTime;

    [SerializeField]
    private CharacterController controller;

	void Start()
	{
		controller = GetComponent<CharacterController>();
	}
	void Update() 
	{
        if (Input.GetButton("Jump") && jumpHeldTime < jumpTime)
		{
			controller.Move(new Vector3(0, jumpSpeed * Time.deltaTime));
			jumpHeldTime += Time.deltaTime;
		}

        if(Input.GetButtonDown("Fire3"))
        {
            Debug.Log("Recieved");
            if (CameraControl.in3DSpace)
            {
                Debug.Log("Not In Wall");
				CameraControl.in3DSpace = !CastToWall.castToWall(gameObject);
                if (!CameraControl.in3DSpace) return;
            }
            else
            {
                Debug.Log("Not In Wall");
				CameraControl.in3DSpace = CastToWall.removeFromWall(gameObject);
                if (CameraControl.in3DSpace) return;
            }
        }

		if(Input.GetButtonUp("Jump"))
			jumpHeldTime = 0;

        Debug.Log("Check if moving");
        if (!CameraControl.in3DSpace)
        {
            Debug.Log("Trying to move");
			Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
            //Debug.Log(dir);
            Vector3 movement = (shiftedPlane.GetComponent<Transform>().right * dir.x) + (shiftedPlane.GetComponent<Transform>().forward * dir.y);
            Vector3 gravity2D = -1 * gravity * Time.deltaTime * shiftedPlane.GetComponent<Transform>().up;
            Debug.Log(shiftedPlane.GetComponent<Transform>().right);
            controller.Move(gravity2D);
            controller.Move(movement * Time.deltaTime * movementSpeed);
        }

        else
        {
            controller.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
            rotationDirection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
            transform.rotation = Quaternion.LookRotation(rotationDirection, Vector3.up);

            if (Input.GetAxis("Horizontal") >= 0)
            {
                controller.Move(transform.right * Time.deltaTime * movementSpeed);
            }
            if (Input.GetAxis("Horizontal") <= 0)
            {
                controller.Move(-transform.right * Time.deltaTime * movementSpeed);
            }
            if (Input.GetAxis("Vertical") >= 0)
            {
                controller.Move(transform.forward * Time.deltaTime * movementSpeed);
            }
            if (Input.GetAxis("Vertical") <= 0)
            {
                controller.Move(-transform.forward * Time.deltaTime * movementSpeed);
            }
        }
	}
}