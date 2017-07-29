//*******************UPDATED*******************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewPlayerShadowInteraction : MonoBehaviour
{
    [Header("Master References")]
    public GameObject m_PlayerShadow;
    GameObject m_LightingMaster;
    [HideInInspector] public GameObject m_LightSourceAligned;
    //public GameController m_GameController;

    [Header("Shadow Shift Variables")]
    public bool m_ShadowshiftAvailable;
    //[SerializeField] AudioClip m_ShadowshiftInAudioClip;
    //[SerializeField] AudioClip m_ShadowShiftOutAudioClip;
    [SerializeField] GameObject m_ShadowShiftFollowPrefab;
    [SerializeField] float m_ShadowShiftDuration;
    [HideInInspector] public GameObject m_ShadowShiftFollowObject;
    List<GameObject> m_ShadowShiftOutPlatforms;
    int m_CurrentPlatformIndex;
    float m_PlayerShiftInOffset;
    Vector3 m_CameraPanInStartPosition;
    Vector3 m_CameraRelativeDirectionOffset;
    float m_CameraPanInRelativeDistance;

    [Header("Shadowmeld Variables and References")]
    public bool m_ShadowmeldAvailable;
    //[SerializeField] AudioClip m_ShadowmeldInAudioClip;
    //[SerializeField] AudioClip m_ShadowmeldOutAudioClip;
    //[SerializeField] GameObject m_ShadowmeldVFX;
    [Range(0, 20)][SerializeField] float m_ShadowmeldResourceCost;
    [Range(5, 20)][SerializeField] float m_ShadowmeldResourceRegen;
    public float m_MaxShadowmeldResource;
    [HideInInspector] public float m_CurrentShadowmeldResource;

    public enum PLAYERSTATE {FORM, SHADOW, GRABBING, SHIFTING, SHADOWMELDED};
    [Header("Player State and Respawn")]
    public static PLAYERSTATE m_CurrentPlayerState;
    public static Vector3 m_PlayerStartPosition;
    bool m_ZAxisTransition;

    void Start()
    {
        m_PlayerStartPosition = transform.position;
        m_CurrentPlayerState = PLAYERSTATE.FORM;
        m_CurrentShadowmeldResource = m_MaxShadowmeldResource;
        m_CurrentPlatformIndex = 0;
        m_LightingMaster = GameObject.Find("Lighting");
    }

    void Update()
    {
        if(!GameController.resetting && !GameController.paused)
        {
            switch(m_CurrentPlayerState)
            {
                case PLAYERSTATE.FORM:
                    if (m_ShadowshiftAvailable)
                        UpdateShadowShiftMaster();
                    if (m_ShadowmeldAvailable)
                        UpdateShadowmeldMaster();
                    break;
                case PLAYERSTATE.SHADOW:
                    UpdateShadowShiftMaster();
                    break;
                case PLAYERSTATE.GRABBING:
                    break;
                case PLAYERSTATE.SHADOWMELDED:
                    UpdateShadowmeldMaster();
                    break;
                case PLAYERSTATE.SHIFTING:
                    if(!NewCameraControl.cameraIsPanning)
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
                case PLAYERSTATE.FORM:
                    if (m_CurrentShadowmeldResource > 0)
                        EnterShadowmeld();
                    break;
                case PLAYERSTATE.SHADOWMELDED:
                    CheckShadowmeldExit();
                    break;
            }
        }
    }

    void UpdateShadowmeldResource()
    {
        switch(m_CurrentPlayerState)
        {
            case PLAYERSTATE.FORM:
                if (m_CurrentShadowmeldResource < m_MaxShadowmeldResource)
                    m_CurrentShadowmeldResource += m_ShadowmeldResourceRegen * Time.deltaTime;
                break;
            case PLAYERSTATE.SHADOWMELDED:
                if (m_CurrentShadowmeldResource > 0)
                    m_CurrentShadowmeldResource -= m_ShadowmeldResourceCost * Time.deltaTime;
                if (m_CurrentShadowmeldResource < 0)
                    CheckShadowmeldExit();
                break;
        }
    }

    void EnterShadowmeld()
    {
        m_CurrentPlayerState = PLAYERSTATE.SHADOWMELDED;
        //m_ShadowmeldVFX.SetActive(true);
        gameObject.layer = LayerMask.NameToLayer("Shadowmeld");
    }

    void CheckShadowmeldExit()
    {
        ExitShadowmeld();
    }

    void ExitShadowmeld()
    {
        //m_ShadowmeldVFX.SetActive(false);
        gameObject.layer = LayerMask.NameToLayer("Form");
        m_CurrentPlayerState = PLAYERSTATE.FORM;
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
                case PLAYERSTATE.FORM:
                    CheckShadowShiftIn();
                    break;
                case PLAYERSTATE.SHADOW:
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
            if (m_CurrentPlatformIndex + 1 < m_ShadowShiftOutPlatforms.Count)
            {
                m_CurrentPlatformIndex += 1;
                if(m_ZAxisTransition)
                {
                    targetLocation.y = m_ShadowShiftOutPlatforms[m_CurrentPlatformIndex].transform.position.y;
                    targetLocation.z = m_ShadowShiftOutPlatforms[m_CurrentPlatformIndex].transform.position.z;
                }
                else
                {
                    targetLocation.x = m_ShadowShiftOutPlatforms[m_CurrentPlatformIndex].transform.position.x;
                    targetLocation.y = m_ShadowShiftOutPlatforms[m_CurrentPlatformIndex].transform.position.y;
                }

                StartCoroutine(CameraPanOut(m_ShadowShiftFollowObject.transform.position, targetLocation, false));
            }
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            Vector3 targetLocation = m_PlayerShadow.transform.position;
            if (m_CurrentPlatformIndex == 0)
            {
                // If the player is at index 0 of the platforms, or the first platform, and they try to go forward,
                // they return to the wall
                StartCoroutine(CameraPanIn(m_ShadowShiftFollowObject.transform.position, targetLocation, -m_LightSourceAligned.GetComponent<LightSourceControl>().lightSourceDirection * Camera.main.GetComponent<NewCameraControl>().m_DistanceToPlayer2D));
            }
            else
            {
                m_CurrentPlatformIndex -= 1;
                if (m_ZAxisTransition)
                {
                    targetLocation.y = m_ShadowShiftOutPlatforms[m_CurrentPlatformIndex].transform.position.y;
                    targetLocation.z = m_ShadowShiftOutPlatforms[m_CurrentPlatformIndex].transform.position.z;
                }
                else
                {
                    targetLocation.x = m_ShadowShiftOutPlatforms[m_CurrentPlatformIndex].transform.position.x;
                    targetLocation.y = m_ShadowShiftOutPlatforms[m_CurrentPlatformIndex].transform.position.y;
                }
                StartCoroutine(CameraPanOut(m_ShadowShiftFollowObject.transform.position, targetLocation, false));
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
            Debug.Log("got a light");
            m_ZAxisTransition = m_LightSourceAligned.GetComponent<LightSourceControl>().zAxisMovement;
            // Cast a sphere in the direction of the most aligned light source on the
            // shadow wall layer
            if (Physics.SphereCast(transform.position, 0.2f,
                m_LightSourceAligned.GetComponent<LightSourceControl>().lightSourceDirection, out shadowWallHit, 1 << 10))
            {
                if (m_ZAxisTransition)
                    m_PlayerShiftInOffset = transform.position.z;
                else
                    m_PlayerShiftInOffset = transform.position.x;
                // Shift in
                StartCoroutine(CameraPanIn(transform.position, shadowWallHit.point,
                    -m_LightSourceAligned.GetComponent<LightSourceControl>().lightSourceDirection * Camera.main.GetComponent<NewCameraControl>().m_DistanceToPlayer2D));
            }
        }
    }

    void StartShadowShiftOut()
    {
        SetupShadowShiftOut();
        m_CurrentPlayerState = PLAYERSTATE.SHIFTING;

        Vector3 targetLocation = m_PlayerShadow.transform.position;
        if (m_ZAxisTransition)
        {
            switch (m_ShadowShiftOutPlatforms.Count)
            {
                case 0:
                    targetLocation.z = m_PlayerShiftInOffset;
                    break;
                default:
                    targetLocation.z = m_ShadowShiftOutPlatforms[0].transform.position.z;
                    break;
            }
        }
        else
        {
            switch (m_ShadowShiftOutPlatforms.Count)
            {
                case 0:
                    targetLocation.x = m_PlayerShiftInOffset;
                    break;
                default:
                    targetLocation.x = m_ShadowShiftOutPlatforms[0].transform.position.x;
                    break;
            }
        }

        if (m_ShadowShiftOutPlatforms.Count == 0 || m_ShadowShiftOutPlatforms.Count == 1)
            StartCoroutine(CameraPanOut(m_PlayerShadow.transform.position, targetLocation, true));
        else
            StartCoroutine(CameraPanOut(m_PlayerShadow.transform.position, targetLocation, false));
    }

    void SetupShadowShiftOut()
    {
        m_ShadowShiftOutPlatforms = m_PlayerShadow.GetComponent<PlayerShadowCollider>().GetTransferPlatforms();

        if (m_ShadowShiftOutPlatforms.Count != 0)
        {
            m_ShadowShiftOutPlatforms.Sort(delegate (GameObject t1, GameObject t2) {
                return Vector3.Distance(t1.transform.position, m_PlayerShadow.transform.position).CompareTo(Vector3.Distance(t2.transform.position, m_PlayerShadow.transform.position));
            });
        }
    }

    IEnumerator CameraPanIn(Vector3 start, Vector3 target, Vector3 cameraOffset)
    {
        m_CurrentPlayerState = PLAYERSTATE.SHIFTING;

        if (m_ShadowShiftFollowObject == null)
            m_ShadowShiftFollowObject = Instantiate(m_ShadowShiftFollowPrefab, start, Quaternion.identity);

        m_CameraPanInStartPosition = Camera.main.transform.position;
        m_CameraRelativeDirectionOffset = (m_CameraPanInStartPosition - transform.position).normalized;

        float panStart = Time.time;

        while(Time.time < panStart + m_ShadowShiftDuration)
        {
            m_ShadowShiftFollowObject.transform.position = Vector3.Lerp(start, target, (Time.time - panStart) / m_ShadowShiftDuration);
            Camera.main.transform.position = Vector3.Lerp(m_CameraPanInStartPosition, target + cameraOffset, (Time.time - panStart) / m_ShadowShiftDuration);
            yield return null;
        }
        FinishShiftIn();
    }

    IEnumerator CameraPanOut(Vector3 start, Vector3 target, bool finish)
    {
        if (m_ShadowShiftFollowObject == null)
            m_ShadowShiftFollowObject = Instantiate(m_ShadowShiftFollowPrefab, start, Quaternion.identity);

        Vector3 startPos = Camera.main.transform.position;
        float panStart = Time.time;
        while (Time.time < panStart + m_ShadowShiftDuration)
        {
            m_ShadowShiftFollowObject.transform.position = Vector3.Lerp(start, target, (Time.time - panStart) / m_ShadowShiftDuration);
            Camera.main.transform.position = Vector3.Lerp(startPos, target + m_CameraRelativeDirectionOffset * Camera.main.GetComponent<NewCameraControl>().m_DistanceMax, (Time.time - panStart) / m_ShadowShiftDuration);
            yield return null;
        }
        if (finish)
            FinishShiftingOut();
    }

    void FinishShiftIn()
    {
        // After the transition is finished, perform final steps
        m_PlayerShadow.transform.position = m_ShadowShiftFollowObject.transform.position;
        Destroy(m_ShadowShiftFollowObject);
        transform.parent = null;
        m_PlayerShadow.transform.parent = null;
        m_CurrentPlayerState = PLAYERSTATE.SHADOW;
    }

    void FinishShiftingOut()
    {
        m_CurrentPlatformIndex = 0;
        transform.position = m_ShadowShiftFollowObject.transform.position;
        Destroy(m_ShadowShiftFollowObject);
        transform.parent = null;
        m_PlayerShadow.transform.parent = null;
        m_CurrentPlayerState = PLAYERSTATE.FORM;
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
}