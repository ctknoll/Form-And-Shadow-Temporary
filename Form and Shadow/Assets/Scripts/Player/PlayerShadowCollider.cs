using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShadowCollider : MonoBehaviour {
	public GameObject player;

	ShadowCast shadowCast;

	void Start()
	{
		shadowCast = GetComponentInParent<ShadowCast>();
	}

	void Update()
	{
		// If the player is moving in the 3D space, force the shadow collider to follow the main player character
		if(PlayerMovement.in3DSpace)
			FollowPlayer();
	}

	public void FollowPlayer()
	{
		transform.position = player.transform.position + Vector3.up * 10;
	}

	public List<GameObject> GetTransferPlatforms()
	{
        //transform.position = player.transform.position + Vector3.up * 10;

		// Cast a ray down from the player shadow and store all colliders hit in an array of RaycastHits
		RaycastHit [] hits;
		hits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.down, Vector3.Distance(transform.position, new Vector3(transform.position.x, 0, transform.position.z)), 1 << 11);

		// Then, create a list of gameobjects and for each RaycastHit in hits, add the hit collider's gameobject to the list of transferPlatforms
		List <GameObject> transferPlatforms = new List<GameObject>();
		foreach (RaycastHit hit in hits)
		{
			if(hit.collider.gameObject.GetComponent<ShadowCollider>().exceptionParent == null)
            {
                if (hit.collider.gameObject.transform.parent.gameObject.tag != "Spikes")
                    transferPlatforms.Add(hit.collider.gameObject.transform.parent.gameObject);
            }
			else
            {
                    transferPlatforms.Add(hit.collider.gameObject.GetComponent<ShadowCollider>().exceptionParent);
            }
				
		}
		// Then, return a list of gameobjects equal to all the shadow colliders below the player when called
		return transferPlatforms;
	}
}
