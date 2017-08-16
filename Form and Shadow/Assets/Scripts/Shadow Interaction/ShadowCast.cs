using System;
using UnityEngine;

public class ShadowCast : MonoBehaviour {
    public enum CastedShadowType { Basic_Shadow, Killzone_Shadow, Move_Platform, Propellor_Platform };
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
        foreach (Transform lightTransform in m_LightingMasterControl.GetComponentInChildren<Transform>())
        {
            if (lightTransform.gameObject.activeSelf)
                CastShadowCollider(lightTransform.GetComponent<LightSourceControl>());
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
        GameObject shadowColliderMaster = new GameObject();
        GameObject shadowColliderMesh = Instantiate(transform.GetChild(0).gameObject, shadowColliderMaster.transform);

        Vector3 tempShadowColliderMeshScale = shadowColliderMesh.transform.lossyScale;
        Vector3 tempShadowCollideMasterPosition = transform.position;
        bool zAxisCast = false;
        
        switch(castedFacingDirection)
        {
            case LightSourceControl.FacingDirection.North:
                m_ShadowColliders.m_NorthShadowCollider = shadowColliderMaster;
                tempShadowColliderMeshScale.z = 1f;
                tempShadowCollideMasterPosition.z = LightingMasterControl.m_NorthFloorTransform.position.z;
                zAxisCast = true;
                break;
            case LightSourceControl.FacingDirection.East:
                m_ShadowColliders.m_EastShadowCollider = shadowColliderMaster;
                if (m_CastedShadowType == CastedShadowType.Propellor_Platform)
                    tempShadowColliderMeshScale.z = tempShadowColliderMeshScale.x;
                tempShadowColliderMeshScale.x = 1f;
                tempShadowCollideMasterPosition.x = LightingMasterControl.m_EastFloorTransform.position.x;
                zAxisCast = false;
                break;
            case LightSourceControl.FacingDirection.South:
                m_ShadowColliders.m_SouthShadowCollider = shadowColliderMaster;
                tempShadowColliderMeshScale.z = 1f;
                tempShadowCollideMasterPosition.z = LightingMasterControl.m_SouthFloorTransform.position.z;
                zAxisCast = true;
                break;
            case LightSourceControl.FacingDirection.West:
                m_ShadowColliders.m_WestShadowCollider = shadowColliderMaster;
                if (m_CastedShadowType == CastedShadowType.Propellor_Platform)
                    tempShadowColliderMeshScale.z = tempShadowColliderMeshScale.x;
                tempShadowColliderMeshScale.x = 1f;
                tempShadowCollideMasterPosition.x = LightingMasterControl.m_WestFloorTransform.position.x;
                zAxisCast = false;
                break;
        }

        shadowColliderMesh.transform.localScale = tempShadowColliderMeshScale;
        shadowColliderMaster.transform.position = tempShadowCollideMasterPosition;


        switch (m_CastedShadowType)
        {
            case CastedShadowType.Basic_Shadow:
                SetUpBasicShadowCollider(shadowColliderMaster, shadowColliderMesh);
                break;
            case CastedShadowType.Move_Platform:
                SetUpMovePlatformShadowCollider(shadowColliderMaster, shadowColliderMesh, zAxisCast);
                break;
            case CastedShadowType.Propellor_Platform:
                SetUpPropellorPlatformShadowCollider(shadowColliderMaster, shadowColliderMesh, zAxisCast);
                break;
            case CastedShadowType.Killzone_Shadow:
                SetUpKillZoneShadowCollider(shadowColliderMaster, shadowColliderMesh, zAxisCast);
                break;
        }

        shadowColliderMaster.AddComponent<ShadowCollider>();
        shadowColliderMaster.GetComponent<ShadowCollider>().m_TransformParent = gameObject.transform;
        shadowColliderMaster.GetComponent<ShadowCollider>().m_ZAxisCast = zAxisCast;
    }



