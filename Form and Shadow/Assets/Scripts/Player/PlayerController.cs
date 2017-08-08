using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    public static CharacterController m_CharacterController;
    public static PlayerController m_Instance;

    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Instance = this;
    }

    void Update()
    {
        if (Camera.main == null)
            return;
        switch(PlayerShadowInteraction.m_CurrentPlayerState)
        {
            case PlayerShadowInteraction.PLAYERSTATE.FORM:
                Get3DLocomotionInput();
                GetJumpInput();
                break;
            case PlayerShadowInteraction.PLAYERSTATE.SHADOW:
                Get2DLocomotionInput();
                GetJumpInput();
                break;
            case PlayerShadowInteraction.PLAYERSTATE.GRABBING:
                GetGrabbingLocomotionInput();
                break;
            default:
                break;
        }
        PlayerMotor.m_Instance.UpdateMovement();
    }

    void Get3DLocomotionInput()
    {
        var deadZone = 0.1f;

        PlayerMotor.m_Instance.m_VerticalVelocity = PlayerMotor.m_Instance.m_MoveVector.y;
        PlayerMotor.m_Instance.m_MoveVector = Vector3.zero;

        // Check a dead zone around 0 on the axis based on the deadZone variable. If so, change the movement vector
        // to be equal to the axis
        if (CrossPlatformInputManager.GetAxis("Vertical") > deadZone || CrossPlatformInputManager.GetAxis("Vertical") < -deadZone)
            PlayerMotor.m_Instance.m_MoveVector += new Vector3(0, 0, CrossPlatformInputManager.GetAxis("Vertical"));

        if (CrossPlatformInputManager.GetAxis("Horizontal") > deadZone || CrossPlatformInputManager.GetAxis("Horizontal") < -deadZone)
            PlayerMotor.m_Instance.m_MoveVector += new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), 0, 0);

        PlayerAnimator.m_Instance.DetermineCurrentMoveDirection();
    }

    void Get2DLocomotionInput()
    {
        var deadZone = 0.1f;

        PlayerMotor.m_Instance.m_VerticalVelocity = PlayerMotor.m_Instance.m_MoveVector.y;
        PlayerMotor.m_Instance.m_MoveVector = Vector3.zero;

        if (CrossPlatformInputManager.GetAxis("Horizontal") > deadZone)
            PlayerMotor.m_Instance.m_MoveVector += Camera.main.transform.right;
        if (CrossPlatformInputManager.GetAxis("Horizontal") < -deadZone)
            PlayerMotor.m_Instance.m_MoveVector += -Camera.main.transform.right;
    }

    void GetGrabbingLocomotionInput()
    {
        var deadZone = 0.1f;

        PlayerMotor.m_Instance.m_VerticalVelocity = PlayerMotor.m_Instance.m_MoveVector.y;
        PlayerMotor.m_Instance.m_MoveVector = Vector3.zero;

        if (CrossPlatformInputManager.GetAxis("Vertical") > deadZone)
            PlayerMotor.m_Instance.m_MoveVector += transform.forward;
        if (CrossPlatformInputManager.GetAxis("Vertical") < -deadZone)
            PlayerMotor.m_Instance.m_MoveVector -= transform.forward;
    }

    void GetJumpInput()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    void Jump()
    {
        PlayerMotor.m_Instance.Jump();
    }
}
