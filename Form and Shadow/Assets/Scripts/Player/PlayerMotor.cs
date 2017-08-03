﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public static PlayerMotor m_Instance;
    [Range(2, 10)][SerializeField] float m_ForwardSpeed = 4f;
    [Range(1, 5)][SerializeField] float m_BackwardSpeed = 1f;
    [Range(2, 10)][SerializeField] float m_StrafingSpeed = 4f;
    [Range(2, 7)][SerializeField] float m_SlideSpeed = 10f;
    [Range(4, 10)][SerializeField] float m_JumpSpeed = 6f;
    [Range(15, 25)] public float m_Gravity = 21f;
    [HideInInspector] public float m_TerminalVelocity = 20f;
    [SerializeField] float m_SlideThreshold = 0.8f;
    [SerializeField] float m_MaxControllableSlideMagnitude = 0.4f;
    [HideInInspector] public Vector3 m_MoveVector;
    [HideInInspector] public float m_VerticalVelocity;

    Vector3 m_SlideDirection;

    void Start ()
    {
        m_Instance = this;
	}


    public void UpdateMovement ()
    {
        switch(NewPlayerShadowInteraction.m_CurrentPlayerState)
        {
            case NewPlayerShadowInteraction.PLAYERSTATE.FORM:
                SnapAlignCharacterWithCamera3D();
                Process3DMotion();
                break;
            case NewPlayerShadowInteraction.PLAYERSTATE.SHADOW:
                SnapAlignCharacterWithCamera2D();
                Process2DMotion();
                PlayerFollowShadow();
                break;
            default:
                break;

        }
	}

    void Process3DMotion()
    {
        // Transform MoveVector into world space relative to character's rotation
        m_MoveVector = transform.TransformDirection(m_MoveVector);
        // Normalize MoveVector if Magnitude > 1
        if (m_MoveVector.magnitude > 1) 
            m_MoveVector = m_MoveVector.normalized;

        // Apply sliding if applicable
        ApplySlide();

        // Multiply normalized MoveVector by MoveSpeed;
        m_MoveVector *= MoveSpeed();

        // Reapply VerticalVelocity to MoveVector.y
        m_MoveVector = new Vector3(m_MoveVector.x, m_VerticalVelocity, m_MoveVector.z);

        // Apply gravity
        ApplyGravity();

        // Move the CharacterController in world space using the MoveVector
        PlayerController.m_CharacterController.Move(m_MoveVector * Time.deltaTime);
    }

    void Process2DMotion()
    {
        // Transform MoveVector into world space relative to character's rotation
        // Normalize MoveVector if Magnitude > 1
        if (m_MoveVector.magnitude > 1)
            m_MoveVector = m_MoveVector.normalized;

        // Apply sliding if applicable
        ApplySlide();

        // Multiply normalized MoveVector by MoveSpeed;
        m_MoveVector *= m_StrafingSpeed;

        // Reapply VerticalVelocity to MoveVector.y
        m_MoveVector = new Vector3(m_MoveVector.x, m_VerticalVelocity, m_MoveVector.z);

        // Apply gravity
        ApplyGravity();

        // Move the CharacterController in world space using the MoveVector
        PlayerController.m_CharacterController.Move(m_MoveVector * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (m_MoveVector.y > -m_TerminalVelocity)
            m_MoveVector = new Vector3(m_MoveVector.x, m_MoveVector.y - m_Gravity * Time.deltaTime, m_MoveVector.z);

        if(PlayerController.m_CharacterController.isGrounded && m_MoveVector.y < -1)
            m_MoveVector = new Vector3(m_MoveVector.x, -1, m_MoveVector.z);
    }

    void ApplySlide()
    {
        if (!PlayerController.m_CharacterController.isGrounded)
        {
            return;
        }

        m_SlideDirection = Vector3.zero;

        RaycastHit hitInfo;

        Debug.DrawRay(PlayerController.m_CharacterController.transform.position + Vector3.up, Vector3.down, Color.red);

        if (Physics.Raycast(PlayerController.m_CharacterController.transform.position + Vector3.up, Vector3.down, out hitInfo))
        {
            if(hitInfo.normal.y < m_SlideThreshold)
            {
                m_SlideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);
            }
        }

        if (m_SlideDirection.magnitude < m_MaxControllableSlideMagnitude)
            m_MoveVector += m_SlideDirection;
        else
        {
            m_MoveVector = m_SlideDirection;
        }
    }

    public void Jump()
    {
        if (PlayerController.m_CharacterController.isGrounded)
            m_VerticalVelocity = m_JumpSpeed;
    }

    void SnapAlignCharacterWithCamera3D()
    {
        if(m_MoveVector.x != 0 || m_MoveVector.z != 0)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    void SnapAlignCharacterWithCamera2D()
    {
        if(m_MoveVector.x > 0)
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.right, Camera.main.transform.up);
        if (m_MoveVector.x < 0)
            transform.rotation = Quaternion.LookRotation(-Camera.main.transform.right, Camera.main.transform.up);
    }

    float MoveSpeed()
    {
        var moveSpeed = 0f;
        switch (PlayerAnimator.m_Instance.m_MoveDirection)
        {
            case PlayerAnimator.Direction.Stationary:
                moveSpeed = 0;
                break;
            case PlayerAnimator.Direction.Forward:
                moveSpeed = m_ForwardSpeed;
                break;
            case PlayerAnimator.Direction.Backward:
                moveSpeed = m_BackwardSpeed;
                break;
            case PlayerAnimator.Direction.Left:
                moveSpeed = m_StrafingSpeed;
                break;
            case PlayerAnimator.Direction.Right:
                moveSpeed = m_StrafingSpeed;
                break;
            case PlayerAnimator.Direction.LeftForward:
                moveSpeed = m_ForwardSpeed;
                break;
            case PlayerAnimator.Direction.RightForward:
                moveSpeed = m_ForwardSpeed;
                break;
            case PlayerAnimator.Direction.LeftBackward:
                moveSpeed = m_BackwardSpeed;
                break;
            case PlayerAnimator.Direction.RightBackward:
                moveSpeed = m_BackwardSpeed;
                break;
            default:
                break;
        }
        if (m_SlideDirection.magnitude > 0)
            moveSpeed = m_SlideSpeed;
        return moveSpeed;
    }

    void PlayerFollowShadow()
    {
        transform.position = NewPlayerShadowInteraction.m_PlayerShadow.transform.position + -GetComponent<NewPlayerShadowInteraction>().m_LightSourceAligned.GetComponent<LightSourceControl>().lightSourceDirection * 4;
    }

    void ShadowFollowPlayer()
    {
        NewPlayerShadowInteraction.m_PlayerShadow.transform.position = transform.position + Vector3.up * 10;
    }
}
