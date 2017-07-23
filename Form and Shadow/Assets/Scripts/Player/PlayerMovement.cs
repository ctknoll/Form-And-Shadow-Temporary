//***************UPDATED****************

using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerMovingTurnSpeed = 360;
    [SerializeField] float playerStationaryTurnSpeed = 180;
    [SerializeField] float playerJumpPower = 12f;
    [Range(1f, 4f)] [SerializeField] float playerGravityMultiplier = 2f;
    [SerializeField] float playerRunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    [SerializeField] float playerMoveSpeedMultiplier = 1f;
    [SerializeField] float playerAnimSpeedMultiplier = 1f;
    [SerializeField] float playerGroundCheckDistance = 0.1f;

    Rigidbody playerRigidbody;
    Animator playerAnimator;
    bool playerIsGrounded;
    float playerOrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float playerTurnAmount;
    float playerForwardAmount;
    Vector3 playerGroundNormal;
    float playerCapsuleHeight;
    Vector3 playerCapsuleCenter;
    CapsuleCollider playerCapsule;
    bool playerCrouching;

    

    void Start ()
    {
        playerAnimator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerCapsule = GetComponent<CapsuleCollider>();
        playerCapsuleHeight = playerCapsule.height;
        playerCapsuleCenter = playerCapsule.center;

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        playerOrigGroundCheckDistance = playerGroundCheckDistance;
	}
	
    public void Move(Vector3 moveVector, bool crouch, bool jump)
    {
        // convert the world relative move vector input form the player
        // to a local-relative turn and forward amount to head in the
        // desired direction
        if (moveVector.magnitude > 1f)
            moveVector.Normalize();
        moveVector = transform.InverseTransformDirection(moveVector);
        CheckGroundStatus();
        moveVector = Vector3.ProjectOnPlane(moveVector, playerGroundNormal);
        playerTurnAmount = Mathf.Atan2(moveVector.x, moveVector.z);
        playerForwardAmount = moveVector.z;

        ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne
        if (playerIsGrounded)
            HandleGroundedMovement(crouch, jump);
        else
            HandleAirborneMovement();

        ScaleCapsuleForCrouching(crouch);
        PreventStandingInLowHeadroom();

        UpdateAnimator(moveVector);
    }
    #region Follow Methods
    public void FollowShadow()
    {
        transform.position = GetComponent<NewPlayerShadowInteraction>().m_PlayerShadow.transform.position + -GetComponent<NewPlayerShadowInteraction>().m_LightSourceAligned.GetComponent<LightSourceControl>().lightSourceDirection * 1.5f;
    }

    public void FollowTransitionObject()
    {
        transform.position = GetComponent<NewPlayerShadowInteraction>().m_ShadowShiftFollowObject.transform.position;
    }
    #endregion

    private void UpdateAnimator(Vector3 moveVector)
    {
        playerAnimator.SetFloat("Forward", playerForwardAmount, 0.1f, Time.deltaTime);
        playerAnimator.SetFloat("Turn", playerTurnAmount, 0.1f, Time.deltaTime);
        playerAnimator.SetBool("Crouch", playerCrouching);
        playerAnimator.SetBool("OnGround", playerIsGrounded);

        if (!playerIsGrounded)
            playerAnimator.SetFloat("Jump", playerRigidbody.velocity.y);

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle = Mathf.Repeat(playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime +
            playerRunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * playerForwardAmount;

        if (playerIsGrounded)
            playerAnimator.SetFloat("JumpLeg", jumpLeg);

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (playerIsGrounded && moveVector.magnitude > 0)
            playerAnimator.speed = playerAnimSpeedMultiplier;
        else
            playerAnimator.speed = 1;
    }

    private void PreventStandingInLowHeadroom()
    {
        if(!playerCrouching)
        {
            Ray crouchRay = new Ray(playerRigidbody.position + Vector3.up * playerCapsule.radius * k_Half, Vector3.up);
            float crouchRayLength = playerCapsuleHeight - playerCapsule.radius * k_Half;
            if(Physics.SphereCast(crouchRay, playerCapsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                playerCrouching = true;
            }
        }
    }

    private void ScaleCapsuleForCrouching(bool crouch)
    {
        if(playerIsGrounded && crouch)
        {
            if (playerCrouching)
                return;
            playerCapsule.height = playerCapsule.height / 2f;
            playerCapsule.center = playerCapsule.center / 2f;
            playerCrouching = true;
        }
        else
        {
            Ray crouchRay = new Ray(playerRigidbody.position + Vector3.up * playerCapsule.radius * k_Half, Vector3.up);
            float crouchRayLength = playerCapsuleHeight - playerCapsule.radius * k_Half;
            if(Physics.SphereCast(crouchRay, playerCapsule.radius * k_Half, crouchRayLength, 
                Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                playerCrouching = true;
                return;
            }
            playerCapsule.height = playerCapsuleHeight;
            playerCapsule.center = playerCapsuleCenter;
            playerCrouching = false;
        }
    }

    private void HandleGroundedMovement(bool crouch, bool jump)
    {
        if(jump && !crouch && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, playerJumpPower, playerRigidbody.velocity.z);
            playerIsGrounded = false;
            playerAnimator.applyRootMotion = false;
            playerGroundCheckDistance = 0.1f;
        }
    }

    private void HandleAirborneMovement()
    {
        Vector3 extraGravityForce = (Physics.gravity * playerGravityMultiplier) - Physics.gravity;
        playerRigidbody.AddForce(extraGravityForce);

        playerGroundCheckDistance = playerRigidbody.velocity.y < 0 ? playerOrigGroundCheckDistance : 0.01f;
    }

    private void ApplyExtraTurnRotation()
    {
        // helps the character turn a bit faster (in addition to root motion)
        float turnSpeed = Mathf.Lerp(playerStationaryTurnSpeed, playerMovingTurnSpeed, playerForwardAmount);
        transform.Rotate(0, playerTurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    private void OnAnimatorMove()
    {
        // overrides the base root motion method to modify the positional
        // speed before it's applied
        if(playerIsGrounded && Time.deltaTime > 0)
        {
            Vector3 tempPlayerVelocity = (playerAnimator.deltaPosition * playerMoveSpeedMultiplier) / Time.deltaTime;

            // preserve the existing y part of the current velocity
            tempPlayerVelocity.y = playerRigidbody.velocity.y;
            playerRigidbody.velocity = tempPlayerVelocity;
        }
    }

    private void CheckGroundStatus()
    {
        RaycastHit hit;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * playerGroundCheckDistance));
#endif

        // cast a ray downwards from the character to check grounded state
        if(Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hit, playerGroundCheckDistance))
        {
            playerGroundNormal = hit.normal;
            playerIsGrounded = true;
            playerAnimator.applyRootMotion = true;
        }
        else
        {
            playerIsGrounded = false;
            playerGroundNormal = Vector3.up;
            playerAnimator.applyRootMotion = false;
        }
    }
}
