//using UnityEngine;
//using System.Collections.Generic;

///*

//    Written by: Daniel Colina and Chris Knoll
//    --PlayerShadowCollider--
//    Attached to the player shadow object and handles the logic of locking
//    behind the wall when the player is in 2D, following the player in 3D,
//    and contains a method called by PlayerMovement that finds platforms to
//    transfer out onto for usage in the shadow shift multi-exit system.

//*/
//public class PlayerShadowCollider : MonoBehaviour
//{
//	private GameObject player;

//    void Start()
//    {
//        player = GameObject.FindGameObjectWithTag("Player");
//    }

//    void Update()
//	{
//        if (NewPlayerShadowInteraction.m_CurrentPlayerState != NewPlayerShadowInteraction.PLAYERSTATE.SHADOW)
//            FollowPlayer();
//	}

//	void FollowPlayer()
//	{
//		transform.position = player.transform.position + Vector3.up * 10;
//	}


//}
