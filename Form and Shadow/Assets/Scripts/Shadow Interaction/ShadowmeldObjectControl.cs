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
    }

    ShadowmeldObjectChildData[] childData;
    Object collideMaterial;
    Object ignoreMaterial;
    Object deathMaterial;
    bool layerCollisionTurnedOn;

    void Awake ()
    {
        collideMaterial = Resources.Load("Shadowmeld_Collide_Material");
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
                TurnOnShadowmeldIgnore();
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
            child.childObject.GetComponent<MeshRenderer>().material = child.childStartingMaterial;
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
        foreach(ShadowmeldObjectChildData child in childData)
        {
            child.childObject.layer = LayerMask.NameToLayer("Shadowmeld Collide");
            child.childObject.GetComponent<MeshRenderer>().material = collideMaterial as Material;
        }
    }

    void TurnOnShadowmeldIgnore()
    {
        foreach (ShadowmeldObjectChildData child in childData)
        {
            child.childObject.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
            child.childObject.GetComponent<MeshRenderer>().material = ignoreMaterial as Material;
        }
    }

    void TurnOnShadowmeldDeath()
    {
        foreach (ShadowmeldObjectChildData child in childData)
        {
            child.childObject.layer = LayerMask.NameToLayer("Shadowmeld Death");
            child.childObject.GetComponent<MeshRenderer>().material = deathMaterial as Material;
        }
    }
}