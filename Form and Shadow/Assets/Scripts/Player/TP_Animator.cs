using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_Animator : MonoBehaviour
{
    public enum Direction
    {
        Stationary, Forward, Backward, Left, Right, LeftForward, RightForward, LeftBackward, RightBackward
    }

    public enum CharacterAnimationState
    {
        Idle, Running, WalkingBackward, StrafingLeft, StrafingRight, Jumping, Falling, Landing, Climbing, 
        Sliding, Using, Dead, ActionLocked
    }

    public static TP_Animator m_Instance;

    public Direction m_MoveDirection { get; set; }
    public CharacterAnimationState m_CurrentAnimationState { get; set; }

    void Awake()
    {
        m_Instance = this;
    }

	void Start ()
    {
		
	}
	
	void Update ()
    {
        DetermineCurrentAnimationState();
        ProcessCurrentAnimationState();
        Debug.Log("Current Character State: " + m_CurrentAnimationState.ToString());
	}

    public void DetermineCurrentMoveDirection()
    {
        var forward = false;
        var backward = false;
        var left = false;
        var right = false;

        if (TP_Motor.m_Instance.m_MoveVector.z > 0)
            forward = true;
        if (TP_Motor.m_Instance.m_MoveVector.z < 0)
            backward = true;
        if (TP_Motor.m_Instance.m_MoveVector.x > 0)
            right = true;
        if (TP_Motor.m_Instance.m_MoveVector.x < 0)
            left = true;

        if(forward)
        {
            if (left)
                m_MoveDirection = Direction.LeftForward;
            else if (right)
                m_MoveDirection = Direction.RightForward;
            else
                m_MoveDirection = Direction.Forward;
        }
        else if(backward)
        {
            if (left)
                m_MoveDirection = Direction.LeftBackward;
            else if (right)
                m_MoveDirection = Direction.RightBackward;
            else
                m_MoveDirection = Direction.Backward;
        }
        else if(left)
            m_MoveDirection = Direction.Left;
        else if(right)
            m_MoveDirection = Direction.Right;
        else
            m_MoveDirection = Direction.Stationary;
    }

    void DetermineCurrentAnimationState()
    {
        if (m_CurrentAnimationState == CharacterAnimationState.Dead)
            return;
        if(!TP_Controller.m_CharacterController.isGrounded)
        {
            if(m_CurrentAnimationState != CharacterAnimationState.Falling && 
                m_CurrentAnimationState != CharacterAnimationState.Jumping && 
                m_CurrentAnimationState != CharacterAnimationState.Landing)
            {
                // We should be falling
            }
        }

        if(m_CurrentAnimationState != CharacterAnimationState.Falling && 
            m_CurrentAnimationState != CharacterAnimationState.Jumping && 
            m_CurrentAnimationState != CharacterAnimationState.Landing && 
            m_CurrentAnimationState != CharacterAnimationState.Using &&
            m_CurrentAnimationState != CharacterAnimationState.Climbing && 
            m_CurrentAnimationState != CharacterAnimationState.Sliding)
        {
            switch(m_MoveDirection)
            {
                case Direction.Stationary:
                    m_CurrentAnimationState = CharacterAnimationState.Idle;
                    break;
                case Direction.Forward:
                    m_CurrentAnimationState = CharacterAnimationState.Running;
                    break;
                case Direction.Backward:
                    m_CurrentAnimationState = CharacterAnimationState.WalkingBackward;
                    break;
                case Direction.Left:
                    m_CurrentAnimationState = CharacterAnimationState.StrafingLeft;
                    break;
                case Direction.Right:
                    m_CurrentAnimationState = CharacterAnimationState.StrafingRight;
                    break;
                case Direction.LeftForward:
                    m_CurrentAnimationState = CharacterAnimationState.Running;
                    break;
                case Direction.RightForward:
                    m_CurrentAnimationState = CharacterAnimationState.Running;
                    break;
                case Direction.LeftBackward:
                    m_CurrentAnimationState = CharacterAnimationState.WalkingBackward;
                    break;
                case Direction.RightBackward:
                    m_CurrentAnimationState = CharacterAnimationState.WalkingBackward;
                    break;
            }
        }
    }

    void ProcessCurrentAnimationState()
    {
        switch(m_CurrentAnimationState)
        {
            case CharacterAnimationState.Idle:
                break;
            case CharacterAnimationState.Running:
                break;
            case CharacterAnimationState.WalkingBackward:
                break;
            case CharacterAnimationState.StrafingLeft:
                break;
            case CharacterAnimationState.StrafingRight:
                break;
            case CharacterAnimationState.Jumping:
                break;
            case CharacterAnimationState.Falling:
                break;
            case CharacterAnimationState.Landing:
                break;
            case CharacterAnimationState.Climbing:
                break;
            case CharacterAnimationState.Sliding:
                break;
            case CharacterAnimationState.Using:
                break;
            case CharacterAnimationState.Dead:
                break;
            case CharacterAnimationState.ActionLocked:
                break;
        }
    }
}
