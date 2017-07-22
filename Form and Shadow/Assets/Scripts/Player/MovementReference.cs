//using UnityEngine;

///*
//    Written by: Daniel Colina
//    --Movement Reference--
//    Basic script attached to the player movement reference
//    object on the Player_Character prefab that rotates the object's
//    y rotation to be equal to the camera, and then PlayerMovement
//    moves based on this object's rotation, rather than the camera
//    to prevent relative camera movement.

//*/

//public class MovementReference : MonoBehaviour {
//	private GameObject mainCamera;
//    void Start()
//    {
//        mainCamera = GetComponentInParent<PlayerShadowInteraction>().mainCamera;
//    }
//	void Update () 
//	{
//		transform.eulerAngles = new Vector3(0, mainCamera.transform.eulerAngles.y, 0);
//	}
//}
