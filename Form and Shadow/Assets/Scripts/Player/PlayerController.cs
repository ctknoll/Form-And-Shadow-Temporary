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
        switch(NewPlayerShadowInteraction.m_CurrentPlayerState)
        {
            case NewPlayerShadowInteraction.PLAYERSTATE.FORM:
                Get3DLocomotionInput();
                PlayerMotor.m_Instance.Update3DMovement();
                break;
            case NewPlayerShadowInteraction.PLAYERSTATE.SHADOW:
                Get2DLocomotionInput();
                break;
        }
        HandleActionInput();
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

        if (CrossPlatformInputManager.GetAxis("Horizontal") > deadZone || CrossPlatformInputManager.GetAxis("Horizontal") < -deadZone)
        {
            if(GetComponent<NewPlayerShadowInteraction>().)
            PlayerMotor.m_Instance.m_MoveVector += new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), 0, 0);
        }
    }
    void HandleActionInput()
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
