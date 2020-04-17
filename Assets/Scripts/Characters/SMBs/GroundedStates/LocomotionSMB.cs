using UnityEngine;

public class LocomotionSMB : CustomSMB
{
    public LayerMask groundLayer;                                       // The layer on which the character moves.


    private GroundedManager groundedManager = new GroundedManager();    // Object for handling whether or not the player is grounded.
    private Transform m_Transform;                                      // Reference to the transform component.
    private Transform m_CameraRig;                                      // Reference to the camera rig (the root object of the camera setup).
    private Transform m_Camera;                                         // Reference to the transform of the camera.
    private float m_DeadZoneAngleRad;                                   // Number of radians below which rotation is not animated.


    private readonly int m_HashVSpeedPara = Animator.StringToHash("VSpeed");            // For referencing the VSpeed animator parameter.
    private readonly int m_HashHSpeedPara = Animator.StringToHash("HSpeed");            // For referencing the HSpeed animator parameter.
    private readonly int m_HashAngSpeedPara = Animator.StringToHash("AngularSpeed");    // For referencing the AngularSpeed animator parameter.
    private readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");        // For referencing the Grounded animator parameter.


    private const float k_CamTargetDistance = 1000f;            // The distance of a point from the camera which the camera is looking at.
    private const float k_AngleResponseTime = 0.3f;             // The response time for turning the player based on the camera's rotation.
    private const float k_TurnDamping = 0.05f;                  // The damping applied to AngularSpeed as it is set.
    private const float k_MoveDamping = 0.1f;                   // The damping applied to HSpeed and VSpeed as they are set.
    private const float k_DeadZoneAngleDeg = 1.15f;             // The number of degrees below which rotation is not animated.
    

    public override void Init(Animator anim)
    {
        // Setting up the groundedManager and component references.
        groundedManager.Init(anim, groundLayer);
        m_Transform = anim.transform;
        m_CameraRig = userInput.cameraRig;
        m_Camera = userInput.mainCamera;

        // Setting the dead zone in radians based on the dead zone in degrees.
        m_DeadZoneAngleRad = k_DeadZoneAngleDeg * Mathf.Deg2Rad;
    }


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Cache all the input.
        float v = userInput.Vertical;
        float h = userInput.Horizontal;
        
        // Set the animator parameters based on movement.
        SetMoveParameters(animator, v, h);

        // The player is running if neither h nor v is zero.
        bool moving = Mathf.Abs(h) > 0f || Mathf.Abs(v) > 0f;

        // Handle the rotation of the player when it is stationary.
        TurnPlayer(animator, moving);

        if (!groundedManager.IsGrounded)
        {
            animator.SetBool(m_HashGroundedPara, false);
        }
    }

    void SetMoveParameters(Animator animator, float v, float h)
    {
        Vector2 movement = new Vector2(h, v);
        if(movement.sqrMagnitude > 1f)
            movement.Normalize();

        // Set the horizontal and vertical speed parameters.
        animator.SetFloat(m_HashVSpeedPara, movement.y, k_MoveDamping, Time.deltaTime);
        animator.SetFloat(m_HashHSpeedPara, movement.x, k_MoveDamping, Time.deltaTime);
    }


    void TurnPlayer(Animator animator, bool moving)
    {
        // If the player is moving and in the locomotion state...
        if (moving)
        {
            // ... make the player face the same direction as the camera.
            m_Transform.rotation = m_CameraRig.rotation;
            return;
        }

        // Find a point in world space in front of the camera.
        Vector3 targetPoint = m_Camera.position + m_Camera.forward * k_CamTargetDistance;

        // Find this point relative to the player.
        Vector3 localTargetPoint = m_Transform.InverseTransformPoint(targetPoint);

        // Find the angle between the z axis of the player and this point.
        float angle = Mathf.Atan2(localTargetPoint.x, localTargetPoint.z);

        // If this angle is small...
        if (Mathf.Abs(angle) < m_DeadZoneAngleRad)
        {
            // Set the angle to zero
            angle = 0f;

            // Set the player's rotation to the camera's.
            m_Transform.rotation = m_CameraRig.rotation;
        }

        // Set the angular speed parameter with appropriate damping.
        animator.SetFloat(m_HashAngSpeedPara, angle / k_AngleResponseTime, k_TurnDamping, Time.deltaTime);
    }


    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check whether the character is grounded.
        groundedManager.CheckGroundedWithVelocity();
    }
}
