﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

--Player Movement--
    Basic master control for the Player_Character and Player_Shadow prefab, handles
    all logic for player movement, shadow shifting, shadowmeld, and physics. Attached
    to the Player_Character prefab at the highest level.

*/

public class PlayerMovement : MonoBehaviour
{
    // Mostly public variables, easily modified
    // by designers to manipulate game systems
    [Header("Movement Variables")]
    public float movementSpeed;
    public float grabMovementSpeed;
    public float jumpSpeed;
    public float gravity;
    private float verticalVelocity;

    [Header("Shadow Shift Variables")]
    public GameObject shadowShiftFollowObjectPrefab;
    public float shadowShiftPanDuration;
    [HideInInspector]
    public GameObject shadowShiftFollowObject;

    [Header("Shadowmeld Variables")]
    public bool shadowMeldAvailable;
    public GameObject shadowMeldVFX;
    public float shadowMeldResourceCost;
    public float shadowMeldResourceRegen;
    public float shadowMeldResource;

    // Static Variables/Object references
    public static Vector3 playerStartPosition;
    public static Vector3 levelStartPosition;
    public static bool in3DSpace;
    public static bool grounded2D;
    public static bool grounded3D;
    public static bool shadowShiftingOut;
    public static bool shadowShiftingIn;
    public static bool shadowMelded;
    public static bool isGrabbing;
    public static GameObject grabbedObject;

    // Various object references, all are set at start
    // or through other places in code.
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    public GameObject mainCamera;
    private GameObject playerShadow;
    private GameObject movementReference;
    private List<GameObject> shadowShiftOutPlatforms;
    private GameController gameController;

    // Various private variables used in shadow shift
    // enter and exit
    private Vector3 rotationDirection;
    private float playerShiftInOffset;
    private Vector3 cameraPanInStartPos;
    private Vector3 cameraRelativeDirectionOffset;
    private float cameraPanInRelativeDistance;
    private int currentPlatformIndex;

    void Start()
    {
        playerStartPosition = transform.position;
		levelStartPosition = transform.position;
        in3DSpace = true;
        shadowShiftingOut = false;
        shadowShiftingIn = false;
        shadowMelded = false;
        isGrabbing = false;

        movementReference = GetComponentInChildren<MovementReference>().gameObject;
        playerShadow = GameObject.Find("Player_Shadow");
        gameController = GameObject.Find("Game_Controller").GetComponent<GameController>();
        controller = GetComponent<CharacterController>();

        currentPlatformIndex = 0;
    }

    // Update handles the calling of various checks for player system like
    // movement, shadowshift, and shadowmeld system, all following boolean
    // flow control based on various player or game states, like shadow
    // shifting in, out, and where the player is currently located (3D or 2D)
    void Update()
    {
        if (!GameController.resetting)
        {
            CheckPlayerMovement();
            CheckShadowshift();
            if (shadowMeldAvailable)
                CheckShadowMeld();
			CheckMenuAndReset();
        }
    }
    #region Game Control Methods
    void CheckMenuAndReset()
    {
        if (Input.GetButtonDown("Reset") && !shadowShiftingIn && !shadowShiftingOut)
        {
            playerStartPosition = levelStartPosition;
            foreach (Transform child in GameObject.Find("Lighting").transform)
            {
                LightSourceControl light = child.GetComponent<LightSourceControl>();
                child.rotation = light.lightSourceStartRotation;
                light.lightSourceDirection = child.transform.forward;
                light.CheckLightingDirection();
            }
            StartCoroutine(GameObject.Find("Game_Controller").GetComponent<GameController>().ResetLevel());
        }
    }
    #endregion

    #region Player Movement Methods
    // As mentioned above, handles player movement method calling based 
    // on various boolean logical flow values.
    void CheckPlayerMovement()
    {
        if (!shadowShiftingIn && !shadowShiftingOut && !isGrabbing)
        {
            PlayerJumpandGravity();
        }
    
        if (in3DSpace && !shadowShiftingIn && !shadowShiftingOut)
        {
            PlayerMovement3D();
        }

        else if (!in3DSpace && !shadowShiftingIn && !shadowShiftingOut)
        {
            GameController.Toggle2DMovementTooltips(true);
            PlayerMovement2D();
            FollowShadow();
        }
    }

