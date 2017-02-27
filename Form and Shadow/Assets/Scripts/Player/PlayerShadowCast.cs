using UnityEngine;

public class PlayerShadowCast : MonoBehaviour {
	private GameObject playerShadow;

	[HideInInspector]
	public Vector3 transformOffset;
	[SerializeField]
	public LightSourceControl lightSourceAligned;

    void Start()
    {
        playerShadow = GameObject.Find("Player_Shadow");
    }
	void Update () 
	{
		Check2DInvisibility();
		lightSourceAligned = CheckLightSourceAligned().GetComponent<LightSourceControl>();
	}

	public void CastShadow()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, lightSourceAligned.lightSourceDirection, out hit, Mathf.Infinity, 1 << 10))
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
			if (lightSourceAligned.zAxisMovement) 
			{
				transformOffset = ((transform.lossyScale.z / 1.9f) * lightSourceAligned.lightSourceDirection);
			}
			// Is the light source projecting left or right?
			else if (lightSourceAligned.xAxisMovement) 
			{
				transformOffset = ((transform.lossyScale.x / 1.9f) * lightSourceAligned.lightSourceDirection);
			}
			
			playerShadow.transform.position = hit.point + transformOffset;
            playerShadow.GetComponent<PlayerShadowCollider>().zAxisMovement = lightSourceAligned.zAxisMovement;
            playerShadow.GetComponent<PlayerShadowCollider>().transformOffset = transformOffset;
            playerShadow.GetComponent<PlayerShadowCollider>().wallTransform = hit.collider.transform;
		}
	}

	public void Check2DInvisibility()
	{
		if(!PlayerMovement.in3DSpace || PlayerMovement.shadowShiftingIn)
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRend in meshRenderers)
            {
                meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
        else
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRend in meshRenderers)
            {
                meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }

	public GameObject CheckLightSourceAligned()
	{
		GameObject nearest = null;
		float distance = float.MaxValue;
		foreach(Transform child in GameObject.Find("Lighting").transform)
		{
			float tempDistance = 0;
			tempDistance += Mathf.Pow((child.forward.x - GameObject.Find ("Main_Camera").transform.forward.x), 2);
			tempDistance += Mathf.Pow((child.forward.y - GameObject.Find ("Main_Camera").transform.forward.y), 2);
			tempDistance += Mathf.Pow((child.forward.z - GameObject.Find ("Main_Camera").transform.forward.z), 2);

			if (tempDistance < distance) 
			{
				distance = tempDistance;
				nearest = child.gameObject;
			}
		}
		return nearest;
	}
}
