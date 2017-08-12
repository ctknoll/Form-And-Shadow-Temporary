using UnityEngine;

public class ShadowmeldObjectControl : MonoBehaviour
{
    public enum ShadowMeldObjectType {Level_Geometry, Static, Glass, Water, Flat_Spikes, Spikes, Conveyor_Belt};
    public ShadowMeldObjectType m_ShadowmeldObjectType;

    public struct ShadowmeldObjectChildData
    {
        public GameObject childObject;
        public LayerMask childStartingLayer;
        public Material childStartingMaterial;
        public Material[] childMaterialReferences;
    }

    ShadowmeldObjectChildData[] childData;
    Object collideMaterial;
    Object ignoreMaterial;
    Object geometryMaterial;
    Object deathMaterial;
    bool layerCollisionTurnedOn;

    void Awake ()
    {
        collideMaterial = Resources.Load("Shadowmeld_Collide_Material");
        geometryMaterial = Resources.Load("Shadowmeld_Geometry_Material");
        ignoreMaterial = Resources.Load("Shadowmeld_Ignore_Material");
        deathMaterial = Resources.Load("Shadowmeld_Death_Material");

        int i = 0;
        childData = new ShadowmeldObjectChildData[GetComponentsInChildren<MeshRenderer>().Length];

        foreach(Transform transformChild in GetComponentsInChildren<Transform>())
        {
            if (!transformChild.gameObject.GetComponent<MeshRenderer>())
                continue;
            childData[i].childObject = transformChild.gameObject;
            childData[i].childStartingLayer = transformChild.gameObject.layer;
            childData[i].childStartingMaterial = transformChild.GetComponent<MeshRenderer>().material;
            childData[i].childMaterialReferences = transformChild.GetComponent<MeshRenderer>().materials;
            i++;
        }
    }
	
	void Update ()
    {
        switch (PlayerShadowInteraction.m_CurrentPlayerState)
        {
            case PlayerShadowInteraction.PlayerState.Shadowmelded:
                if(!layerCollisionTurnedOn)
                {
                    TurnOnShadowmeldLayerandCollision();
                    layerCollisionTurnedOn = true;
                }
                break;
            default:
                if(layerCollisionTurnedOn)
                {
                    TurnOffShadowmeldLayerandCollision();
                    layerCollisionTurnedOn = false;
                }
                break;
        }
    }

    void TurnOnShadowmeldLayerandCollision()
    {
        switch (m_ShadowmeldObjectType)
        {
            case ShadowMeldObjectType.Level_Geometry:
                TurnOnShadowmeldGeometry();
                break;
            case ShadowMeldObjectType.Static:
                TurnOnShadowmeldCollide();
                break;
            case ShadowMeldObjectType.Glass:
                TurnOnShadowmeldIgnore();
                break;
            case ShadowMeldObjectType.Water:
                TurnOnShadowmeldIgnore();
                break;
            case ShadowMeldObjectType.Conveyor_Belt:
                TurnOnShadowmeldIgnore();
                break;
            case ShadowMeldObjectType.Flat_Spikes:
                TurnOnShadowmeldCollide();
                GetComponent<BoxCollider>().isTrigger = false;
                break;
            case ShadowMeldObjectType.Spikes:
                TurnOnShadowmeldDeath();
                break;
        }
    }

    void TurnOffShadowmeldLayerandCollision()
    {
        foreach (ShadowmeldObjectChildData child in childData)
        {
            child.childObject.layer = child.childStartingLayer;
            int i = 0;
            foreach (Material rendererMaterial in child.childMaterialReferences)
            {
                child.childMaterialReferences[i] = child.childStartingMaterial as Material;
                i++;
            }
            child.childObject.GetComponent<MeshRenderer>().materials = child.childMaterialReferences;
        }

        switch (m_ShadowmeldObjectType)
        {
            case ShadowMeldObjectType.Flat_Spikes:
                GetComponent<BoxCollider>().isTrigger = true;
                break;
            default:
                break;
        }
    }

    void TurnOnShadowmeldCollide()
    {
        foreach (ShadowmeldObjectChildData child in childData)
        {
            child.childObject.layer = LayerMask.NameToLayer("Shadowmeld Collide");
            int i = 0;
            foreach(Material mat in child.childMaterialReferences)
            {
                child.childMaterialReferences[i] = collideMaterial as Material;
                i++;
            }
            child.childObject.GetComponent<MeshRenderer>().materials = child.childMaterialReferences;
        }
    }

    void TurnOnShadowmeldGeometry()
    {
        foreach (ShadowmeldObjectChildData child in childData)
        {
            child.childObject.layer = LayerMask.NameToLayer("Shadowmeld Collide");
            int i = 0;
            foreach (Material mat in child.childMaterialReferences)
            {
                child.childMaterialReferences[i] = geometryMaterial as Material;
                i++;
            }
            child.childObject.GetComponent<MeshRenderer>().materials = child.childMaterialReferences;
        }
    }

    void TurnOnShadowmeldIgnore()
    {
        foreach (ShadowmeldObjectChildData child in childData)
        {
            child.childObject.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
            int i = 0;
            foreach (Material mat in child.childMaterialReferences)
            {
                child.childMaterialReferences[i] = ignoreMaterial as Material;
                i++;
            }
            child.childObject.GetComponent<MeshRenderer>().materials = child.childMaterialReferences;
        }
    }

    void TurnOnShadowmeldDeath()
    {
        foreach (ShadowmeldObjectChildData child in childData)
        {
            child.childObject.layer = LayerMask.NameToLayer("Shadowmeld Death");
            int i = 0;
            foreach (Material mat in child.childMaterialReferences)
            {
                child.childMaterialReferences[i] = deathMaterial as Material;
                i++;
            }
            child.childObject.GetComponent<MeshRenderer>().materials = child.childMaterialReferences;
        }
    }
}