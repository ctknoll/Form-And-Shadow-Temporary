using UnityEngine;

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
		if(PlayerMovement.in3DSpace && !PlayerMovement.shadowMelded)
			FollowPlayer();
	}

	void LateUpdate () 
	{
		// Prevent the player shadow collider from moving on the Z axis
		//LockZPosition();
	}

	public void FollowPlayer()
	{
		transform.position = player.transform.position;
	}

	//public void LockZPosition()
	//{
	//	Vector3 pos = transform.position + shadowCast.zOffset;
	//	transform.position = pos;
	//}
}
