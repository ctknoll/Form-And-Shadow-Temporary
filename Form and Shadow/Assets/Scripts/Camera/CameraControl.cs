﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class CameraControl : MonoBehaviour
{
	[Header("Player Target")]
    public Transform target2D;
	public Transform target3D;

	[Header("3D Camera Variables")]
	public float distanceToPlayer3D;
    public float xSpeed;
    public float ySpeed;

    public float yMinLimit;
    public float yMaxLimit;

    float x = 0.0f;
    float y = 0.0f;

	[Header("2D Camera Variables")]
	public float smoothSpeed = 0.125f;
	public float distanceToPlayer2D;

	public float cameraPanDuration;
	[HideInInspector]
	public static bool cameraIsPanning;



    void Start()
    {
		cameraIsPanning = false;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
		if(!cameraIsPanning && !PlayerMovement.shiftingOut && !PlayerMovement.shiftingIn)
		{
			if (PlayerMovement.in3DSpace)
	        {
	            x += Input.GetAxis("Mouse X") * xSpeed * distanceToPlayer3D * 0.02f;
	            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

	            y = ClampAngle(y, yMinLimit, yMaxLimit);

	            Quaternion rotation = Quaternion.Euler(y, x, 0);

	            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distanceToPlayer3D);
	            Vector3 position = rotation * negDistance + target3D.position;

	            transform.rotation = rotation;
	            transform.position = position;
	        }

			else
			{
				Vector3 desiredPosition = target2D.transform.position + -transform.forward * distanceToPlayer2D;

				transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
			}
		}
		else
		{
			if(PlayerMovement.in3DSpace)
				transform.LookAt(target3D.GetComponent<PlayerMovement>().transitionInFollow.transform);
			else if(!PlayerMovement.in3DSpace)
			{
				if(target3D.GetComponent<PlayerMovement>().transitionOutFollow)
					transform.LookAt(target3D.GetComponent<PlayerMovement>().transitionOutFollow.transform);
				else
					transform.LookAt(target3D.GetComponent<PlayerMovement>().transitionInFollow.transform);
			}
		}
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}