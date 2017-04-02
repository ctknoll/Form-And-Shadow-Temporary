using UnityEngine;
using System.Collections.Generic;

public class ShadowCast : MonoBehaviour {
    public GameObject shadowColliderPrefab;
    public enum MeshType { NO_SHADOW, BASIC_CUBE, SPIKES, MOVE_PLATFORM, PUSH_CUBE, PROPELLOR_PLATFORM, ENEMY_TOAD };
    public bool meshException;
    public MeshType meshType;

    public List<GameObject> shadowColliders = new List<GameObject>();

    
    
    public bool singleMesh;
	private UnityEngine.Rendering.ShadowCastingMode shadowCastMode;

	void Start()
	{
        // Used to differentiate between 3D objects with multiple mesh renderers childed under it (like spikes)
		// and single objects with one master mesh renderer
		singleMesh = GetComponent<MeshRenderer>();
		if(singleMesh)
			// Then, set the shadowCastMode for this specific object equal to the master mesh renderer
			shadowCastMode = GetComponent<MeshRenderer>().shadowCastingMode;
		else
			// Or, in the case of multiple children renderers, access just one of the renderers and 
			// set the global shadowcast mode equal to it, assuming all children follow the same shadowcast mode
			shadowCastMode = GetComponentInChildren<MeshRenderer>().shadowCastingMode;

        if(meshType != MeshType.NO_SHADOW)
        {
            CastShadow(GameObject.Find("Lighting_Reference").transform.right);
            CastShadow(-GameObject.Find("Lighting_Reference").transform.right);
            CastShadow(GameObject.Find("Lighting_Reference").transform.forward);
            CastShadow(-GameObject.Find("Lighting_Reference").transform.forward);
        }
    }

	void Update () 
	{
        UpdateShadowcastMode();
    }

    public void CastShadow(Vector3 direction)
	{
		RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, 1 << 10))
		{   
            bool lockedInZ = GetLockedAxis(direction);
            Vector3 transOffset = GetTransformOffset(direction, lockedInZ);
            if (meshType == MeshType.PROPELLOR_PLATFORM || meshType == MeshType.ENEMY_TOAD || meshType == MeshType.PUSH_CUBE)
            {
                GameObject shadowAdd = Instantiate(shadowColliderPrefab, hit.point + transOffset, hit.collider.gameObject.transform.rotation) as GameObject;
                shadowAdd.GetComponent<ShadowCollider>().exceptionParent = gameObject;
                shadowAdd.GetComponent<ShadowCollider>().transformOffset = transOffset;
                shadowAdd.GetComponent<ShadowCollider>().lockedInZAxis = lockedInZ;
                shadowAdd.GetComponent<ShadowCollider>().castDirection = direction;
                shadowAdd.GetComponent<ShadowCollider>().wallTransform = hit.collider.transform;
                shadowColliders.Add(shadowAdd);

            }
            // If not, instantiate the shadowcollider prefab and child it to the gameobject
            else
            {
                GameObject shadowAdd = Instantiate(shadowColliderPrefab, hit.point + transOffset, Quaternion.identity, gameObject.transform) as GameObject;
                shadowAdd.GetComponent<ShadowCollider>().transformOffset = transOffset;
                shadowAdd.GetComponent<ShadowCollider>().castDirection = direction;
                shadowAdd.GetComponent<ShadowCollider>().lockedInZAxis = lockedInZ;
                shadowAdd.GetComponent<ShadowCollider>().wallTransform = hit.collider.transform;
                shadowColliders.Add(shadowAdd);
            }


        }
	}
    public bool GetLockedAxis(Vector3 castDir)
    {
        return castDir == GameObject.Find("Lighting_Reference").transform.forward || -1 * castDir == GameObject.Find("Lighting_Reference").transform.forward;
    }

    public Vector3 GetTransformOffset(Vector3 castDirection, bool zLocked)
    {
        Vector3 transOffset;
        if (zLocked)
        {
            if (meshException)
                transOffset = ((GetComponent<MeshCollider>().bounds.size.z / 1.95f) * castDirection);
            else if (meshType == MeshType.PROPELLOR_PLATFORM)
                transOffset = ((transform.lossyScale.z / 1.95f) * castDirection);
            else if (meshType == MeshType.SPIKES)
                transOffset = ((GetComponent<BoxCollider>().bounds.size.z / 1.95f) * castDirection);
            else
                transOffset = ((transform.lossyScale.z / 1.95f) * castDirection);

        }
        else
        {
            if (meshException)
                transOffset = ((GetComponent<MeshCollider>().bounds.size.x / 1.95f) * castDirection);
            else if (meshType == MeshType.PROPELLOR_PLATFORM)
                transOffset = ((transform.lossyScale.z / 1.95f) * castDirection);
            else if (meshType == MeshType.SPIKES)
                transOffset = ((GetComponent<BoxCollider>().bounds.size.x / 1.95f) * castDirection);
            else
                transOffset = ((transform.lossyScale.x / 1.95f) * castDirection);
        }
        return transOffset;
    }
    
    // Handles turning off an on Shadowcastmodes for various objects based on if they are
    // not casting shadows (ShadowCastingMode.Off), or normally casting shadows (ShadowCastingMode.On)
    // in 3D space, and changing their ShadowCastingMode when the player transitions into 2D.
	public void UpdateShadowcastMode()
	{
        if(!PlayerMovement.shadowMelded)
        {
            if (!PlayerMovement.in3DSpace && !PlayerMovement.shadowShiftingOut)
            {
                if (singleMesh)
                {
                    if (shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.Off)
                        GetComponent<MeshRenderer>().enabled = false;
                    else if (shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.On)
                        GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
                else
                {
                    MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer meshRend in meshRenderers)
                    {
                        if (shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.Off)
                            meshRend.enabled = false;
                        else if (shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.On)
                            meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    }
                }
            }
            else
            {
                if (singleMesh)
                {
                    if (shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.Off)
                        GetComponent<MeshRenderer>().enabled = true;
                    else if (shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.On)
                        GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                else
                {
                    MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer meshRend in meshRenderers)
                    {
                        if (shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.Off)
                            meshRend.enabled = true;
                        else if (shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.On)
                            meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }
                }
            }
        }
    }
}
