using UnityEngine;

public class PlayerShadowCast : MonoBehaviour {
	public GameObject playerShadowCollider;

	[HideInInspector]
	public float zOffset;

	void Update () 
	{
		zOffset = -(transform.lossyScale.z / 2f);
		CastShadow();
		Check2DInvisibility();
	}


	public void CastShadow()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity))
		{
			if(hit.collider.gameObject.tag == "Shadow Wall")
				playerShadowCollider.SetActive(true);
			else
				playerShadowCollider.SetActive(false);
		}
	}

	public void Check2DInvisibility()
	{
		if(!PlayerMovement.in3DSpace)
			GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		else
			GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
	}
}
