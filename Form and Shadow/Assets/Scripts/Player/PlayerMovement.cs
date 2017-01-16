using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public Transform cameraTransform;
	public float movementSpeed;
	public float jumpSpeed;
	public float jumpTime;
	public float gravity;

	private Vector3 moveDirection = Vector3.zero;
	private Vector3 rotationDirection;
	private float jumpHeldTime;

	private CharacterController controller;

	void Start()
	{
		controller = GetComponent<CharacterController>();
	}
	void Update() 
	{
		if(Input.GetKeyDown(KeyCode.X))
			CameraControl.in3DSpace = !CameraControl.in3DSpace;
		if (Input.GetButton("Jump") && jumpHeldTime < jumpTime)
		{
			controller.Move(new Vector3(0, jumpSpeed * Time.deltaTime));
			jumpHeldTime += Time.deltaTime;
		}
		if(Input.GetButtonUp("Jump"))
			jumpHeldTime = 0;

		rotationDirection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
		transform.rotation = Quaternion.LookRotation(rotationDirection, Vector3.up);

		controller.Move(new Vector3(0, -gravity * Time.deltaTime, 0));

		if(Input.GetAxis("Horizontal") >= 0)
		{
			controller.Move(transform.right * Time.deltaTime * movementSpeed);
		}
		if(Input.GetAxis("Horizontal") <= 0)
		{
			controller.Move(-transform.right * Time.deltaTime * movementSpeed);
		}
		if(Input.GetAxis("Vertical") >= 0)
		{
			controller.Move(transform.forward * Time.deltaTime * movementSpeed);
		}
		if(Input.GetAxis("Vertical") <= 0)
		{
			controller.Move(-transform.forward * Time.deltaTime * movementSpeed);
		}
	}
}