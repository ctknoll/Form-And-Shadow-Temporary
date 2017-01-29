using UnityEngine;

public class PlayerShadowCast : MonoBehaviour {
	public GameObject playerShadowCollider;

	[HideInInspector]
	public Vector3 zOffset;
	[SerializeField]
	public Transform wallTransform = null;

	void Update () 
	{
		zOffset = ((transform.lossyScale.z / 2f + .1f) * LightSourceControl.lightSourceDirection);
		if (wallTransform != null)
			zOffset = .5f * wallTransform.forward;
		else {zOffset = new Vector3 (0, 0, -1);}
		Check2DInvisibility();
	}


	public void CastShadow()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity))
		{
			if (hit.collider.gameObject.tag == "Shadow Wall") 
			{
				playerShadowCollider.SetActive(true);
				wallTransform = hit.collider.transform;
				GetComponent<PlayerMovement>().playerShadow.transform.position = hit.point + zOffset;
                GetComponent<PlayerMovement>().playerShadow.transform.rotation = Quaternion.LookRotation(hit.normal);
				GameObject.Find("Player_Shadow").transform.position = hit.point + zOffset;
			}
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
