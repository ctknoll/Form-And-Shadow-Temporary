using UnityEngine;

public class ShadowCast : MonoBehaviour {
    public enum CastedShadowType { No_Shadow, Basic_Shadow, Killzone_Shadow, Move_Platform, Propellor_Platform };
    public CastedShadowType m_CastedShadowType;
    private GameObject m_LightingMasterControl;

    [System.Serializable] public struct ShadowColliders
    {
        public GameObject m_NorthShadowCollider;
        public GameObject m_EastShadowCollider;
        public GameObject m_SouthShadowCollider;
        public GameObject m_WestShadowCollider;
    }
    public ShadowColliders m_ShadowColliders;

    void Start()
	{
        m_LightingMasterControl = GameObject.Find("Lighting_Master_Control");

        if(m_CastedShadowType != CastedShadowType.No_Shadow)
        {
            foreach (Transform lightTransform in m_LightingMasterControl.GetComponentInChildren<Transform>())
            {
                if(lightTransform.gameObject.activeSelf)
                    CastShadowCollider(lightTransform.GetComponent<LightSourceControl>());
            }
        }
    }

    void CastShadowCollider(LightSourceControl lightSourceControl)
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, lightSourceControl.m_LightSourceForward, Color.red, 10f);
        if (Physics.Raycast(transform.position, lightSourceControl.m_LightSourceForward, out hit, Mathf.Infinity, 1 << 10))
        {
            switch (lightSourceControl.m_CurrentFacingDirection)
            {
                case LightSourceControl.FacingDirection.North:
                    if(!m_ShadowColliders.m_NorthShadowCollider)
                        CreateShadowCollider(lightSourceControl.m_CurrentFacingDirection);
                    break;
                case LightSourceControl.FacingDirection.East:
                    if(!m_ShadowColliders.m_EastShadowCollider)
                        CreateShadowCollider(lightSourceControl.m_CurrentFacingDirection);
                    break;
                case LightSourceControl.FacingDirection.South:
                    if (!m_ShadowColliders.m_SouthShadowCollider)
                        CreateShadowCollider(lightSourceControl.m_CurrentFacingDirection);
                    break;
                case LightSourceControl.FacingDirection.West:
                    if (!m_ShadowColliders.m_WestShadowCollider)
                        CreateShadowCollider(lightSourceControl.m_CurrentFacingDirection);
                    break;
            }
        }
	}

    void CreateShadowCollider(LightSourceControl.FacingDirection castedFacingDirection)
    {
        GameObject newShadowCollider = new GameObject();
        newShadowCollider.AddComponent<ShadowCollider>();
        newShadowCollider.GetComponent<ShadowCollider>().m_TransformParent = gameObject.transform;
        Vector3 tempShadowColliderScale = gameObject.transform.lossyScale;
        Vector3 tempShadowColliderPosition = gameObject.transform.position;
        
        switch(castedFacingDirection)
        {
            case LightSourceControl.FacingDirection.North:
                m_ShadowColliders.m_NorthShadowCollider = newShadowCollider;
                tempShadowColliderScale.z = 1f;
                tempShadowColliderPosition.z = LightingMasterControl.m_NorthFloorTransform.position.z;
                newShadowCollider.GetComponent<ShadowCollider>().m_ZAxisCast = true;
                break;
            case LightSourceControl.FacingDirection.East:
                m_ShadowColliders.m_EastShadowCollider = newShadowCollider;
                if (m_CastedShadowType == CastedShadowType.Propellor_Platform)
                    tempShadowColliderScale.z = tempShadowColliderScale.x;
                tempShadowColliderScale.x = 1f;
                tempShadowColliderPosition.x = LightingMasterControl.m_EastFloorTransform.position.x;
                newShadowCollider.GetComponent<ShadowCollider>().m_ZAxisCast = false;
                break;
            case LightSourceControl.FacingDirection.South:
                m_ShadowColliders.m_SouthShadowCollider = newShadowCollider;
                tempShadowColliderScale.z = 1f;
                tempShadowColliderPosition.z = LightingMasterControl.m_SouthFloorTransform.position.z;
                newShadowCollider.GetComponent<ShadowCollider>().m_ZAxisCast = true;
                break;
            case LightSourceControl.FacingDirection.West:
                m_ShadowColliders.m_WestShadowCollider = newShadowCollider;
                if (m_CastedShadowType == CastedShadowType.Propellor_Platform)
                    tempShadowColliderScale.z = tempShadowColliderScale.x;
                tempShadowColliderScale.x = 1f;
                tempShadowColliderPosition.x = LightingMasterControl.m_WestFloorTransform.position.x;
                newShadowCollider.GetComponent<ShadowCollider>().m_ZAxisCast = false;
                break;
        }

        newShadowCollider.layer = LayerMask.NameToLayer("Shadow");
        newShadowCollider.transform.localScale = tempShadowColliderScale;
        newShadowCollider.transform.position = tempShadowColliderPosition;

        switch(m_CastedShadowType)
        {
            case CastedShadowType.Basic_Shadow:
                SetUpBasicShadowCollider(newShadowCollider);
                break;
            case CastedShadowType.Move_Platform:
                SetUpMovePlatformShadowCollider(newShadowCollider);
                break;
            case CastedShadowType.Propellor_Platform:
                SetUpPropellorPlatformShadowCollider(newShadowCollider);
                break;
            case CastedShadowType.Killzone_Shadow:
                SetUpKillZoneShadowCollider(newShadowCollider);
                break;
        }
    }

    void SetUpBasicShadowCollider(GameObject shadowColliderObj)
    {
        shadowColliderObj.name = "Basic_Shadow_Collider";
        shadowColliderObj.AddComponent<BoxCollider>();
    }

    void SetUpMovePlatformShadowCollider(GameObject shadowColliderObj)
    {
        // First, add a box collider to the base shadow-casting object that mimics the size and scale of its
        // parent, the shadow-casting object
        shadowColliderObj.name = "Move_Platform_Shadow_Collider";
        shadowColliderObj.AddComponent<BoxCollider>();

        // Then, proceed with an unnecessarily complicated method to add a moving trigger zone on top of the
        // platform that childs the player shadow to it when within it so the player follows the platform
        GameObject platformColliderTriggerZone = new GameObject("Move_Platform_Shadow_Collider_Trigger_Zone");
        platformColliderTriggerZone.layer = LayerMask.NameToLayer("Shadow");
        platformColliderTriggerZone.transform.position = shadowColliderObj.transform.position;
        platformColliderTriggerZone.transform.rotation = shadowColliderObj.transform.rotation;
        platformColliderTriggerZone.transform.localScale = new Vector3(shadowColliderObj.transform.lossyScale.x, 0.5f, shadowColliderObj.transform.lossyScale.z);
        platformColliderTriggerZone.transform.parent = shadowColliderObj.transform;
        platformColliderTriggerZone.AddComponent<BoxCollider>();
        platformColliderTriggerZone.GetComponent<BoxCollider>().isTrigger = true;
        platformColliderTriggerZone.GetComponent<BoxCollider>().center = new Vector3(0, transform.lossyScale.y + platformColliderTriggerZone.transform.lossyScale.y, 0);
        platformColliderTriggerZone.AddComponent<MovingPlatformShadowCollider>();
    }

    void SetUpPropellorPlatformShadowCollider(GameObject shadowColliderObj)
    {
        shadowColliderObj.name = "Propellor_Collider_Shadow_Collider";
        shadowColliderObj.AddComponent<BoxCollider>();
        shadowColliderObj.GetComponent<BoxCollider>().isTrigger = true;

        GameObject propellorShadowColliderZone = new GameObject("Propellor_Platform_Shadow_Collider_Zone");
        propellorShadowColliderZone.transform.position = shadowColliderObj.transform.position;
        propellorShadowColliderZone.transform.parent = shadowColliderObj.transform;
        propellorShadowColliderZone.AddComponent<BoxCollider>();
        propellorShadowColliderZone.AddComponent<PropellorPlatformShadowCollider>();
        propellorShadowColliderZone.GetComponent<PropellorPlatformShadowCollider>().m_PropellorMesh = gameObject;
    }

    public void SetUpKillZoneShadowCollider(GameObject shadowColliderObj)
    {
        shadowColliderObj.name = "Killzone_Shadow_Collider";
        shadowColliderObj.AddComponent<BoxCollider>();
        shadowColliderObj.GetComponent<BoxCollider>().size = GetComponent<BoxCollider>().size;
        shadowColliderObj.AddComponent<Killzone>();
        shadowColliderObj.GetComponent<BoxCollider>().isTrigger = true;
    }
}
