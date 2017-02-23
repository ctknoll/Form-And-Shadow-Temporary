using UnityEngine;
using System.Collections.Generic;

public class ShadowCast : MonoBehaviour {
	public GameObject shadowColliderPrefab;
    public enum MeshType {NO_SHADOW, BASIC_CUBE, SPIKES, MOVE_PLATFORM, PUSH_CUBE, PROPELLOR_PLATFORM, ENEMY_TOAD};
    public bool meshException;
    public MeshType meshType;

    public List<GameObject> shadowColliders = new List<GameObject>();

    private LayerMask startingLayer;
    private bool singleMesh;
	private UnityEngine.Rendering.ShadowCastingMode shadowCastMode;

	void Start()
	{
        startingLayer = gameObject.layer;
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
        if (shadowCastMode != UnityEngine.Rendering.ShadowCastingMode.Off && meshType != MeshType.ENEMY_TOAD)
            Check2DShadowsOnly();
        else
            Check2DInvisibility();

        if(GameController.playerShadowMelded)
        {
            CheckShadowmeldLayerandCollision();
        }
        else
        {
            gameObject.layer = startingLayer;
        }
	}

	public void CastShadow(Vector3 direction)
	{
		RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, 1 << 10))
		{
            /* To explain this, first one must know that there is a game object called Lighting Reference in the scene
			 * that is always rotated to (0, 0, 0) and that all objects in the scene that will cast shadows must have
			 * their parent objects rotated the same. This basically checks if the light source direction in relativity
			 * to said lighting reference, and then sets the transform offset of the shadow collider (whether it be the
			 * player's shadow collider or an object's) relative to said direction. So, if the light source direction is
			 * equal to the reference's -transform.right (left), then all shadows are offset a little more than their size
			 * behind the respective wall the previous Raycast hits.
			*/

            
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
                transOffset = ((GetComponent<MeshCollider>().bounds.size.z / 2) * castDirection);
            else if (meshType == ShadowCast.MeshType.PROPELLOR_PLATFORM)
                transOffset = ((transform.lossyScale.z / 2) * castDirection);
            else
                transOffset = ((transform.lossyScale.z / 2) * castDirection);

        }
        else
        {
            if (meshException)
                transOffset = ((GetComponent<MeshCollider>().bounds.size.x / 2) * castDirection);
            else if (meshType == ShadowCast.MeshType.PROPELLOR_PLATFORM)
                transOffset = ((transform.lossyScale.z / 2) * castDirection);
            else
                transOffset = ((transform.lossyScale.x / 2) * castDirection);
        }
        return transOffset;
    }

    public void CheckShadowmeldLayerandCollision()
    {
        switch (meshType)
        {
            case MeshType.NO_SHADOW:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
                break;
            default:
                break;
        }
    }

	public void Check2DShadowsOnly()
	{
        if (!PlayerMovement.in3DSpace && !PlayerMovement.shiftingOut)
        {
            if (singleMesh)
                GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            else
            {
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRend in meshRenderers)
                {
                    meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }

        }
        else
        {
            if (singleMesh)
                GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            else
            {
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRend in meshRenderers)
                {
                    meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
            }
        }
    }
    public void Check2DInvisibility()
    {
        if(!PlayerMovement.in3DSpace && !PlayerMovement.shiftingOut)
        {
            if (singleMesh)
                GetComponent<MeshRenderer>().enabled = false;
            else
            {
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRend in meshRenderers)
                {
                    meshRend.enabled = false;
                }
            }
        }
        else
        {
            if (singleMesh)
                GetComponent<MeshRenderer>().enabled = true;
            else
            {
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRend in meshRenderers)
                {
                    meshRend.enabled = true;
                }
            }
        }
    }
}
