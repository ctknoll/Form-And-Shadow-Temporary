//*******************UPDATED******************

using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerMovementInput : MonoBehaviour
{
    private PlayerMovement pMovement;
    private Transform mainCamera;
    private Vector3 mainCameraForward;
    private Vector3 playerMoveVector;
    private bool playerJump;

	private void Start ()
    {
		if(Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
        pMovement = GetComponent<PlayerMovement>();
    }
	
	private void Update ()
    {
        if (!playerJump)
        {
            playerJump = CrossPlatformInputManager.GetButtonDown("Jump");
        }
	}


    private void FixedUpdate()
    {
        // Get player inputs
        {
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.LeftControl);

            if(mainCamera != null)
            {
                mainCameraForward = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
                playerMoveVector = vertical * mainCameraForward + horizontal * mainCamera.right;
            }

            else
            {
                playerMoveVector = vertical * Vector3.forward + horizontal * Vector3.right;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerMoveVector *= 0.5f;
            }

            pMovement.Move(playerMoveVector, crouch, playerJump);
            playerJump = false;
        }
    }
}
