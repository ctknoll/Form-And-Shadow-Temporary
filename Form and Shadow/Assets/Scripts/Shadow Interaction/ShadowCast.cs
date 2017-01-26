using UnityEngine;

public class ShadowCast : MonoBehaviour {
	public GameObject shadowPrefab;
	public GameObject shadowCollider;

	[HideInInspector]
	public float zOffset;

	void Update () 
	{
		if(!shadowCollider)
			CastShadow();
		zOffset = -(transform.lossyScale.z / 2f);
		Check2DInvisibility();
	}

	public void CastShadow()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity))
		{
			if(hit.collider.gameObject.tag == "Shadow Wall")
				shadowCollider = Instantiate(shadowPrefab, hit.point, Quaternion.identity, gameObject.transform) as GameObject;
		}
	}

	public void Check2DInvisibility()
	{
		if(!PlayerMovement.in3DSpace)
		{
			if(GetComponent<MeshRenderer>())
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
			if(GetComponent<MeshRenderer>())
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
