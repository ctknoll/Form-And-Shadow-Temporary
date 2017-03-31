using UnityEngine;

/*
    Written by: Daniel Colina and Chris Knoll
    --PlayerShadowCast--
    This script handles all logic of throwing the player
    shadow into the wall through Shadowshift, making the 
    player character's mesh invisible when the player is
    in the world, and finding aligned light sources

*/

public class PlayerShadowCast : MonoBehaviour {
	private GameObject playerShadow;

	[HideInInspector]
	[SerializeField]
	public LightSourceControl lightSourceAligned;

    void Start()
    {
        playerShadow = GameObject.Find("Player_Shadow");
    }

	void Update () 
	{
		CheckShadowcastModeandLightingChange();
		lightSourceAligned = CheckLightSourceAligned().GetComponent<LightSourceControl>();
        if(PlayerMovement.in3DSpace && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !PlayerMovement.isGrabbing && !PlayerMovement.shadowMelded && !GameController.paused)
            GameController.CheckShadowShiftTooltip(lightSourceAligned.gameObject.activeSelf == true);
	}

    // Similar to the CastShadow method in Shadowcast, this method throws the player's shadow onto by casting a ray
    // in the direction of whichever lightsource most aligned with the way the camera is currently rotated around 
    // the player, and then changes various variables on the PlayerShadowCollider script related to which wall it
    // should be attached to.
	public void CastShadow()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, lightSourceAligned.lightSourceDirection, out hit, Mathf.Infinity, 1 << 10))
		{

            Vector3 transformOffset = new Vector3();

			if (lightSourceAligned.zAxisMovement) 
			{
				transformOffset = ((transform.lossyScale.z / 1.95f) * lightSourceAligned.lightSourceDirection);
			}
			else if (lightSourceAligned.xAxisMovement) 
			{
				transformOffset = ((transform.lossyScale.x / 1.95f) * lightSourceAligned.lightSourceDirection);
			}
			
			playerShadow.transform.position = hit.point + transformOffset;
            playerShadow.transform.rotation = Quaternion.LookRotation(hit.normal);
            playerShadow.GetComponent<PlayerShadowCollider>().zAxisMovement = lightSourceAligned.zAxisMovement;
            playerShadow.GetComponent<PlayerShadowCollider>().transformOffset = transformOffset;
            playerShadow.GetComponent<PlayerShadowCollider>().wallTransform = hit.collider.transform;
		}
	}

    // Similar to Check2DInvisibility method in Shadowcast, this method
    // turns off or changes the shadowcasting modes of the various mesh renderers
    // that compose the player's mesh when the player is currently in 2D space
    // and switches their layers for lighting effects
    public void CheckShadowcastModeandLightingChange()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        if (PlayerMovement.shadowShiftingIn || PlayerMovement.shadowShiftingOut)
        {
            foreach (MeshRenderer meshRend in meshRenderers)
            {
                meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
        else
        {
            foreach (MeshRenderer meshRend in meshRenderers)
            {
                meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }

        if(!PlayerMovement.in3DSpace)
        {
            foreach (MeshRenderer meshRend in meshRenderers)
            {
                meshRend.gameObject.layer = LayerMask.NameToLayer("Shadow");
            }
        }
        else if(PlayerMovement.shadowMelded)
        {
            foreach (MeshRenderer meshRend in meshRenderers)
            {
                meshRend.gameObject.layer = LayerMask.NameToLayer("Shadowmeld");
            }
        }
        else
        {
            foreach (MeshRenderer meshRend in meshRenderers)
            {
                meshRend.gameObject.layer = LayerMask.NameToLayer("Form");
            }
        }
    }

    // This method checks all light sources in the "Lighting" gameobject and finds
    // the most closely aligned light source to the camera's rotation, or not if
    // there is current not one, and then returns it and places it inside the 
    // lightSourceAligned object's locations.
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
