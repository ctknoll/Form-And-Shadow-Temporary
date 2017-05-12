using UnityEngine;
using System.Collections.Generic;

public class ShadowCast : MonoBehaviour {
    public GameObject shadowColliderPrefab;
    public enum ShadowType { NO_SHADOW, BASIC_SHADOW, SPIKES, MOVE_PLATFORM, PUSH_CUBE, PROPELLOR_PLATFORM, ENEMY_TOAD };
    public ShadowType shadowType;
    public bool meshException;
    private GameObject lightReference;
    public List<GameObject> shadowColliders = new List<GameObject>();
    [HideInInspector]
    public bool singleMesh;
	private UnityEngine.Rendering.ShadowCastingMode shadowCastMode;
    List<float> zWallReferenceNumbers = new List<float>();
    List<float> xWallReferenceNumbers = new List<float>();

    void Start()
	{
        lightReference = GameObject.Find("Lighting_Reference");
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

        if(shadowType != ShadowType.NO_SHADOW)
        {
            foreach (Transform lightTransform in GameObject.Find("Lighting").GetComponentInChildren<Transform>())
            {
                if (!lightTransform.gameObject.activeSelf) continue;
                CastShadowCollider(lightTransform.transform.forward);
            }
        }
    }

	void Update () 
	{
        UpdateShadowcastMode();
    }

    public void CastShadowCollider(Vector3 direction)
    {
        // Number of casts from the unit
        float castNumber;

        // Check if you are casting in the z direction (forward or backward)
        if (direction == lightReference.transform.forward || direction == -lightReference.transform.forward)
        {
            castNumber = transform.lossyScale.x * 2;
            List<Vector3> castPoints = new List<Vector3>();

            float castPointIncrement = 0;
            for (int i = 0; i <= castNumber; i++)
            {
                Vector3 temp = new Vector3(transform.position.x - transform.lossyScale.x / 2 + castPointIncrement, transform.position.y, transform.position.z);
                castPoints.Add(temp);
                castPointIncrement += 0.5f;
            }

            foreach (Vector3 castPoint in castPoints)
            {
                Debug.DrawRay(castPoint, direction, Color.red, 10f);
                RaycastHit hit;
                if (Physics.Raycast(castPoint, direction, out hit, Mathf.Infinity, 1 << 10))
                {
                    if(!zWallReferenceNumbers.Contains(hit.collider.gameObject.transform.position.z))
                    {
                        zWallReferenceNumbers.Add(hit.collider.gameObject.transform.position.z);
                        bool lockedInZ = GetLockedAxis(direction);
                        Vector3 transOffset = GetTransformOffset(direction, lockedInZ);
                        GameObject shadowAdd = Instantiate(shadowColliderPrefab, hit.point + transOffset, hit.collider.gameObject.transform.rotation) as GameObject;
                        shadowAdd.GetComponent<ShadowCollider>().transformParent = gameObject;
                        shadowAdd.GetComponent<ShadowCollider>().transformOffset = transOffset;
                        shadowAdd.GetComponent<ShadowCollider>().lockedInZAxis = lockedInZ;
                        shadowAdd.GetComponent<ShadowCollider>().castDirection = direction;
                        shadowAdd.GetComponent<ShadowCollider>().wallTransform = hit.collider.transform;
                        shadowColliders.Add(shadowAdd);
                    }
                }
            }
        }
        // Or if you are casting in the x direction (left or right)
        else
        {
            castNumber = transform.lossyScale.z * 2;
            List<Vector3> castPoints = new List<Vector3>();

            float castPointIncrement = 0;
            for (int i = 0; i <= castNumber; i++)
            {
                Vector3 temp = new Vector3(transform.position.x, transform.position.y, transform.position.z - transform.lossyScale.z / 2 + castPointIncrement);
                castPoints.Add(temp);
                castPointIncrement += 0.5f;
            }

            foreach (Vector3 castPoint in castPoints)
            {
                Debug.DrawRay(castPoint, direction, Color.red, 10f);
                RaycastHit hit;
                if (Physics.Raycast(castPoint, direction, out hit, Mathf.Infinity, 1 << 10))
                {
                    if (!xWallReferenceNumbers.Contains(hit.collider.gameObject.transform.position.x))
                    {
                        xWallReferenceNumbers.Add(hit.collider.gameObject.transform.position.x);
                        bool lockedInZ = GetLockedAxis(direction);
                        Vector3 transOffset = GetTransformOffset(direction, lockedInZ);
                        GameObject shadowAdd = Instantiate(shadowColliderPrefab, hit.point + transOffset, hit.collider.gameObject.transform.rotation) as GameObject;
                        shadowAdd.GetComponent<ShadowCollider>().transformParent = gameObject;
                        shadowAdd.GetComponent<ShadowCollider>().transformOffset = transOffset;
                        shadowAdd.GetComponent<ShadowCollider>().lockedInZAxis = lockedInZ;
                        shadowAdd.GetComponent<ShadowCollider>().castDirection = direction;
                        shadowAdd.GetComponent<ShadowCollider>().wallTransform = hit.collider.transform;
                        shadowColliders.Add(shadowAdd);
                    }
                }
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
            else if (shadowType == ShadowType.PROPELLOR_PLATFORM)
                transOffset = ((transform.lossyScale.z / 1.95f) * castDirection);
            else if (shadowType == ShadowType.SPIKES)
                transOffset = ((GetComponent<BoxCollider>().bounds.size.z / 1.95f) * castDirection);
            else
                transOffset = ((transform.lossyScale.z / 1.95f) * castDirection);

        }
        else
        {
            if (meshException)
                transOffset = ((GetComponent<MeshCollider>().bounds.size.x / 1.95f) * castDirection);
            else if (shadowType == ShadowType.PROPELLOR_PLATFORM)
                transOffset = ((transform.lossyScale.z / 1.95f) * castDirection);
            else if (shadowType == ShadowType.SPIKES)
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