    public void PlayerJumpandGravity()
    {
        if ((in3DSpace && grounded3D) || (!in3DSpace && grounded2D))
        {
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpSpeed;
            }
        }
        verticalVelocity -= gravity * Time.deltaTime;
        if (verticalVelocity < -gravity * .75f) verticalVelocity = -gravity * .75f;
        Vector3 moveVector = new Vector3(0, verticalVelocity, 0);
        controller.Move(moveVector * Time.deltaTime);
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
        if (!isGrabbing)
        {
            GameController.Toggle3DMovementTooltips(true);
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
            GameController.ToggleGrabbingTooltips(true);
            if (Input.GetAxisRaw("Vertical") > 0 && !grabbedObject.GetComponent<PushCube>().blockedAhead)
            {
                controller.Move(grabbedObject.GetComponent<PushCube>().directionAwayFromPlayer * Time.deltaTime * grabMovementSpeed);
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                controller.Move(-grabbedObject.GetComponent<PushCube>().directionAwayFromPlayer * Time.deltaTime * grabMovementSpeed);
            }
        }
    }
    #endregion

    #region Shadowmeld Methods
    // Handles checking Shadowmeld input, and calling various subsequent methods
    // that deal with shadowmelding, 
    void CheckShadowMeld()
    {
        if (in3DSpace && !isGrabbing && !shadowShiftingIn && !shadowShiftingOut)
        {
			GameController.CheckShadowMeldTooltip(true);
            if (Input.GetButtonDown("Shadowmeld") && shadowMeldResource > 0)
            {
                if (!shadowMelded)
                {
                    EnterShadowMeld();
                }
                else
                {
                    CheckShadowMeldExit();
                }
            }
        }
        if (shadowMelded)
        {
            if(shadowMeldResource > 0)
                shadowMeldResource -= shadowMeldResourceCost * Time.deltaTime;

            if (shadowMeldResource <= 0)
            {
                CheckShadowMeldExit();
            }
        }
        else
        {
            if(shadowMeldResource < 100)
                shadowMeldResource += shadowMeldResourceRegen * Time.deltaTime;
        }
    }

    void EnterShadowMeld()
    {
        Debug.Log("Shadowmelding");
        shadowMelded = true;
        shadowMeldVFX.SetActive(true);
        GameController.CheckInteractToolip(false);
        gameObject.layer = LayerMask.NameToLayer("Shadowmeld");

    }

    void CheckShadowMeldExit()
    {
        Collider[] collidingObjects = Physics.OverlapCapsule(new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z),
            new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), 0.5f);

        foreach (Collider collideObj in collidingObjects)
        {
            if (collideObj.tag != "Player")
            {
                switch (collideObj.GetComponent<ShadowmeldObjectControl>().shadowMeldObjectType)
                {
                    case ShadowmeldObjectControl.ShadowMeldObjectType.GLASS:
                        StartCoroutine(gameController.ResetLevel());
                        ExitShadowMeld();
                        break;
                    case ShadowmeldObjectControl.ShadowMeldObjectType.WATER:
                        StartCoroutine(gameController.ResetLevel());
                        ExitShadowMeld();
                        break;
                    default:
                        ExitShadowMeld();
                        break;
                }
            }
            else
            {
                ExitShadowMeld();
            }
        }
    }

    public void ExitShadowMeld()
    {
        Debug.Log("Leaving shadowmelded");
        shadowMelded = false;
        shadowMeldVFX.SetActive(false);
        gameObject.layer = LayerMask.NameToLayer("Form");
    }
    #endregion

    #region Follow Methods
    public void FollowShadow()
    {
        transform.position = playerShadow.transform.position + -GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection * 1.5f;
    }

    public void FollowTransitionObject()
    {
        transform.position = shadowShiftFollowObject.transform.position;
    }
    #endregion

    #region Shadowshift Methods
    // Handles checking Shadowshift input, and calling various subsequent methods
    // that deal with shifting in (both vector math and animations/VFX) and shifting out
    void CheckShadowshift()
    {
        if (!shadowMelded && !isGrabbing && !shadowShiftingIn && !shadowShiftingOut)
        {
            if (Input.GetButtonDown("Shadowshift"))
            {
                if (in3DSpace)
                    StartShadowShiftIn();
                else
                {
                    StartShadowShiftOut();
                }
            }
        }
        if (shadowShiftingOut && !CameraControl.cameraIsPanning)
        {
            ShiftingOutControl();
        }

        if (shadowShiftingOut)
            FollowTransitionObject();
    }

    public void StartShadowShiftIn()
	{
		RaycastHit hit;
		// Cast a ray in the direction of the light source and see if it hits a shadow wall

		if (Physics.SphereCast(transform.position, 0.1f, GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection, out hit, Mathf.Infinity))
		{
			if(hit.collider.gameObject.tag == "Shadow Wall")
			{
				// If so, cast a ray offset 0.2 upwards and downwards in the y and see if it collides with any shadow colliders
				// behind the wall. If not, shift the player into the wall
				if(!Physics.Raycast(hit.point + new Vector3(0, transform.lossyScale.y * 1/3, 0), GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection, 1f, 1 << 11) && 
					!Physics.Raycast(hit.point - new Vector3(0,transform.lossyScale.y * 2/3, 0), GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection, 1f, 1 << 11) && 
					!Physics.Raycast(hit.point, GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection, 1f, 1 << 11))
				{
                    if (GetComponent<PlayerShadowCast>().lightSourceAligned.zAxisMovement)
                        playerShiftInOffset = transform.position.z;
                    else if (GetComponent<PlayerShadowCast>().lightSourceAligned.xAxisMovement)
                        playerShiftInOffset = transform.position.x;
                    GetComponent<CharacterController>().enabled = false;

                    StartCoroutine(CameraPanIn(transform.position, hit.point, -GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection * mainCamera.GetComponent<CameraControl>().distanceToPlayer2D));
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
		if(shadowShiftOutPlatforms.Count != 0)
		{
			if(shadowShiftOutPlatforms.Count == 1)
			{
				if(GetComponent<PlayerShadowCast>().lightSourceAligned.zAxisMovement)
					StartCoroutine(CameraPanOut(playerShadow.transform.position, new Vector3(playerShadow.transform.position.x, 
						(shadowShiftOutPlatforms[0].transform.position.y + shadowShiftOutPlatforms[0].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						shadowShiftOutPlatforms[0].transform.position.z), true));
				else if(GetComponent<PlayerShadowCast>().lightSourceAligned.xAxisMovement)
					StartCoroutine(CameraPanOut(playerShadow.transform.position, new Vector3(shadowShiftOutPlatforms[0].transform.position.x, 
						(shadowShiftOutPlatforms[0].transform.position.y + shadowShiftOutPlatforms[0].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						playerShadow.transform.position.z), true));
			}
			else
			{
				if(GetComponent<PlayerShadowCast>().lightSourceAligned.zAxisMovement)
					StartCoroutine(CameraPanOut(playerShadow.transform.position, new Vector3(playerShadow.transform.position.x, 
						(shadowShiftOutPlatforms[0].transform.position.y + shadowShiftOutPlatforms[0].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						shadowShiftOutPlatforms[0].transform.position.z), false));
				else if(GetComponent<PlayerShadowCast>().lightSourceAligned.xAxisMovement)
					StartCoroutine(CameraPanOut(playerShadow.transform.position, new Vector3(shadowShiftOutPlatforms[0].transform.position.x, 
						(shadowShiftOutPlatforms[0].transform.position.y + shadowShiftOutPlatforms[0].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						playerShadow.transform.position.z), false));
			}
		}

		else
		{
            if (GetComponent<PlayerShadowCast>().lightSourceAligned.zAxisMovement)
                StartCoroutine(CameraPanOut(playerShadow.transform.position, new Vector3(playerShadow.transform.position.x, playerShadow.transform.position.y, playerShiftInOffset), true));
            else if (GetComponent<PlayerShadowCast>().lightSourceAligned.xAxisMovement)
                StartCoroutine(CameraPanOut(playerShadow.transform.position, new Vector3(playerShiftInOffset, playerShadow.transform.position.y, playerShadow.transform.position.z), true));
		}
	}

	public void ShiftingOutControl()
	{
        GameController.ToggleShadowShiftOutTooltips(true);
		if(Input.GetKeyDown(KeyCode.S))
		{
			if(currentPlatformIndex + 1 < shadowShiftOutPlatforms.Count)
			{
				currentPlatformIndex += 1;

				if(GetComponent<PlayerShadowCast>().lightSourceAligned.zAxisMovement)
					StartCoroutine(CameraPanOut(shadowShiftFollowObject.transform.position, new Vector3(playerShadow.transform.position.x, 
						(shadowShiftOutPlatforms[currentPlatformIndex].transform.position.y + shadowShiftOutPlatforms[currentPlatformIndex].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						shadowShiftOutPlatforms[currentPlatformIndex].transform.position.z), false));
				else if(GetComponent<PlayerShadowCast>().lightSourceAligned.xAxisMovement)
					StartCoroutine(CameraPanOut(shadowShiftFollowObject.transform.position, new Vector3(shadowShiftOutPlatforms[currentPlatformIndex].transform.position.x, 
						(shadowShiftOutPlatforms[currentPlatformIndex].transform.position.y + shadowShiftOutPlatforms[currentPlatformIndex].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						playerShadow.transform.position.z), false));
			}
		}

		if(Input.GetKeyDown(KeyCode.W))
		{
			if(currentPlatformIndex - 1 >= 0)
			{
				currentPlatformIndex -= 1;

				if(GetComponent<PlayerShadowCast>().lightSourceAligned.zAxisMovement)
					StartCoroutine(CameraPanOut(shadowShiftFollowObject.transform.position, new Vector3(playerShadow.transform.position.x, 
						(shadowShiftOutPlatforms[currentPlatformIndex].transform.position.y + shadowShiftOutPlatforms[currentPlatformIndex].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						shadowShiftOutPlatforms[currentPlatformIndex].transform.position.z), false));
				else if(GetComponent<PlayerShadowCast>().lightSourceAligned.xAxisMovement)
					StartCoroutine(CameraPanOut(shadowShiftFollowObject.transform.position, new Vector3(shadowShiftOutPlatforms[currentPlatformIndex].transform.position.x, 
						(shadowShiftOutPlatforms[currentPlatformIndex].transform.position.y + shadowShiftOutPlatforms[currentPlatformIndex].transform.lossyScale.y / 2 + transform.lossyScale.y), 
						playerShadow.transform.position.z), false));
			}
		}

		if(Input.GetButtonDown("Shadowshift"))
		{
			FinishShiftOut();
		}

		if(Input.GetButtonDown("Cancel"))
		{
			StartCoroutine(CameraPanInMultiExit(shadowShiftFollowObject.transform.position, playerShadow.transform.position, -GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection * mainCamera.GetComponent<CameraControl>().distanceToPlayer2D));
			StartCoroutine(FinishShiftInMultiExit());
		}
	}

	public IEnumerator CameraPanIn(Vector3 start, Vector3 target, Vector3 offset)
	{
		shadowShiftingIn = true;
		CameraControl.cameraIsPanning = true;

		if(shadowShiftFollowObject == null)
			shadowShiftFollowObject = Instantiate(shadowShiftFollowObjectPrefab, start, shadowShiftFollowObjectPrefab.transform.rotation);

		// Camera transition inwards to the targeted location
		cameraPanInStartPos = mainCamera.transform.position;
		cameraRelativeDirectionOffset = (cameraPanInStartPos - transform.position).normalized;

		float panStart = Time.time;
		while(Time.time < panStart + shadowShiftPanDuration)
		{
			shadowShiftFollowObject.transform.position = Vector3.Lerp(start, target, (Time.time - panStart)/shadowShiftPanDuration);
			mainCamera.transform.position = Vector3.Lerp(cameraPanInStartPos, target + offset, (Time.time - panStart)/shadowShiftPanDuration);
			yield return null;
		}
		CameraControl.cameraIsPanning = false;
		Destroy(shadowShiftFollowObject);
	}

	public IEnumerator CameraPanOut(Vector3 start, Vector3 target, bool finishing)
	{
		shadowShiftingOut = true;
		CameraControl.cameraIsPanning = true;

		if(shadowShiftFollowObject == null)
			shadowShiftFollowObject = Instantiate(shadowShiftFollowObjectPrefab, start, shadowShiftFollowObjectPrefab.transform.rotation);

		// Camera transition outwards from the wall
		Vector3 startPos = mainCamera.transform.position;
		float panStart = Time.time;
		while(Time.time < panStart + shadowShiftPanDuration)
		{
			shadowShiftFollowObject.transform.position = Vector3.Lerp(start, target, (Time.time - panStart)/shadowShiftPanDuration);
			mainCamera.transform.position = Vector3.Lerp(startPos, target + cameraRelativeDirectionOffset * mainCamera.GetComponent<CameraControl>().distanceToPlayer3D, (Time.time - panStart)/shadowShiftPanDuration);

			yield return null;
		}

		CameraControl.cameraIsPanning = false;
		if(finishing)
			FinishShiftOut();
	}

    public IEnumerator CameraPanInMultiExit(Vector3 start, Vector3 target, Vector3 offset)
    {
        CameraControl.cameraIsPanning = true;

        if (shadowShiftFollowObject == null)
            shadowShiftFollowObject = Instantiate(shadowShiftFollowObjectPrefab, start, shadowShiftFollowObjectPrefab.transform.rotation);
        Vector3 startPos = mainCamera.transform.position;

        float panStart = Time.time;
        while (Time.time < panStart + shadowShiftPanDuration)
        {
            shadowShiftFollowObject.transform.position = Vector3.Lerp(start, target, (Time.time - panStart) / shadowShiftPanDuration);
            mainCamera.transform.position = Vector3.Lerp(startPos, target + offset, (Time.time - panStart) / shadowShiftPanDuration);
            yield return null;
        }
        CameraControl.cameraIsPanning = false;
    }

    public IEnumerator FinishShiftIn()
	{
		yield return new WaitForSeconds(shadowShiftPanDuration);

		shadowShiftingIn = false;
        in3DSpace = false;
        transform.parent = null;
		playerShadow.transform.parent = null;
		playerShadow.GetComponent<CharacterController>().enabled = true;
		controller = playerShadow.GetComponent<CharacterController>();
        GetComponent<PlayerShadowCast>().CastShadow();
        Debug.Log("In Wall");
	}

	public IEnumerator FinishShiftInMultiExit()
	{
		yield return new WaitForSeconds(shadowShiftPanDuration);
        Destroy(shadowShiftFollowObject);
		currentPlatformIndex = 0;
		shadowShiftingOut = false;

		transform.parent = null;
		playerShadow.transform.parent = null;
		in3DSpace = false;
		GetComponent<CharacterController>().enabled = false;
		playerShadow.GetComponent<CharacterController>().enabled = true;
		controller = playerShadow.GetComponent<CharacterController>();
		GetComponent<PlayerShadowCast>().CastShadow();
        GameController.ToggleShadowShiftOutTooltips(false);
        Debug.Log("In Wall");
	}
		
	public void FinishShiftOut()
	{
		currentPlatformIndex = 0;
		shadowShiftingOut = false;
		transform.position = shadowShiftFollowObject.transform.position;
		Destroy(shadowShiftFollowObject);

		transform.parent = null;
		playerShadow.transform.parent = null;
		in3DSpace = true;
		playerShadow.GetComponent<CharacterController>().enabled = false;
		GetComponent<CharacterController>().enabled = true;
		controller = GetComponent<CharacterController>();
        GameController.ToggleShadowShiftOutTooltips(false);
		Debug.Log("Out of wall");
	}

	public void ShiftOutSetup()
	{
		// Get a list of gameobjects from the PlayerShadowCollider script on the player shadow
		shadowShiftOutPlatforms = playerShadow.GetComponent<PlayerShadowCollider>().GetTransferPlatforms();

		// If there are platforms to which the player can transfer out to, sort the platforms by their distance from the player
		// and then transfer the player character to the closest one
		if(shadowShiftOutPlatforms.Count != 0)
		{
			shadowShiftOutPlatforms.Sort(delegate(GameObject t1, GameObject t2) {
				return Vector3.Distance(t1.transform.position, playerShadow.transform.position).CompareTo(Vector3.Distance(t2.transform.position, playerShadow.transform.position));
			});
		}
	}
    #endregion
}