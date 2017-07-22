using UnityEngine;
using System.Collections.Generic;

/*

    Written by: Daniel Colina and Chris Knoll
    --PlayerShadowCollider--
    Attached to the player shadow object and handles the logic of locking
    behind the wall when the player is in 2D, following the player in 3D,
    and contains a method called by PlayerMovement that finds platforms to
    transfer out onto for usage in the shadow shift multi-exit system.

*/
public class PlayerShadowCollider : MonoBehaviour
{
	public GameObject player;
    public Vector3 transformOffset;
    public Transform wallTransform;
    public bool zAxisMovement;

    void Update()
	{
        // If the player is moving in the 3D space, force the shadow collider to follow the main player character
        if (PlayerShadowInteraction.in3DSpace && !PlayerShadowInteraction.shadowShiftingIn && !PlayerShadowInteraction.shadowShiftingOut)
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
            if(!PlayerShadowInteraction.in3DSpace && !PlayerShadowInteraction.shadowShiftingIn && !PlayerShadowInteraction.shadowShiftingOut)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + gameObject.transform.forward, -gameObject.transform.forward, out hit, Mathf.Infinity, 1 << 10))
                {
                    if(hit.distance <= 1)
                         wallTransform = hit.transform;
                }
                else if (Physics.Raycast(transform.position + gameObject.transform.forward, -gameObject.transform.forward, out hit, Mathf.Infinity, 1 << 12))
                {
                    if (hit.distance <= 1)
                        wallTransform = null;
                }
            }
        }     
	}

	public void FollowPlayer()
	{
		transform.position = player.transform.position + Vector3.up * 10;
	}

	public List<GameObject> GetTransferPlatforms()
	{
        List<GameObject> transferPlatforms = new List<GameObject>();
        // Cast a ray down from the player shadow and store all shadow colliders hit in an array of RaycastHits
        RaycastHit firstPlatformHit;
        if(Physics.SphereCast(transform.position, 0.5f, Vector3.down, out firstPlatformHit, transform.position.y, 1 << 11, QueryTriggerInteraction.Collide))
        {
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(firstPlatformHit.point - new Vector3(0, 0.5f, 0), 0.5f, Vector3.down, GetComponent<CharacterController>().height / 2, 1 << 11, QueryTriggerInteraction.Collide);
            // Then, create a list of gameobjects and for each RaycastHit in hits, add the hit collider's gameobject to the list of transferPlatforms
            foreach (RaycastHit hit in hits)
            {
                // Prevent spikes from being added as shadow collider objects
                if (hit.collider.GetComponentInParent<ShadowCollider>().transformParent.GetComponent<ShadowCast>().shadowType != ShadowCast.ShadowType.SPIKES)
                    transferPlatforms.Add(hit.collider.gameObject.GetComponentInParent<ShadowCollider>().transformParent);
            }
        }
		// Then, return a list of gameobjects equal to all the shadow colliders below the player when called
		return transferPlatforms;
	}
}
