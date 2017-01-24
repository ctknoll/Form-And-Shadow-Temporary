using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public static bool in3DSpace;
	public static bool shadowMelded;

	public GameObject playerShadow;
	public Transform cameraTransform;
	public float movementSpeed;
	public float jumpSpeed;
	public float jumpTime;
	public float gravity;

	private Vector3 rotationDirection;
	private float jumpHeldTime;

    [SerializeField]
    private CharacterController controller;

	void Start()
	{
		in3DSpace = true;
		shadowMelded = false;
		controller = GetComponent<CharacterController>();
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
		if(Input.GetButtonDown("Fire1"))
			CheckShadowMeld();
	}

	public void PlayerJumpandGravity()
	{
		if (Input.GetButton("Jump") && jumpHeldTime < jumpTime)
		{
			controller.Move(new Vector3(0, jumpSpeed * Time.deltaTime, 0));
			jumpHeldTime += Time.deltaTime;
		}
		if(Input.GetButtonUp("Jump"))
			jumpHeldTime = 0;

		controller.Move(new Vector3(0, -gravity * Time.deltaTime, 0));

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

	public void FollowShadow()
	{
		transform.position = new Vector3(playerShadow.transform.position.x, playerShadow.transform.position.y, transform.position.z);
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

	public void CheckShadowMeld()
	{
		RaycastHit hit;

		if(!shadowMelded)
		{
			if (Physics.SphereCast(transform.position, 1f, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity))
			{
				if(hit.collider.gameObject.tag == "Shadow Wall")
				{
					shadowMelded = true;
					GetComponent<CharacterController>().enabled = false;
					playerShadow.GetComponent<CharacterController>().enabled = true;
					controller = playerShadow.GetComponent<CharacterController>();
					Debug.Log("You are shadowmelded");
				}
				else
					Debug.Log("You can't shadowmeld here!");
			}
			else
				Debug.Log("You can't shadowmeld here!");
		}
		else
		{
			shadowMelded = false;
			playerShadow.GetComponent<CharacterController>().enabled = false;
			GetComponent<CharacterController>().enabled = true;
			controller = GetComponent<CharacterController>();
			Debug.Log("You are no longer shadowmelded");
		}
	}
}