    void SetUpBasicShadowCollider(GameObject shadowColliderMaster, GameObject shadowColliderObj)
    {
        shadowColliderMaster.name = "Basic_Shadow_Collider_Master";
        shadowColliderObj.name = "Basic_Shadow_Collider_Mesh_Master";
    }

    void SetUpMovePlatformShadowCollider(GameObject shadowColliderMaster, GameObject shadowColliderMesh, bool zAxisCast)
    {
        shadowColliderMaster.name = "Move_Platform_Shadow_Collider_Master";
        shadowColliderMesh.name = "Platform_Shadow_Collider_Mesh_Master";

        GameObject platformShadowColliderTriggerZone = Instantiate(transform.GetChild(1).gameObject, shadowColliderMaster.transform.position, shadowColliderMaster.transform.rotation, shadowColliderMaster.transform);
        if (zAxisCast)
            platformShadowColliderTriggerZone.GetComponent<BoxCollider>().size = new Vector3(platformShadowColliderTriggerZone.GetComponent<BoxCollider>().size.x, platformShadowColliderTriggerZone.GetComponent<BoxCollider>().size.y, 1);
        else
            platformShadowColliderTriggerZone.GetComponent<BoxCollider>().size = new Vector3(1, platformShadowColliderTriggerZone.GetComponent<BoxCollider>().size.y, platformShadowColliderTriggerZone.GetComponent<BoxCollider>().size.z);

    }

    void SetUpPropellorPlatformShadowCollider(GameObject shadowColliderMaster, GameObject shadowColliderMesh, bool zAxisCast)
    {
        shadowColliderMaster.name = "Propellor_Platform_Shadow_Collider_Master";
        shadowColliderMesh.name = "Propellor_Shadow_Collider_Mesh_Master";

        shadowColliderMesh.AddComponent<PropellorPlatformShadowCollider>();
        shadowColliderMesh.GetComponent<PropellorPlatformShadowCollider>().m_ZAxisCast = zAxisCast;
        shadowColliderMesh.GetComponent<PropellorPlatformShadowCollider>().m_PropellorRotationSpeed = GetComponentInParent<PropellorPlatform>().m_RotationSpeed;

        GameObject propellorShadowColliderTriggerZone = Instantiate(shadowColliderMesh.transform.GetChild(0).gameObject, shadowColliderMesh.transform.position, shadowColliderMesh.transform.rotation, shadowColliderMaster.transform);
        propellorShadowColliderTriggerZone.name = "Propellor_Shadow_Collider_Trigger_Zone";
        propellorShadowColliderTriggerZone.transform.localScale = shadowColliderMesh.transform.lossyScale;
        propellorShadowColliderTriggerZone.GetComponent<BoxCollider>().isTrigger = true;
    }

    void SetUpKillZoneShadowCollider(GameObject shadowColliderMaster, GameObject shadowColliderMesh, bool zAxisCast)
    {
        shadowColliderMaster.name = "Killzone_Shadow_Collider_Master";
        shadowColliderMesh.name = "Killzone_Shadow_Collider_Mesh_Master";

        GameObject spikesShadowColliderKillZone = Instantiate(transform.GetChild(1).gameObject, shadowColliderMaster.transform.position, shadowColliderMaster.transform.rotation, shadowColliderMaster.transform);
        if (zAxisCast)
            spikesShadowColliderKillZone.GetComponent<BoxCollider>().size = new Vector3(spikesShadowColliderKillZone.GetComponent<BoxCollider>().size.x, spikesShadowColliderKillZone.GetComponent<BoxCollider>().size.y, 1);
        else
            spikesShadowColliderKillZone.GetComponent<BoxCollider>().size = new Vector3(1, spikesShadowColliderKillZone.GetComponent<BoxCollider>().size.y, spikesShadowColliderKillZone.GetComponent<BoxCollider>().size.z);
    }
}
