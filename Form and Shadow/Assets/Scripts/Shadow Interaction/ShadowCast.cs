using UnityEngine;
using System.Collections.Generic;

public class ShadowCast : MonoBehaviour {
	public GameObject shadowColliderPrefab;
	public List<GameObject> shadowCollider = new List<GameObject>();

	[HideInInspector]
	public Vector3 transformOffset;
	[SerializeField]
	public Transform wallTransform = null;
	private bool singleMesh;
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
			// set the global shadowcast mode equal to it, assuming all children follow the same shadowcast
			// mode
			shadowCastMode = GetComponentInChildren<MeshRenderer>().shadowCastingMode;
	}

	void Update () 
	{
		if((shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.On || shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly) && shadowCollider.Count == 1)
			CastShadow();

		if(shadowCastMode != UnityEngine.Rendering.ShadowCastingMode.Off)
			Check2DInvisibility();
	}

	public void CastShadow()
	{
		RaycastHit hit;
        Debug.DrawLine(transform.position, transform.position + LightSourceControl.lightSourceDirection * 10, Color.red, 1);
        if (Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity, 1 << 10))
		{
			/* To explain this, first one must know that there is a game object called Lighting Reference in the scene
			 * that is always rotated to (0, 0, 0) and that all objects in the scene that will cast shadows must have
			 * their parent objects rotated the same. This basically checks if the light source direction in relativity
			 * to said lighting reference, and then sets the transform offset of the shadow collider (whether it be the
			 * player's shadow collider or an object's) relative to said direction. So, if the light source direction is
			 * equal to the reference's -transform.right (left), then all shadows are offset a little more than their size
			 * behind the respective wall the previous Raycast hits.
			*/

			// Is thelight source projecting forward or backward?
			if (LightSourceControl.zAxisMovement) 
			{
				transformOffset = ((transform.lossyScale.z / 1.9f) * LightSourceControl.lightSourceDirection);
			}
			// Is the light source projecting left or right?
			else if (LightSourceControl.xAxisMovement) 
			{
				transformOffset = ((transform.lossyScale.x / 1.9f) * LightSourceControl.lightSourceDirection);
			}
			else 
			{
				transformOffset = ((transform.lossyScale.y / 1.9f) * LightSourceControl.lightSourceDirection);
			}

			if (tag != "Propellor Platform") 
			{
				shadowCollider.Add(Instantiate (shadowColliderPrefab, hit.point, Quaternion.identity, gameObject.transform) as GameObject);
			}
				
			else {
				GameObject shadowAdd = Instantiate (shadowColliderPrefab, hit.point, Quaternion.identity) as GameObject;
                shadowAdd.GetComponent<ShadowCollider> ().exceptionParent = gameObject;
                shadowCollider.Add(shadowAdd);
			}
			
			wallTransform = hit.collider.transform;
		}
	}

	public void Check2DInvisibility()
	{
		if(!PlayerMovement.in3DSpace && !PlayerMovement.shiftingOut)
		{
			if(singleMesh)
				GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			else
			{
				MeshRenderer [] meshRenderers = GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRend in meshRenderers)
				{
					meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;	
				}
			}
				
		}
		else
			if(singleMesh)
				GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			else
			{
				MeshRenderer [] meshRenderers = GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRend in meshRenderers)
				{
					meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;	
				}
			}
	}
}
