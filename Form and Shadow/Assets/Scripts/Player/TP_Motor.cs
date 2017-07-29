using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_Motor : MonoBehaviour
{
    public static TP_Motor m_Instance;
    [Range(5, 15)][SerializeField] float m_ForwardSpeed = 10f;
    [Range(2, 8)][SerializeField] float m_BackwardSpeed = 2f;
    [Range(5, 15)][SerializeField] float m_StrafingSpeed = 5f;
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


    public void UpdateMotor ()
    {
        SnapAlignCharacterWithCamera();
        ProcessMotion();
	}

    void ProcessMotion()
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
        TP_Controller.m_CharacterController.Move(m_MoveVector * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (m_MoveVector.y > -m_TerminalVelocity)
            m_MoveVector = new Vector3(m_MoveVector.x, m_MoveVector.y - m_Gravity * Time.deltaTime, m_MoveVector.z);

        if(TP_Controller.m_CharacterController.isGrounded && m_MoveVector.y < -1)
            m_MoveVector = new Vector3(m_MoveVector.x, -1, m_MoveVector.z);
    }

    void ApplySlide()
    {
        if (!TP_Controller.m_CharacterController.isGrounded)
        {
            return;
        }

        m_SlideDirection = Vector3.zero;

        RaycastHit hitInfo;

        Debug.DrawRay(transform.position + Vector3.up, Vector3.down, Color.red);

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo))
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
        if (TP_Controller.m_CharacterController.isGrounded)
            m_VerticalVelocity = m_JumpSpeed;
    }

    void SnapAlignCharacterWithCamera()
    {
        if(m_MoveVector.x != 0 || m_MoveVector.z != 0)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    float MoveSpeed()
    {
        var moveSpeed = 0f;
        switch (TP_Animator.m_Instance.m_MoveDirection)
        {
            case TP_Animator.Direction.Stationary:
                moveSpeed = 0;
                break;
            case TP_Animator.Direction.Forward:
                moveSpeed = m_ForwardSpeed;
                break;
            case TP_Animator.Direction.Backward:
                moveSpeed = m_BackwardSpeed;
                break;
            case TP_Animator.Direction.Left:
                moveSpeed = m_StrafingSpeed;
                break;
            case TP_Animator.Direction.Right:
                moveSpeed = m_StrafingSpeed;
                break;
            case TP_Animator.Direction.LeftForward:
                moveSpeed = m_ForwardSpeed;
                break;
            case TP_Animator.Direction.RightForward:
                moveSpeed = m_ForwardSpeed;
                break;
            case TP_Animator.Direction.LeftBackward:
                moveSpeed = m_BackwardSpeed;
                break;
            case TP_Animator.Direction.RightBackward:
                moveSpeed = m_BackwardSpeed;
                break;
            default:
                break;
        }
        if (m_SlideDirection.magnitude > 0)
            moveSpeed = m_SlideSpeed;
        
        return moveSpeed;
    }
}
