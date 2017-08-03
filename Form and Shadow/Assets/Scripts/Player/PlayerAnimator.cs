using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public enum Direction
    {
        Stationary, Forward, Backward, Left, Right, LeftForward, RightForward, LeftBackward, RightBackward
    }

    public Direction m_MoveDirection { get; set; }

    public static PlayerAnimator m_Instance;
    private Animator animator;


    void Awake()
    {
        m_Instance = this;
    }

	void Start ()
    {
        animator = GetComponent<Animator>();
	}
	
	void Update ()
    {
        switch(PlayerShadowInteraction.m_CurrentPlayerState)
        {
            case PlayerShadowInteraction.PLAYERSTATE.FORM:
                if(animator.GetBool("PlayerInShadow"))
                    animator.SetBool("PlayerInShadow", false);
                animator.SetFloat("VSpeed", Input.GetAxis("Vertical"), 0.1f, Time.deltaTime);
                animator.SetFloat("HSpeed", Input.GetAxis("Horizontal"), 0.1f, Time.deltaTime);
                break;
            case PlayerShadowInteraction.PLAYERSTATE.SHADOW:
                if (!animator.GetBool("PlayerInShadow"))
                    animator.SetBool("PlayerInShadow", true);
                animator.SetFloat("HSpeed", Input.GetAxis("Horizontal"));
                break;
        }

        animator.SetBool("IsGrounded", PlayerController.m_CharacterController.isGrounded);
        if (!PlayerController.m_CharacterController.isGrounded)
            animator.SetFloat("Jump", PlayerMotor.m_Instance.m_MoveVector.y);
    }

    public void DetermineCurrentMoveDirection()
    {
        var forward = false;
        var backward = false;
        var left = false;
        var right = false;

        if (PlayerMotor.m_Instance.m_MoveVector.z > 0)
            forward = true;
        if (PlayerMotor.m_Instance.m_MoveVector.z < 0)
            backward = true;
        if (PlayerMotor.m_Instance.m_MoveVector.x > 0)
            right = true;
        if (PlayerMotor.m_Instance.m_MoveVector.x < 0)
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
}
