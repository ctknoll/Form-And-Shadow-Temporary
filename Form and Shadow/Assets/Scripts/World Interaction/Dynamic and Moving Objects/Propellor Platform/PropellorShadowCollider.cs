using UnityEngine;
using System.Collections;

public class PropellorShadowCollider : MonoBehaviour
{
	public GameObject propellor;

	private float midScale;
	private float maxScale;
	private float minScale;

	private GameObject propellorMesh;

	void Start ()
	{
		propellorMesh = propellor.transform.GetChild(0).gameObject;
	}
	
	void FixedUpdate ()
	{
		if(!CameraControl.cameraIsPanning)
		{
			if(LightSourceControl.zAxisMovement)
			{
				transform.localScale = new Vector3(propellorMesh.transform.lossyScale.x + (propellorMesh.transform.lossyScale.z - propellorMesh.transform.lossyScale.x) * Mathf.Abs(Mathf.Sin((propellor.GetComponent<PropellorPlatform>().rotationSpeed * Time.time * Mathf.PI / 180))),
					propellorMesh.transform.lossyScale.y, propellorMesh.transform.lossyScale.z);
			}
			else if(LightSourceControl.xAxisMovement)
			{
				transform.localScale = new Vector3(propellorMesh.transform.lossyScale.x, propellorMesh.transform.lossyScale.y, 
					propellorMesh.transform.lossyScale.z + (propellorMesh.transform.lossyScale.x - propellorMesh.transform.lossyScale.z) * Mathf.Abs(Mathf.Sin((propellor.GetComponent<PropellorPlatform>().rotationSpeed * Time.time * Mathf.PI / 180))));
			}
		}
	}
}

