using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/*

--Player Movement--
    Basic master control for the Player_Character and Player_Shadow prefab, handles
    all logic for player movement, shadow shifting, shadowmeld, and physics. Attached
    to the Player_Character prefab at the highest level.

*/

public class PlayerShadowInteraction : MonoBehaviour
{
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

    [HideInInspector]
    public AudioSource playerAudioSource;

    [Header("Sound Variables")]
    public AudioClip jumpLiftClip;
    public AudioClip jumpLandClip;
    public AudioClip[] walkAudioClips;
    public AudioClip shadowMeldInAudioClip;
    public AudioClip shadowMeldOutAudioClip;
    public AudioClip shadowShiftFailAudioClip;

    private AudioClip previousWalkAudioClip;
    public float walkStepFrequency;
    private float walkStepStartTime;

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
    public GameObject playerShadow;
    private List<GameObject> shadowShiftOutPlatforms;
    private GameController gameController;

    // Various private variables used in shadow shift
    // enter and exit
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

        playerAudioSource = GetComponent<AudioSource>();
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
        if (!GameController.resetting && !GameController.paused)
        {
            CheckShadowshift();
            if (shadowMeldAvailable)
                CheckShadowMeld();
        }
    }

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
                    if (!GameController.shadowMeld_First_Time_Used)
                        GameController.shadowMeld_First_Time_Used = true;
                    GameController.CheckShadowMeldTooltip(false);
                    EnterShadowMeld();
                }
                else
                {
                    CheckShadowMeldExit();
                }
            }
        }
        else
        {
            GameController.CheckShadowMeldTooltip(false);
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
        GetComponent<AudioSource>().clip = shadowMeldInAudioClip;
        GetComponent<AudioSource>().Play();
        shadowMelded = true;
        shadowMeldVFX.SetActive(true);
        GameController.CheckInteractToolip(false, false);
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
                        StartCoroutine(gameController.ResetLevel(false, false));
                        ExitShadowMeld();
                        break;
                    case ShadowmeldObjectControl.ShadowMeldObjectType.WATER:
                        StartCoroutine(gameController.ResetLevel(false, true));
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
        GetComponent<AudioSource>().clip = shadowMeldOutAudioClip;
        GetComponent<AudioSource>().Play();
        shadowMelded = false;
        shadowMeldVFX.SetActive(false);
        gameObject.layer = LayerMask.NameToLayer("Form");
    }
    #endregion

    #region Shadowshift Methods
    // Handles checking Shadowshift input, and calling various subsequent methods
    // that deal with shifting in (both vector math and animations/VFX) and shifting out
    void CheckShadowshift()
    {
        if (Input.GetButtonDown("Shadowshift"))
        {
            if (!shadowMelded && !isGrabbing && !shadowShiftingIn && !shadowShiftingOut)
            {
                if (in3DSpace)
                {
                    StartShadowShiftIn();
                }
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
    }

    public void StartShadowShiftIn()
	{
		RaycastHit hit;
		// Cast a ray in the direction of the light source and see if it hits a shadow wall

		if (Physics.SphereCast(transform.position, 0.1f, 
            GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection, out hit, 1 << 11))
		{
			if(hit.collider.gameObject.tag == "Shadow Wall")
			{
				// If so, cast a ray offset 0.2 upwards and downwards in the y and see if it collides with any shadow colliders
				// behind the wall. If not, shift the player into the wall
				if(!Physics.Raycast(hit.point + new Vector3(0, transform.lossyScale.y * 1/3, 0), 
                    GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection, 1f, 1 << 11) && 
					!Physics.Raycast(hit.point - new Vector3(0,transform.lossyScale.y * 2/3, 0), 
                    GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection, 1f, 1 << 11) && 
					!Physics.Raycast(hit.point, 
                    GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection, 1f, 1 << 11))
				{
                    if (GetComponent<PlayerShadowCast>().lightSourceAligned.zAxisMovement)
                        playerShiftInOffset = transform.position.z;
                    else if (GetComponent<PlayerShadowCast>().lightSourceAligned.xAxisMovement)
                        playerShiftInOffset = transform.position.x;


                    StartCoroutine(CameraPanIn(transform.position, hit.point, 
                        -GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection * Camera.main.GetComponent<CameraControl>().distanceToPlayer2D));
					StartCoroutine(FinishShiftIn());
				}
				else
                {
                    GetComponent<AudioSource>().clip = shadowShiftFailAudioClip;
                    GetComponent<AudioSource>().Play();
                    Debug.Log("You can't transfer, there is a shadow in the way!");
                }
            }
			else
            {
                Debug.Log("You can't transfer, you didn't hit a shadow wall!");
                GetComponent<AudioSource>().clip = shadowShiftFailAudioClip;
                GetComponent<AudioSource>().Play();
            }
		}
        else
        {
            GetComponent<AudioSource>().clip = shadowShiftFailAudioClip;
            GetComponent<AudioSource>().Play();
        }
	}

	public void StartShadowShiftOut()
	{
		ShiftOutSetup();

        GetComponent<AudioSource>().Play();

        if (shadowShiftOutPlatforms.Count != 0)
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
            if(currentPlatformIndex != 0)
            {
                if (currentPlatformIndex - 1 >= 0)
                {
                    currentPlatformIndex -= 1;

                    if (GetComponent<PlayerShadowCast>().lightSourceAligned.zAxisMovement)
                        StartCoroutine(CameraPanOut(shadowShiftFollowObject.transform.position, new Vector3(playerShadow.transform.position.x,
                            (shadowShiftOutPlatforms[currentPlatformIndex].transform.position.y + shadowShiftOutPlatforms[currentPlatformIndex].transform.lossyScale.y / 2 + transform.lossyScale.y),
                            shadowShiftOutPlatforms[currentPlatformIndex].transform.position.z), false));
                    else if (GetComponent<PlayerShadowCast>().lightSourceAligned.xAxisMovement)
                        StartCoroutine(CameraPanOut(shadowShiftFollowObject.transform.position, new Vector3(shadowShiftOutPlatforms[currentPlatformIndex].transform.position.x,
                            (shadowShiftOutPlatforms[currentPlatformIndex].transform.position.y + shadowShiftOutPlatforms[currentPlatformIndex].transform.lossyScale.y / 2 + transform.lossyScale.y),
                            playerShadow.transform.position.z), false));
                }
            }
			else
            {
                StartCoroutine(CameraPanInMultiExit(shadowShiftFollowObject.transform.position, playerShadow.transform.position, -GetComponent<PlayerShadowCast>().lightSourceAligned.lightSourceDirection * Camera.main.GetComponent<CameraControl>().distanceToPlayer2D));
                StartCoroutine(FinishShiftOutMultiExit());
            }
		}

		if(Input.GetButtonDown("Shadowshift"))
		{
			FinishShiftOut();
		}
	}

	public IEnumerator CameraPanIn(Vector3 start, Vector3 target, Vector3 offset)
	{
		shadowShiftingIn = true;
		CameraControl.cameraIsPanning = true;

		if(shadowShiftFollowObject == null)
			shadowShiftFollowObject = Instantiate(shadowShiftFollowObjectPrefab, start, shadowShiftFollowObjectPrefab.transform.rotation);

		// Camera transition inwards to the targeted location
		cameraPanInStartPos = Camera.main.transform.position;
		cameraRelativeDirectionOffset = (cameraPanInStartPos - transform.position).normalized;

		float panStart = Time.time;
		while(Time.time < panStart + shadowShiftPanDuration)
		{
			shadowShiftFollowObject.transform.position = Vector3.Lerp(start, target, (Time.time - panStart)/shadowShiftPanDuration);
			Camera.main.transform.position = Vector3.Lerp(cameraPanInStartPos, target + offset, (Time.time - panStart)/shadowShiftPanDuration);
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
		Vector3 startPos = Camera.main.transform.position;
		float panStart = Time.time;
		while(Time.time < panStart + shadowShiftPanDuration)
		{
			shadowShiftFollowObject.transform.position = Vector3.Lerp(start, target, (Time.time - panStart)/shadowShiftPanDuration);
			Camera.main.transform.position = Vector3.Lerp(startPos, target + cameraRelativeDirectionOffset * Camera.main.GetComponent<CameraControl>().distanceToPlayer3D, (Time.time - panStart)/shadowShiftPanDuration);

			yield return null;
		}

		CameraControl.cameraIsPanning = false;
		if(finishing)
        {
            shadowShiftFollowObject.transform.position = target;
            FinishShiftOut();
        }
    }

    public IEnumerator CameraPanInMultiExit(Vector3 start, Vector3 target, Vector3 offset)
    {
        CameraControl.cameraIsPanning = true;

        if (shadowShiftFollowObject == null)
            shadowShiftFollowObject = Instantiate(shadowShiftFollowObjectPrefab, start, shadowShiftFollowObjectPrefab.transform.rotation);
        Vector3 startPos = Camera.main.transform.position;

        float panStart = Time.time;
        while (Time.time < panStart + shadowShiftPanDuration)
        {
            shadowShiftFollowObject.transform.position = Vector3.Lerp(start, target, (Time.time - panStart) / shadowShiftPanDuration);
            Camera.main.transform.position = Vector3.Lerp(startPos, target + offset, (Time.time - panStart) / shadowShiftPanDuration);
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

	public IEnumerator FinishShiftOutMultiExit()
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