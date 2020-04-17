using UnityEngine;

public class GroundedManager
{
    public bool IsGrounded {
        get
        {
            if(firstCheckDone)
                return m_grounded;
            return true;
            
        }
        private set { m_grounded = value; }
    }


    Animator m_Animator;
    Transform m_Transform;
    private Rigidbody m_Rigidbody;
    private CapsuleCollider m_Capusle;
    int m_LayerMask;
    private bool firstCheckDone;
    private bool m_grounded;
    const float k_RayLength = 1.4f;
    private const float k_RayError = 0.21f;     // do not lower unless lift speed has been reduced (was 0.21)


    public void Init(Animator anim, int layerMask)
    {
        m_Animator = anim;
        m_Transform = m_Animator.transform;
        m_Rigidbody = anim.GetComponent<Rigidbody>();
        m_Capusle = anim.GetComponent<CapsuleCollider>();
        m_LayerMask = layerMask;
    }


    bool GroundedRaycast(Vector3 origin, Vector3 direction, float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, distance, m_LayerMask))
        {
            return !hit.collider.isTrigger;
        }
        return false;
    }


    public void CheckGrounded()
    {
        Vector3 leftFoot = m_Animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        Vector3 rightFoot = m_Animator.GetIKPosition(AvatarIKGoal.RightFoot);
        Vector3 root = m_Transform.position + (m_Capusle.center.y - m_Capusle.height*0.5f + k_RayError) * Vector3.up;

        IsGrounded = GroundedRaycast (leftFoot, -Vector3.up, k_RayLength);
        IsGrounded |= GroundedRaycast (rightFoot, -Vector3.up, k_RayLength);
        IsGrounded |= GroundedRaycast (root, -Vector3.up, k_RayLength);
        
        firstCheckDone = true;
    }


    public void CheckGroundedWithVelocity()
    {
        Vector3 centre = m_Transform.position + k_RayError * Vector3.up;
        Vector3 velocityDirection = m_Rigidbody.velocity * Time.deltaTime - 2f * k_RayError * Vector3.up;
        Vector3 capuleEdge = m_Transform.position + m_Rigidbody.velocity.normalized * m_Capusle.radius +
                             k_RayError * Vector3.up;
        
        IsGrounded = GroundedRaycast (centre, velocityDirection, velocityDirection.magnitude);
        IsGrounded |= GroundedRaycast (capuleEdge, -Vector3.up, 2f * k_RayError);
        
        firstCheckDone = true;
    }
}
