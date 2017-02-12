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
		if(Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, Mathf.Infinity, 1 << 10))
		{
			/* To explain this, first one must know that there is a game object called Lighting Reference in the scene
			 * that is always rotated to (0, 0, 0) and that all objects in the scene that will cast shadows must have
			 * their parent objects rotated the same. This basically checks if the light source direction in relativity
			 * to said lighting reference, and then sets the transform offset of the shadow collider (whether it be the
			 * player's shadow collider or an object's) relative to said direction. So, if the light source direction is
			 * equal to the reference's -transform.right (left), then all shadows are offset a little more than their size
			 * behind the respective wall the previous Raycast hits.
			*/
			
			// Is the light source projecting forward or backward?
			if (LightSourceControl.zAxisMovement) 
			{
				transformOffset = ((transform.lossyScale.z / 1.9f) * LightSourceControl.lightSourceDirection);
			}
			// Is the light source projecting left or right?
			else if (LightSourceControl.xAxisMovement) 
			{
				transformOffset = ((transform.lossyScale.x / 1.9f) * LightSourceControl.lightSourceDirection);
			}
			
			wallTransform = hit.collider.transform;
			GetComponent<PlayerMovement>().playerShadow.transform.position = hit.point + transformOffset;
			GetComponent<PlayerMovement>().playerShadow.transform.rotation = Quaternion.LookRotation(hit.normal);
		}
	}

	public void Check2DInvisibility()
	{
		if(!PlayerMovement.in3DSpace || PlayerMovement.shiftingIn)
			GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		else
			GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
	}
}
