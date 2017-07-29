using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class TP_Controller : MonoBehaviour
{
    public static CharacterController m_CharacterController;
    public static TP_Controller m_Instance;

    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Instance = this;
    }

    void Update()
    {
        if (Camera.main == null)
            return;

        GetLocomotionInput();
        HandleActionInput();
        TP_Motor.m_Instance.UpdateMotor();
    }

    void GetLocomotionInput()
    {
        var deadZone = 0.1f;

        TP_Motor.m_Instance.m_VerticalVelocity = TP_Motor.m_Instance.m_MoveVector.y;
        TP_Motor.m_Instance.m_MoveVector = Vector3.zero;

        // Check a dead zone around 0 on the axis based on the deadZone variable. If so, change the movement vector
        // to be equal to the axis
        if (CrossPlatformInputManager.GetAxis("Vertical") > deadZone || CrossPlatformInputManager.GetAxis("Vertical") < -deadZone)
            TP_Motor.m_Instance.m_MoveVector += new Vector3(0, 0, CrossPlatformInputManager.GetAxis("Vertical"));

        if (CrossPlatformInputManager.GetAxis("Horizontal") > deadZone || CrossPlatformInputManager.GetAxis("Horizontal") < -deadZone)
            TP_Motor.m_Instance.m_MoveVector += new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), 0, 0);

        TP_Animator.m_Instance.DetermineCurrentMoveDirection();
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
        TP_Motor.m_Instance.Jump();
    }
}
