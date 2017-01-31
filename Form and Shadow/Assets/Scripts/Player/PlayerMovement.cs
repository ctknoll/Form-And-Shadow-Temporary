using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour 
{
	public static Vector3 playerStartPosition;
	public static bool in3DSpace;

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
		controller = GetComponent<CharacterController>();
		jumpSpeedCurrent = jumpSpeed;
	}

	void Update() 
	{
        // Movement Methods
        PlayerJumpandGravity();

		// In the case that the player is controlling their shadow, force the base player character
		// to follow the shadow in the wall
		if(in3DSpace)
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
		if (Input.GetButton("Jump") && jumpHeldTime < jumpTime && !isGrabbing)
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
		// Check if the player is shifting into or out from the wall
		if(in3DSpace)
		{
			// Cast a ray in the direction of the light source and see if it hits a shadow wall
			if (Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity))
			{
				if(hit.collider.gameObject.tag == "Shadow Wall")
				{
					// If so, cast a ray offset 0.2 upwards and downwards in the y and see if it collides with any shadow colliders
					// behind the wall. If not, shift the player into the wall
					if(!Physics.Raycast(hit.point + new Vector3(0, 0.2f, 0), LightSourceControl.lightSourceDirection, 1f, 1 << 11) && 
						!Physics.Raycast(hit.point - new Vector3(0, 0.2f, 0), LightSourceControl.lightSourceDirection, 1f, 1 << 11))
					{
						// This basically manages all of the turning off and on of the CharacterControllers and the 3Dspace global
						// variable and sets the distanceFromShadow float
						in3DSpace = false;
						GetComponent<CharacterController>().enabled = false;
						playerShadow.GetComponent<CharacterController>().enabled = true;
						controller = playerShadow.GetComponent<CharacterController>();
						GetComponent<PlayerShadowCast>().CastShadow();
						distanceFromShadow = transform.position - playerShadow.transform.position;
						Debug.Log("In Wall");
					}
					else
						Debug.Log("You can't transfer, there is a shadow in the way!");
				}
				else
					Debug.Log("You can't transfer, you didn't hit a shadow wall!");
			}
		}
		else
		{
			// To prevent issues with moving platforms, remove parents when transferring
			transform.parent = null;
			playerShadow.transform.parent = null;

			// Get a list of gameobjects from the PlayerShadowCollider script on the player shadow
			List<GameObject> transferPlatforms = playerShadow.GetComponent<PlayerShadowCollider>().GetTransferPlatforms();

			// If there are platforms to which the player can transfer out to, sort the platforms by their distance from the player
			// and then transfer the player character to the closest one
			if(transferPlatforms.Count != 0)
			{
				transferPlatforms.Sort(delegate(GameObject t1, GameObject t2) {
					return Vector3.Distance(t1.transform.position, playerShadow.transform.position).CompareTo(Vector3.Distance(t2.transform.position, playerShadow.transform.position));
				});
				transform.position = new Vector3(playerShadow.transform.position.x, playerShadow.transform.position.y, transferPlatforms[0].transform.position.z);
			}
			in3DSpace = true;
			playerShadow.GetComponent<CharacterController>().enabled = false;
			GetComponent<CharacterController>().enabled = true;
			controller = GetComponent<CharacterController>();
			Debug.Log("Out of wall");
		}
	}
}

