using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour 
{
	public static Vector3 playerStartPosition;
	public static bool in3DSpace;
	public static bool shiftingOut;
	public static bool shiftingIn;

	[Header("Object References")]
	public GameObject playerShadow;
	public GameObject camera;
	public GameObject movementReference;
	public GameObject transitionFollowPrefab;
	public GameObject transitionInFollow;
	public GameObject transitionOutFollow;
	private List<GameObject> transferPlatforms;
	private CameraControl camControl;

	[Header("Movement Variables")]
	public float movementSpeed;
	public float grabMovementSpeed;


	public float jumpSpeed;
	public float jumpTime;
	public float gravity;
	public static bool grounded;
	private float verticalVelocity;


	[Header("Interaction Variables")]
	public static bool isGrabbing;
	public static GameObject grabbedObject;


	private Vector3 rotationDirection;
	private float jumpHeldTime;
	private Vector3 cameraPanInStartPos;
	private Vector3 cameraRelativeDirectionOffset;
	private float cameraPanInRelativeDistance;
	private int currentPlatformIndex;
    public Vector3 distanceFromShadow;

	public CharacterController controller;

	void Start()
	{

		playerStartPosition = transform.position;
		in3DSpace = true;
		shiftingOut = false;
		shiftingIn = false;
		camControl = camera.GetComponent<CameraControl>();
		controller = GetComponent<CharacterController>();

		currentPlatformIndex = 0;
	}

	void Update() 
	{
        // Movement Methods
		CheckPlayerMovement();
		CheckShadowshift();

		if(shiftingOut && !CameraControl.cameraIsPanning)
		{
			ShiftingOutControl();
		}
	}

	void CheckPlayerMovement()
	{
		if(!shiftingIn && !shiftingOut && !isGrabbing)
			PlayerJumpandGravity();
		
		if(in3DSpace && !shiftingIn && !shiftingOut)
			PlayerMovement3D();

		else if(!in3DSpace && !shiftingIn && !shiftingOut)
		{
			PlayerMovement2D();
			FollowShadow();
		}
	}

	void CheckShadowshift()
	{
		// Shadowshift Master Control
		if(!isGrabbing && !shiftingIn && !shiftingOut)
		{
			if(Input.GetButtonDown("Fire3"))
			{
				if(in3DSpace)
					StartShadowShiftIn();
				else
				{
					StartShadowShiftOut();
				}
			}
		}
	}

    public void PlayerJumpandGravity()
    {
		if(grounded)
		{
			if(Input.GetButtonDown("Jump"))
			{
				verticalVelocity = jumpSpeed;
			}
		}
		else
		{
			verticalVelocity -= gravity * Time.deltaTime;
		}
		Vector3 moveVector = new Vector3(0, verticalVelocity, 0);
		controller.Move(moveVector * Time.deltaTime);
//		if (Input.GetButton("Jump") && jumpHeldTime < jumpTime)
//        {
//			if (jumpSpeedCurrent <= ((jumpSpeed - gravity) / jumpTime) * Time.deltaTime)
//				jumpSpeedCurrent = 0;
//			else jumpSpeedCurrent -= ((jumpSpeed - gravity) / jumpTime) * Time.deltaTime;
//			controller.Move(new Vector3(0, jumpSpeedCurrent * Time.deltaTime, 0));
//            jumpHeldTime += Time.deltaTime;
//        }
//
//		controller.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
//
//        RaycastHit hit;
//        if (in3DSpace)
//        {
//			if (Physics.Raycast (new Vector3 (transform.position.x, transform.position.y - (transform.lossyScale.y), transform.position.z), -Vector3.up, out hit, .1f)) 
//			{
//				jumpHeldTime = 0;
//				jumpSpeedCurrent = jumpSpeed;
//			}
//        }
//        else
//        {
//            Debug.DrawLine(playerShadow.transform.position + Vector3.down * transform.lossyScale.y, playerShadow.transform.position + Vector3.down * (transform.lossyScale.y+.1f), Color.red, .25f);
//            if (Physics.Raycast(new Vector3(playerShadow.transform.position.x, playerShadow.transform.position.y - (playerShadow.transform.lossyScale.y), playerShadow.transform.position.z), -Vector3.up, out hit, .1f))
//			{
//				jumpHeldTime = 0;
//				jumpSpeedCurrent = jumpSpeed;
//			}
//        }
	}

	public void PlayerMovement2D()
	{
		if (Input.GetAxisRaw("Horizontal") > 0)
		{
			controller.Move(-playerShadow.transform.right * Time.deltaTime * movementSpeed);
		}
		if (Input.GetAxisRaw("Horizontal") < 0)
		{
			controller.Move(playerShadow.transform.right * Time.deltaTime * movementSpeed);
		}

	}

	public void PlayerMovement3D()
	{
		if(!isGrabbing)
		{
			rotationDirection = new Vector3(movementReference.transform.forward.x, 0, movementReference.transform.forward.z);
			transform.rotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
			if (Input.GetAxisRaw("Horizontal") > 0)
			{
				controller.Move(movementReference.transform.right * Time.deltaTime * movementSpeed);
			}
			if (Input.GetAxisRaw("Horizontal") < 0)
			{
				controller.Move(-movementReference.transform.right * Time.deltaTime * movementSpeed);
			}
			if (Input.GetAxisRaw("Vertical") > 0)
			{
				controller.Move(movementReference.transform.forward * Time.deltaTime * movementSpeed);
			}
			if (Input.GetAxisRaw("Vertical") < 0)
			{
				controller.Move(-movementReference.transform.forward * Time.deltaTime * movementSpeed);
			}
		}
		// Grabbing movement
		else
		{
			if (Input.GetAxisRaw("Vertical") > 0 && !grabbedObject.GetComponent<MoveCube>().blockedAhead)
			{
				controller.Move(grabbedObject.GetComponent<MoveCube>().directionAwayFromPlayer * Time.deltaTime * grabMovementSpeed);
			}
			if (Input.GetAxisRaw("Vertical") < 0)
			{
				controller.Move(-grabbedObject.GetComponent<MoveCube>().directionAwayFromPlayer * Time.deltaTime * grabMovementSpeed);
			}
		}
	}

	public void FollowShadow()
	{
		transform.position = playerShadow.transform.position + distanceFromShadow;
	}

	public void StartShadowShiftIn()
	{
		RaycastHit hit;
		// Cast a ray in the direction of the light source and see if it hits a shadow wall

		if (Physics.SphereCast(transform.position, 0.1f, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity))
		{
			if(hit.collider.gameObject.tag == "Shadow Wall")
			{
				// If so, cast a ray offset 0.2 upwards and downwards in the y and see if it collides with any shadow colliders
				// behind the wall. If not, shift the player into the wall

				if(!Physics.Raycast(hit.point + new Vector3(0, transform.lossyScale.y * 1/3, 0), LightSourceControl.lightSourceDirection, 1f, 1 << 11) && 
					!Physics.Raycast(hit.point - new Vector3(0,transform.lossyScale.y * 2/3, 0), LightSourceControl.lightSourceDirection, 1f, 1 << 11) && 
					!Physics.Raycast(hit.point, LightSourceControl.lightSourceDirection, 1f, 1 << 11))
				{
					StartCoroutine(CameraPanIn(transform.position, hit.point, -LightSourceControl.lightSourceDirection * camControl.distanceToPlayer2D));
					StartCoroutine(FinishShiftIn());
				}
				else
					Debug.Log("You can't transfer, there is a shadow in the way!");
			}
			else
				Debug.Log("You can't transfer, you didn't hit a shadow wall!");
		}
		
	}

	public void StartShadowShiftOut()
	{
		ShiftOutSetup();
		if(transferPlatforms.Count != 0)
		{
			if(transferPlatforms.Count == 1)
			{
				if(LightSourceControl.zAxisMovement)
					StartCoroutine(CameraPanOut(playerShadow.transform.position, new Vector3(playerShadow.transform.position.x, 
						(transferPlatforms[0].transform.position.y + transferPlatforms[0].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						transferPlatforms[0].transform.position.z), true));
				else if(LightSourceControl.xAxisMovement)
					StartCoroutine(CameraPanOut(playerShadow.transform.position, new Vector3(transferPlatforms[0].transform.position.x, 
						(transferPlatforms[0].transform.position.y + transferPlatforms[0].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						playerShadow.transform.position.z), true));
			}
			else
			{
				if(LightSourceControl.zAxisMovement)
				{
					StartCoroutine(CameraPanOut(playerShadow.transform.position, new Vector3(playerShadow.transform.position.x, 
						(transferPlatforms[0].transform.position.y + transferPlatforms[0].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						transferPlatforms[0].transform.position.z), false));
				}
				else if(LightSourceControl.xAxisMovement)
					StartCoroutine(CameraPanOut(playerShadow.transform.position, new Vector3(transferPlatforms[0].transform.position.x, 
						(transferPlatforms[0].transform.position.y + transferPlatforms[0].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						playerShadow.transform.position.z), false));
			}
		}

		else
		{
			StartCoroutine(CameraPanOut(playerShadow.transform.position, transform.position, true));
		}
	}

	public void ShiftingOutControl()
	{
		if(Input.GetKeyDown(KeyCode.S))
		{
			if(currentPlatformIndex + 1 < transferPlatforms.Count)
			{
				currentPlatformIndex += 1;

				if(LightSourceControl.zAxisMovement)
					StartCoroutine(CameraPanOut(transitionOutFollow.transform.position, new Vector3(playerShadow.transform.position.x, 
						(transferPlatforms[currentPlatformIndex].transform.position.y + transferPlatforms[currentPlatformIndex].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						transferPlatforms[currentPlatformIndex].transform.position.z), false));
				else if(LightSourceControl.xAxisMovement)
					StartCoroutine(CameraPanOut(transitionOutFollow.transform.position, new Vector3(transferPlatforms[currentPlatformIndex].transform.position.x, 
						(transferPlatforms[currentPlatformIndex].transform.position.y + transferPlatforms[currentPlatformIndex].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						playerShadow.transform.position.z), false));
			}
		}

		if(Input.GetKeyDown(KeyCode.W))
		{
			if(currentPlatformIndex - 1 >= 0)
			{
				currentPlatformIndex -= 1;

				if(LightSourceControl.zAxisMovement)
					StartCoroutine(CameraPanOut(transitionOutFollow.transform.position, new Vector3(playerShadow.transform.position.x, 
						(transferPlatforms[currentPlatformIndex].transform.position.y + transferPlatforms[currentPlatformIndex].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						transferPlatforms[currentPlatformIndex].transform.position.z), false));
				else if(LightSourceControl.xAxisMovement)
					StartCoroutine(CameraPanOut(transitionOutFollow.transform.position, new Vector3(transferPlatforms[currentPlatformIndex].transform.position.x, 
						(transferPlatforms[currentPlatformIndex].transform.position.y + transferPlatforms[currentPlatformIndex].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						playerShadow.transform.position.z), false));
			}
		}

		if(Input.GetButtonDown("Fire3"))
		{
			FinishShiftOut();
		}

		if(Input.GetButtonDown("Cancel"))
		{
			transitionInFollow = transitionOutFollow;
			transitionOutFollow = null;
			StartCoroutine(CameraPanIn(transitionInFollow.transform.position, playerShadow.transform.position, -LightSourceControl.lightSourceDirection * camControl.distanceToPlayer2D));
			StartCoroutine(FinishShiftInMultiExit());
		}
	}

	public IEnumerator CameraPanIn(Vector3 start, Vector3 target, Vector3 offset)
	{
		shiftingIn = true;
		CameraControl.cameraIsPanning = true;

		if(transitionInFollow == null)
			transitionInFollow = Instantiate(transitionFollowPrefab, start, transitionFollowPrefab.transform.rotation);

		// Camera transition inwards to the targeted location
		cameraPanInStartPos = camera.transform.position;
		cameraRelativeDirectionOffset = (cameraPanInStartPos - transform.position).normalized;

		float panStart = Time.time;
		while(Time.time < panStart + camControl.cameraPanDuration)
		{
			transitionInFollow.transform.position = Vector3.Lerp(start, target, (Time.time - panStart)/camControl.cameraPanDuration);
			camera.transform.position = Vector3.Lerp(cameraPanInStartPos, target + offset, (Time.time - panStart)/camControl.cameraPanDuration);
			yield return null;
		}
		CameraControl.cameraIsPanning = false;
		Destroy(transitionInFollow);
	}

	public IEnumerator CameraPanOut(Vector3 start, Vector3 target, bool finishing)
	{
		shiftingOut = true;
		CameraControl.cameraIsPanning = true;

		if(transitionOutFollow == null)
			transitionOutFollow = Instantiate(transitionFollowPrefab, start, transitionFollowPrefab.transform.rotation);

		// Camera transition outwards from the wall
		Vector3 startPos = camera.transform.position;
		float panStart = Time.time;
		while(Time.time < panStart + camControl.cameraPanDuration)
		{
			transitionOutFollow.transform.position = Vector3.Lerp(start, target, (Time.time - panStart)/camControl.cameraPanDuration);
			camera.transform.position = Vector3.Lerp(startPos, target + cameraRelativeDirectionOffset * camControl.distanceToPlayer3D, (Time.time - panStart)/camControl.cameraPanDuration);

			yield return null;
		}

		CameraControl.cameraIsPanning = false;
		if(finishing)
			FinishShiftOut();
	}

	public IEnumerator FinishShiftIn()
	{
		yield return new WaitForSeconds(camControl.cameraPanDuration);

		shiftingIn = false;
		transform.parent = null;
		playerShadow.transform.parent = null;
		in3DSpace = false;
		GetComponent<CharacterController>().enabled = false;
		playerShadow.GetComponent<CharacterController>().enabled = true;
		controller = playerShadow.GetComponent<CharacterController>();
		GetComponent<PlayerShadowCast>().CastShadow();
		distanceFromShadow = transform.position - playerShadow.transform.position;
		Debug.Log("In Wall");
	}

	public IEnumerator FinishShiftInMultiExit()
	{
		Destroy(transitionOutFollow);
		yield return new WaitForSeconds(camControl.cameraPanDuration);

		currentPlatformIndex = 0;
		shiftingIn = false;
		shiftingOut = false;
		transform.parent = null;
		playerShadow.transform.parent = null;
		in3DSpace = false;
		GetComponent<CharacterController>().enabled = false;
		playerShadow.GetComponent<CharacterController>().enabled = true;
		controller = playerShadow.GetComponent<CharacterController>();
		GetComponent<PlayerShadowCast>().CastShadow();
		distanceFromShadow = transform.position - playerShadow.transform.position;
		Debug.Log("In Wall");
	}
		
	public void FinishShiftOut()
	{
		currentPlatformIndex = 0;
		shiftingOut = false;
		transform.position = transitionOutFollow.transform.position;
		Destroy(transitionOutFollow);

		transform.parent = null;
		playerShadow.transform.parent = null;
		in3DSpace = true;
		playerShadow.GetComponent<CharacterController>().enabled = false;
		GetComponent<CharacterController>().enabled = true;
		controller = GetComponent<CharacterController>();
		Debug.Log("Out of wall");
	}

	public void ShiftOutSetup()
	{
		// Get a list of gameobjects from the PlayerShadowCollider script on the player shadow
		transferPlatforms = playerShadow.GetComponent<PlayerShadowCollider>().GetTransferPlatforms();

		// If there are platforms to which the player can transfer out to, sort the platforms by their distance from the player
		// and then transfer the player character to the closest one
		if(transferPlatforms.Count != 0)
		{
			transferPlatforms.Sort(delegate(GameObject t1, GameObject t2) {
				return Vector3.Distance(t1.transform.position, playerShadow.transform.position).CompareTo(Vector3.Distance(t2.transform.position, playerShadow.transform.position));
			});
		}
	}
}

