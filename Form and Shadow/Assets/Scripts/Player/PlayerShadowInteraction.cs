using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShadowInteraction : MonoBehaviour
{
    public static PlayerShadowInteraction m_Instance;
    [Header("Master References")]
    public static GameObject m_PlayerShadow;
    GameObject m_LightingMaster;
    [HideInInspector] public GameObject m_LightSourceAligned;

    [Header("Shadow Shift Variables")]
    public bool m_ShadowshiftAvailable;
    [SerializeField] GameObject m_ShadowShiftFollowPrefab;
    [SerializeField] float m_ShadowShiftDuration;
    [HideInInspector] public static GameObject m_ShadowShiftFollowObject;
    List<GameObject> m_ShadowShiftOutPlatforms;

    [Header("Shadowmeld Variables and References")]
    public bool m_ShadowmeldAvailable;
    [Range(0, 20)][SerializeField] float m_ShadowmeldResourceCost;
    [Range(5, 20)][SerializeField] float m_ShadowmeldResourceRegen;
    public float m_MaxShadowmeldResource;
    [HideInInspector] public float m_CurrentShadowmeldResource;

    public enum PlayerState {Form, Shadow, Grabbing, Shifting, Shadowmelded};
    [Header("Player State and Respawn")]
    public static PlayerState m_CurrentPlayerState;
    public static Vector3 m_PlayerRespawnPosition;
    [HideInInspector] public bool m_ZAxisTransition;

    Material playerStartMaterial;
    int currentPlatformIndex;
    float playerShiftInOffset;
    Vector3 cameraPanInStartPosition;
    Vector3 cameraRelativeDirectionOffset;
    float cameraPanInRelativeDistance;

    void Start()
    {
        m_Instance = this;
        m_PlayerRespawnPosition = transform.position;
        m_CurrentPlayerState = PlayerState.Form;
        m_CurrentShadowmeldResource = m_MaxShadowmeldResource;
        m_PlayerShadow = GameObject.Find("Player_Shadow");
        m_LightingMaster = GameObject.Find("Lighting_Master_Control");
        currentPlatformIndex = 0;
        playerStartMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
        m_PlayerShadow.GetComponentInChildren<PlayerShadowSquishCheck>().enabled = false;
    }

    void Update()
    {
        if(!GameController.m_Resetting && !GameController.m_Paused)
        {
            switch(m_CurrentPlayerState)
            {
                case PlayerState.Form:
                    if (m_ShadowshiftAvailable)
                        UpdateShadowShiftMaster();
                    if (m_ShadowmeldAvailable)
                        UpdateShadowmeldMaster();
                    break;
                case PlayerState.Shadow:
                    UpdateShadowShiftMaster();
                    break;
                case PlayerState.Grabbing:
                    break;
                case PlayerState.Shadowmelded:
                    UpdateShadowmeldMaster();
                    break;
                case PlayerState.Shifting:
                    if(!CameraControl.cameraIsPanning)
                        UpdateShadowShiftingInput();
                    break;
            }
        }
    }

#region Shadowmeld Methods
    void UpdateShadowmeldMaster()
    {
        UpdateShadowmeldInput();
        UpdateShadowmeldResource();
    }

    void UpdateShadowmeldInput()
    {
        if (Input.GetButtonDown("Shadowmeld"))
        {
            switch (m_CurrentPlayerState)
            {
                case PlayerState.Form:
                    if (m_CurrentShadowmeldResource > 0)
                        EnterShadowmeld();
                    break;
                case PlayerState.Shadowmelded:
                    CheckShadowmeldExit();
                    break;
            }
        }
    }

    void UpdateShadowmeldResource()
    {
        switch(m_CurrentPlayerState)
        {
            case PlayerState.Form:
                if (m_CurrentShadowmeldResource < m_MaxShadowmeldResource)
                    m_CurrentShadowmeldResource += m_ShadowmeldResourceRegen * Time.deltaTime;
                break;
            case PlayerState.Shadowmelded:
                if (m_CurrentShadowmeldResource > 0)
                    m_CurrentShadowmeldResource -= m_ShadowmeldResourceCost * Time.deltaTime;
                if (m_CurrentShadowmeldResource < 0)
                    CheckShadowmeldExit();
                break;
        }
    }

    void EnterShadowmeld()
    {
        m_CurrentPlayerState = PlayerState.Shadowmelded;
        TogglePlayerShadowmeldAppearance(true);
        gameObject.layer = LayerMask.NameToLayer("Shadowmeld");
    }

    void CheckShadowmeldExit()
    {
        ExitShadowmeld();
    }

    public void ExitShadowmeld()
    {
        m_CurrentPlayerState = PlayerState.Form;
        gameObject.layer = LayerMask.NameToLayer("Form");
        TogglePlayerShadowmeldAppearance(false);
    }
#endregion

#region Shadow Shift Methods
    void UpdateShadowShiftMaster()
    {
        UpdateShadowShiftInput();        
    }

    void UpdateShadowShiftInput()
    {
        if (Input.GetButtonDown("Shadowshift"))
        {
            switch (m_CurrentPlayerState)
            {
                case PlayerState.Form:
                    CheckShadowShiftIn();
                    break;
                case PlayerState.Shadow:
                    StartShadowShiftOut();
                    break;
            }
        }
    }

    void UpdateShadowShiftingInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Vector3 targetLocation = m_PlayerShadow.transform.position;
            if (currentPlatformIndex + 1 < m_ShadowShiftOutPlatforms.Count)
            {
                currentPlatformIndex += 1;
                if(m_ZAxisTransition)
                {
                    targetLocation.y = m_ShadowShiftOutPlatforms[currentPlatformIndex].transform.position.y + PlayerController.m_CharacterController.height / 2;
                    targetLocation.z = m_ShadowShiftOutPlatforms[currentPlatformIndex].transform.position.z;
                }
                else
                {
                    targetLocation.x = m_ShadowShiftOutPlatforms[currentPlatformIndex].transform.position.x;
                    targetLocation.y = m_ShadowShiftOutPlatforms[currentPlatformIndex].transform.position.y + PlayerController.m_CharacterController.height / 2;
                }

                StartCoroutine(ShiftPlayerOut(m_ShadowShiftFollowObject.transform.position, targetLocation, false));
            }
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            Vector3 targetLocation = m_PlayerShadow.transform.position;
            if (currentPlatformIndex == 0)
            {
                // If the player is at index 0 of the platforms, or the first platform, and they try to go forward,
                // they return to the wall
                StartCoroutine(ShiftPlayerIn(m_ShadowShiftFollowObject.transform.position, targetLocation, 
                    -m_LightSourceAligned.GetComponent<LightSourceControl>().m_LightSourceForward * Camera.main.GetComponent<CameraControl>().m_DistanceToPlayer2D));
            }
            else
            {
                currentPlatformIndex -= 1;
                if (m_ZAxisTransition)
                {
                    targetLocation.y = m_ShadowShiftOutPlatforms[currentPlatformIndex].transform.position.y + PlayerController.m_CharacterController.height;
                    targetLocation.z = m_ShadowShiftOutPlatforms[currentPlatformIndex].transform.position.z;
                }
                else
                {
                    targetLocation.x = m_ShadowShiftOutPlatforms[currentPlatformIndex].transform.position.x;
                    targetLocation.y = m_ShadowShiftOutPlatforms[currentPlatformIndex].transform.position.y + PlayerController.m_CharacterController.height;
                }
                StartCoroutine(ShiftPlayerOut(m_ShadowShiftFollowObject.transform.position, targetLocation, false));
            }
        }

        if(Input.GetButtonDown("Shadowshift"))
        {
            FinishShiftingOut();
        }
    }

    void CheckShadowShiftIn()
    {
        RaycastHit shadowWallHit;

        if(CheckLightSourceAligned() != null)
        {
            Debug.Log("Aligned light source found!");
            switch(m_LightSourceAligned.GetComponent<LightSourceControl>().m_CurrentFacingDirection)
            {
                case LightSourceControl.FacingDirection.North:
                    m_ZAxisTransition = true;
                    break;
                case LightSourceControl.FacingDirection.East:
                    m_ZAxisTransition = false;
                    break;
                case LightSourceControl.FacingDirection.South:
                    m_ZAxisTransition = true;
                    break;
                case LightSourceControl.FacingDirection.West:
                    m_ZAxisTransition = false;
                    break;
            }
            // Cast a sphere in the direction of the most aligned light source on the
            // shadow wall layer
            if (Physics.SphereCast(transform.position + new Vector3(0, 0.6f, 0), 0.2f, m_LightSourceAligned.GetComponent<LightSourceControl>().m_LightSourceForward, out shadowWallHit, 1 << 10))
            {
                RaycastHit hit;
                if(!Physics.Raycast(shadowWallHit.point, m_LightSourceAligned.GetComponent<LightSourceControl>().m_LightSourceForward, out hit, Mathf.Infinity, 1 << 11))
                {
                    if (m_ZAxisTransition)
                        playerShiftInOffset = transform.position.z;
                    else
                        playerShiftInOffset = transform.position.x;
                    TogglePlayerMeshVisibility(true);
                    StartCoroutine(ShiftPlayerIn(transform.position, shadowWallHit.point - new Vector3(0, 0.6f, 0) + m_LightSourceAligned.GetComponent<LightSourceControl>().m_LightSourceForward,
                        -m_LightSourceAligned.GetComponent<LightSourceControl>().m_LightSourceForward * 8));
                }
            }
        }
    }

    void StartShadowShiftOut()
    {
        SetupShadowShiftOut();
        m_CurrentPlayerState = PlayerState.Shifting;

        Vector3 targetLocation = m_PlayerShadow.transform.position;
        if (m_ZAxisTransition)
        {
            switch (m_ShadowShiftOutPlatforms.Count)
            {
                case 0:
                    targetLocation.z = playerShiftInOffset;
                    break;
                default:
                    targetLocation.y = m_ShadowShiftOutPlatforms[0].transform.position.y + PlayerController.m_CharacterController.height / 2;
                    targetLocation.z = m_ShadowShiftOutPlatforms[0].transform.position.z;
                    break;
            }
        }
        else
        {
            switch (m_ShadowShiftOutPlatforms.Count)
            {
                case 0:
                    targetLocation.x = playerShiftInOffset;
                    break;
                default:
                    targetLocation.x = m_ShadowShiftOutPlatforms[0].transform.position.x;
                    targetLocation.y = m_ShadowShiftOutPlatforms[0].transform.position.y + PlayerController.m_CharacterController.height / 2;
                    break;
            }
        }

        Ray ray = new Ray(m_PlayerShadow.transform.position, -m_LightSourceAligned.GetComponent<LightSourceControl>().m_LightSourceForward);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 10);

        Vector3 startLocation = hit.point;
        if (m_ShadowShiftOutPlatforms.Count == 0 || m_ShadowShiftOutPlatforms.Count == 1)
            StartCoroutine(ShiftPlayerOut(startLocation, targetLocation, true));
        else
            StartCoroutine(ShiftPlayerOut(startLocation, targetLocation, false));
    }

    void SetupShadowShiftOut()
    {
        m_ShadowShiftOutPlatforms = GetTransferPlatforms();
        if (m_ShadowShiftOutPlatforms.Count != 0)
        {
            m_ShadowShiftOutPlatforms.Sort(delegate (GameObject t1, GameObject t2) {
                return Vector3.Distance(t1.transform.position, m_PlayerShadow.transform.position).CompareTo(Vector3.Distance(t2.transform.position, m_PlayerShadow.transform.position));
            });
        }
    }

    List<GameObject> GetTransferPlatforms()
    {
        List<GameObject> transferPlatforms = new List<GameObject>();
        // Cast a ray down from the player shadow and store all shadow colliders hit in an array of RaycastHits
        RaycastHit firstPlatformHit;
        Debug.DrawRay(m_PlayerShadow.transform.position, Vector3.down, Color.red, 10f);
        if (Physics.SphereCast(m_PlayerShadow.transform.position + new Vector3(0, 1, 0), 0.5f, Vector3.down, out firstPlatformHit, m_PlayerShadow.transform.position.y, 1 << 11))
        {
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(firstPlatformHit.point - new Vector3(0, 0.5f, 0), 0.5f, Vector3.down, GetComponent<CharacterController>().height / 2, 1 << 11);
            // Then, create a list of gameobjects and for each RaycastHit in hits, add the hit collider's gameobject to the list of transferPlatforms
            foreach (RaycastHit hit in hits)
            {
                // Prevent killzone colliders from being added as shadow collider objects
                if (hit.collider.GetComponentInParent<ShadowCollider>().m_TransformParent.GetComponent<ShadowCast>().m_CastedShadowType != ShadowCast.CastedShadowType.Killzone_Shadow
                    && hit.collider.GetComponentInParent<ShadowCollider>().m_TransformParent.GetComponent<ShadowmeldObjectControl>().m_ShadowmeldObjectType != ShadowmeldObjectControl.ShadowMeldObjectType.Flat_Spikes)
                    transferPlatforms.Add(hit.collider.gameObject.GetComponentInParent<ShadowCollider>().m_TransformParent.gameObject);
            }
        }
        // Then, return a list of gameobjects equal to all the shadow colliders below the player when called
        return transferPlatforms;
    }

    IEnumerator ShiftPlayerIn(Vector3 start, Vector3 target, Vector3 cameraOffset)
    {
        m_CurrentPlayerState = PlayerState.Shifting;
        CameraControl.cameraIsPanning = true;

        if (m_ShadowShiftFollowObject == null)
            m_ShadowShiftFollowObject = Instantiate(m_ShadowShiftFollowPrefab, start, Quaternion.identity);

        cameraPanInStartPosition = Camera.main.transform.position;
        cameraRelativeDirectionOffset = (cameraPanInStartPosition - transform.position).normalized;

        float panStart = Time.time;

        while(Time.time < panStart + m_ShadowShiftDuration)
        {
            m_ShadowShiftFollowObject.transform.position = Vector3.Lerp(start, target, (Time.time - panStart) / m_ShadowShiftDuration);
            Camera.main.transform.position = Vector3.Lerp(cameraPanInStartPosition, target + cameraOffset, (Time.time - panStart) / m_ShadowShiftDuration);
            yield return null;
        }
        CameraControl.cameraIsPanning = false;
        FinishShiftIn();
    }

    IEnumerator ShiftPlayerOut(Vector3 start, Vector3 target, bool finish)
    {
        CameraControl.cameraIsPanning = true;

        if (m_ShadowShiftFollowObject == null)
            m_ShadowShiftFollowObject = Instantiate(m_ShadowShiftFollowPrefab, start, Quaternion.identity);

        Vector3 startPos = Camera.main.transform.position;
        float panStart = Time.time;
        while (Time.time < panStart + m_ShadowShiftDuration)
        {
            m_ShadowShiftFollowObject.transform.position = Vector3.Lerp(start, target, (Time.time - panStart) / m_ShadowShiftDuration);
            Camera.main.transform.position = Vector3.Lerp(startPos, target + cameraRelativeDirectionOffset * Camera.main.GetComponent<CameraControl>().m_DistanceMax, (Time.time - panStart) / m_ShadowShiftDuration);
            yield return null;
        }

        CameraControl.cameraIsPanning = false;

        if (finish)
            FinishShiftingOut();
    }

    void FinishShiftIn()
    {
        // After the transition is finished, perform final steps
        switch(m_LightSourceAligned.GetComponent<LightSourceControl>().m_CurrentFacingDirection)
        {
            case LightSourceControl.FacingDirection.North:
                m_PlayerShadow.transform.position = new Vector3(m_ShadowShiftFollowObject.transform.position.x, m_ShadowShiftFollowObject.transform.position.y, LightingMasterControl.m_NorthFloorTransform.position.z);
                break;
            case LightSourceControl.FacingDirection.East:
                m_PlayerShadow.transform.position = new Vector3(LightingMasterControl.m_EastFloorTransform.position.x, m_ShadowShiftFollowObject.transform.position.y, m_ShadowShiftFollowObject.transform.position.z);
                break;
            case LightSourceControl.FacingDirection.South:
                m_PlayerShadow.transform.position = new Vector3(m_ShadowShiftFollowObject.transform.position.x, m_ShadowShiftFollowObject.transform.position.y, LightingMasterControl.m_SouthFloorTransform.position.z);
                break;
            case LightSourceControl.FacingDirection.West:
                m_PlayerShadow.transform.position = new Vector3(LightingMasterControl.m_WestFloorTransform.position.x, m_ShadowShiftFollowObject.transform.position.y, m_ShadowShiftFollowObject.transform.position.z);
                break;
        }

        m_PlayerShadow.GetComponentInChildren<PlayerShadowSquishCheck>().enabled = true;
        m_PlayerShadow.GetComponent<CharacterController>().enabled = true;
        PlayerController.m_CharacterController = m_PlayerShadow.GetComponent<CharacterController>();
        GetComponent<CharacterController>().enabled = false;

        Destroy(m_ShadowShiftFollowObject);
        transform.parent = null;
        m_PlayerShadow.transform.parent = null;
        m_CurrentPlayerState = PlayerState.Shadow;
    }

    public void FinishShiftingOut()
    {
        currentPlatformIndex = 0;

        if (m_ShadowShiftFollowObject)
        {
            Destroy(m_ShadowShiftFollowObject);
            transform.position = m_ShadowShiftFollowObject.transform.position;
        }

        m_PlayerShadow.GetComponentInChildren<PlayerShadowSquishCheck>().enabled = false;
        m_PlayerShadow.GetComponent<CharacterController>().enabled = false;
        PlayerController.m_CharacterController = GetComponent<CharacterController>();
        GetComponent<CharacterController>().enabled = true;
        TogglePlayerMeshVisibility(false);

        transform.parent = null;
        m_PlayerShadow.transform.parent = null;
        m_CurrentPlayerState = PlayerState.Form;
    }

    public GameObject CheckLightSourceAligned()
    {
        GameObject mostAlignedLightSource = null;
        foreach (Transform lightTransform in m_LightingMaster.transform)
        {
            if (lightTransform.gameObject.activeSelf)
            {
                Transform cameraTransform = Camera.main.transform;
                cameraTransform.eulerAngles = new Vector3(0, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
                float tempAngle = Vector3.Angle(lightTransform.forward, cameraTransform.forward);
                if (Mathf.Abs(tempAngle) < 35)
                {
                    m_LightSourceAligned = lightTransform.gameObject;
                    mostAlignedLightSource = lightTransform.gameObject;
                }
            }
            else
                continue;
        }
        return mostAlignedLightSource;
    }
    #endregion

#region Utility Methods
    void TogglePlayerMeshVisibility(bool on)
    {
        foreach (SkinnedMeshRenderer meshRend in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (on)
                meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            else
                meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

    void TogglePlayerShadowmeldAppearance(bool on)
    {
        foreach(SkinnedMeshRenderer meshRend in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (on)
            {
                meshRend.material = Resources.Load("Shadowmeld_Player_Material") as Material;
                meshRend.gameObject.layer = LayerMask.NameToLayer("Shadowmeld");
            }
            else
            {
                meshRend.material = playerStartMaterial;
                meshRend.gameObject.layer = LayerMask.NameToLayer("Form");
            }
        }
    }
#endregion
}