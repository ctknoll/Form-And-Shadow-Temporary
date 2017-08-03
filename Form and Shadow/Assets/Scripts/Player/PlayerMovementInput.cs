////*******************UPDATED******************

//using UnityEngine;
//using UnityStandardAssets.CrossPlatformInput;

//[RequireComponent(typeof(PlayerMovement))]
//public class PlayerMovementInput : MonoBehaviour
//{
//    private PlayerMovement pMovement;
//    private Transform mainCamera;
//    private Vector3 mainCameraForward;
//    private Vector3 playerMoveVector;
//    private bool playerJump;

//	private void Start ()
//    {
//		if(Camera.main != null)
//        {
//            mainCamera = Camera.main.transform;
//        }
//        pMovement = GetComponent<PlayerMovement>();
//    }
	
//	private void Update ()
//    {
//        if (!playerJump)
//        {
//            playerJump = CrossPlatformInputManager.GetButtonDown("Jump");
//        }
//	}


//    void FixedUpdate()
//    {
//        // Get player inputs
//        {
//            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
//            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

//            if(mainCamera != null)
//            {
//                mainCameraForward = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
//                playerMoveVector = vertical * mainCameraForward + horizontal * mainCamera.right;
//            }

//            else
//            {
//                playerMoveVector = vertical * Vector3.forward + horizontal * Vector3.right;
//            }

//            pMovement.Move(playerMoveVector, playerJump);
//            playerJump = false;
//        }
//    }
//}
