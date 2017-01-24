using UnityEngine;

public class PlayerShadowCollider : MonoBehaviour {
	public GameObject player;

	void Update()
	{
		// If the player is moving in the 3D space, force the shadow collider to follow the main player character
		if(PlayerMovement.in3DSpace && !PlayerMovement.shadowMelded)
			FollowPlayer();
	}

	void LateUpdate () 
	{
		// Prevent the player shadow collider from moving on the Z axis
		LockZPosition();
	}

	public void FollowPlayer()
	{
		transform.position = player.transform.position;
	}

	public void LockZPosition()
	{
		Vector3 pos = transform.position;
		pos.z = player.GetComponent<PlayerShadowCast>().zOffset - 0.1f;
		transform.position = pos;
	}
}
