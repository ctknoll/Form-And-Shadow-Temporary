using UnityEngine;

public class PlayerShadowCast : MonoBehaviour {
	public GameObject playerShadowCollider;

	[HideInInspector]
	public Vector3 transformOffset;
	[SerializeField]
	public Transform wallTransform;

	void Update () 
	{
		Check2DInvisibility();
	}

	public void CastShadow()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity))
		{
			if (hit.collider.gameObject.tag == "Shadow Wall") 
			{
				if (LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.forward || 
					-1 * LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.forward) 
				{
					transformOffset = ((transform.lossyScale.z / 1.9f) * LightSourceControl.lightSourceDirection);
				}
				else if (LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.right || 
					-1 * LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.right) 
				{
					transformOffset = ((transform.lossyScale.x / 1.9f) * LightSourceControl.lightSourceDirection);
				}
				else 
				{
					transformOffset = ((transform.lossyScale.y / 1.9f) * LightSourceControl.lightSourceDirection);
				}

				wallTransform = hit.collider.transform;
				playerShadowCollider.SetActive(true);
				GetComponent<PlayerMovement>().playerShadow.transform.position = hit.point + transformOffset;
                GetComponent<PlayerMovement>().playerShadow.transform.rotation = Quaternion.LookRotation(hit.normal);
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
