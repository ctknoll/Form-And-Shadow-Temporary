﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShadowCollider : MonoBehaviour {
	public GameObject player;
    public Vector3 transformOffset;
    public Transform wallTransform;
    public bool zAxisMovement;

    void Update()
	{
        // If the player is moving in the 3D space, force the shadow collider to follow the main player character
        if (PlayerMovement.in3DSpace && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
			FollowPlayer();
        else
        {
            if(wallTransform != null)
            {
                if (zAxisMovement)
                    transform.position = new Vector3(transform.position.x, transform.position.y, wallTransform.position.z + transformOffset.z);
                else
                    transform.position = new Vector3(wallTransform.position.x + transformOffset.x, transform.position.y, transform.position.z);
            }
        }
	}

	public void FollowPlayer()
	{
		transform.position = player.transform.position + Vector3.up * 10;
	}

	public List<GameObject> GetTransferPlatforms()
	{
		// Cast a ray down from the player shadow and store all colliders hit in an array of RaycastHits
		RaycastHit [] hits;
        hits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.down, GetComponent<CharacterController>().height, 1 << 11);

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
