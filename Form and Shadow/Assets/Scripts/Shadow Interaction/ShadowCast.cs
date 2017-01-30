using UnityEngine;

public class ShadowCast : MonoBehaviour {
	public GameObject shadowPrefab;
	public GameObject shadowCollider;

	[HideInInspector]
	public Vector3 transformOffset;
	[SerializeField]
	public Transform wallTransform = null;
	private bool singleMesh;
	private UnityEngine.Rendering.ShadowCastingMode shadowCastMode;

	void Start()
	{
		singleMesh = GetComponent<MeshRenderer>();
		if(singleMesh)
			shadowCastMode = GetComponent<MeshRenderer>().shadowCastingMode;
		else
			shadowCastMode = GetComponentInChildren<MeshRenderer>().shadowCastingMode;
	}

	void Update () 
	{
		if(!shadowCollider)
		{
			if(singleMesh)
			{
				if(shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.On || shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly)
					CastShadow();
			}
			else
			{
				if(shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.On || shadowCastMode == UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly)
					CastShadow();
			}
		}

		if (wallTransform != null) 
		{
			{
				if (LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.forward || -1 * LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.forward) 
				{
					transformOffset = ((transform.lossyScale.z / 2f) * LightSourceControl.lightSourceDirection);
				}
				else if (LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.right || -1 * LightSourceControl.lightSourceDirection == GameObject.Find("Light Reference").transform.right) 
				{
					transformOffset = ((transform.lossyScale.x / 2f) * LightSourceControl.lightSourceDirection);
				}
				else 
				{
					transformOffset = ((transform.lossyScale.y / 2f) * LightSourceControl.lightSourceDirection);
				}
			}
		}
		else {transformOffset = new Vector3 (0, 0, -1);}

		if(shadowCastMode != UnityEngine.Rendering.ShadowCastingMode.Off)
			Check2DInvisibility();
	}

	public void CastShadow()
	{
		RaycastHit hit;
        Debug.DrawLine(transform.position, transform.position + LightSourceControl.lightSourceDirection * 10, Color.red, 1);
        if (Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity, 1 << 10))
		{         
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
