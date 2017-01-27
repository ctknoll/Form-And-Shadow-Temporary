using UnityEngine;

public class ShadowCast : MonoBehaviour {
	public GameObject shadowPrefab;
	public GameObject shadowCollider;

	[HideInInspector]
	public Vector3 zOffset;
	[SerializeField]
	public Transform wallTransform = null;

	void Update () 
	{
		if(!shadowCollider)
			CastShadow();
		if (wallTransform != null) 
		{
				if (LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.forward || -1 * LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.forward) 
				{
					zOffset = ((transform.lossyScale.z / 2f) * LightSourceControl.lightSourceDirection);
				}
				else if (LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.right || -1 * LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.right) 
				{
					zOffset = ((transform.lossyScale.x / 2f) * LightSourceControl.lightSourceDirection);
				}
				else 
				{
					zOffset = ((transform.lossyScale.y / 2f) * LightSourceControl.lightSourceDirection);
				}
			}

		else {zOffset = new Vector3 (0, 0, -1);}
		Check2DInvisibility();
	}

	public void CastShadow()
	{
		RaycastHit hit;
        Debug.DrawLine(transform.position, transform.position + LightSourceControl.lightSourceDirection * 10, Color.red, 1);
        if (Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity, 1 << 10))
		{
            Debug.Log(Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity, 1 << 10));
            Debug.Log(hit.collider);
            if (hit.collider.gameObject.tag == "Shadow Wall") 
			{
                
				shadowCollider = Instantiate (shadowPrefab, hit.point, Quaternion.identity, gameObject.transform) as GameObject;
				wallTransform = hit.collider.transform;
			}
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
