using System;
using UnityEngine;

[Serializable]
public class RootMotionManager
{
    // A struct for keeping the information about the surface the character is on.
    public struct SurfaceInfo
    {
        public RaycastHit currentHit;
        public RaycastHit predictHit;
        public Vector3 currentMovement;
        public Vector3 predictMovement;


        public SurfaceInfo(RaycastHit currentHit, RaycastHit predictHit, Vector3 currentMovement, Vector3 predictMovement)
        {
            this.currentHit = currentHit;
            this.predictHit = predictHit;
            this.currentMovement = currentMovement;
            this.predictMovement = predictMovement;
        }
    }


    public float motionMultiplier = 1f;             // Percent of root motion application, 0.5 is half speed, 2 is double speed.
    public bool overrideVerticalMotion = true;      // Whether or not the root motion overrides any vertical velocity the character has.


    private Animator m_Animator;                    // Reference to the animator component.
    private Transform m_Transform;                  // Reference to the transform component.
    private Rigidbody m_Rigidbody;                  // Reference to the rigidbody component.
    private CapsuleCollider m_Capsule;              // Reference to the capsule collider.
    private LayerMask mask;                         // The layer which root motion should be applied tangentially too.


    private const float k_RayDistance = 1.5f;       // The distance of the raycasts for detecting the surface the character is on.
    private const float k_ErrorMargin = 0.01f;      // A small margin to avoid any errors when casting rays and detecting surfaces.

    
    public void Init(Animator animator)
    {
        // Setup all references
        m_Animator = animator;
        m_Transform = animator.GetComponent<Transform>();
        m_Rigidbody = animator.GetComponent<Rigidbody>();
        m_Capsule = animator.GetComponent<CapsuleCollider>();

        // Set the mask to the Collision layer.
        mask = LayerMask.GetMask("Collision");
    }


    private SurfaceInfo GetSurfaceInfo()
    {
        RaycastHit currentHit;
        RaycastHit predictHit;

        // Raycast straight down from the character's position.
        Ray ray = new Ray(m_Transform.position + Vector3.up, -Vector3.up);
        Physics.Raycast(ray, out currentHit, k_RayDistance, mask);
        
        // Raycast straight down from the forward edge of the character's capsule.
        ray.origin += m_Animator.deltaPosition.normalized * (m_Capsule.radius + k_ErrorMargin);
        Physics.Raycast(ray, out predictHit, k_RayDistance, mask);
        
        // Create vectors of motion along the planes where the character currently is and where it's going.
        float deltaPosMag = m_Animator.deltaPosition.magnitude;
        Vector3 currentMotion = Vector3.ProjectOnPlane(m_Animator.deltaPosition, currentHit.normal).normalized * deltaPosMag;
        Vector3 predictMotion = Vector3.ProjectOnPlane(m_Animator.deltaPosition, predictHit.normal).normalized * deltaPosMag;

        // Return the information about the surface.
        return new SurfaceInfo(currentHit, predictHit, currentMotion, predictMotion);
    }


    Vector3 HandleStepUp(Vector3 stepHitPoint)
    {
        // Find the nearest point to the step, on the collider's surface.
        Vector3 stepContactPoint = m_Capsule.ClosestPointOnBounds(stepHitPoint);

        // Find a vector from the character's current position to that point on the capsule.
        Vector3 rootToContact = stepContactPoint - m_Transform.position;

        // The movement is along that vector with a magnitude of the original movement.
        Vector3 movement = rootToContact.normalized * m_Animator.deltaPosition.magnitude;

        return movement;
    }


    private void RigidbodyApplication(Vector3 deltaPosition)
    {
        // If the vertical motion shouldn't be overridden...
        if (!overrideVerticalMotion)
            // ... the delta position's y component should just be the distance moved by velocity (any motion multiplication needs to be canceled out).
            deltaPosition.y = m_Rigidbody.velocity.y * Time.deltaTime / motionMultiplier;

        // Set the velocity to the intended distance to travel over the delta time.
        m_Rigidbody.velocity = deltaPosition*motionMultiplier/Time.deltaTime;
    }

    
    public void ApplyToRigidbodyAlongSurface()
    {
        // Get the information about the surface the character is on.
        SurfaceInfo info = GetSurfaceInfo();

        // Calculate the height difference between where the character currently is and where the forward edge of the collider is.
        float heightDiff = info.predictHit.point.y - info.currentHit.point.y;

        // If this height difference is small, the character is on a flat surface so apply the normal root motion.
        if (heightDiff < k_ErrorMargin && heightDiff > -k_ErrorMargin)
        {
            RigidbodyApplication(m_Animator.deltaPosition);
            return;
        }

        // If the movement suggested at the edge of the collider has a positive y component,
        // then the character is about to go up a slope and this movement should be applied.
        if (info.predictMovement.y > k_ErrorMargin)
        {
            RigidbodyApplication(info.predictMovement);
            return;
        }

        // If the movement suggested for the character's current position has a positive y component (but the predicted movement doesn't),
        // then the character is coming to the top of a slope but needs to make it over the top.
        if (info.currentMovement.y > k_ErrorMargin)
        {
            RigidbodyApplication(info.currentMovement);
            return;
        }

        // If the current position is on a down slope, or the collider edge position is on a down slope, but not both,
        // then the character is on a down slope that's about to be flat or on a flat that's about to be a down slope so normal motion should be applied.
        if (info.currentMovement.y < -k_ErrorMargin ^ info.predictMovement.y < -k_ErrorMargin)
        {
            RigidbodyApplication(m_Animator.deltaPosition);
            return;
        }

        // If the current position is on a down slope and the collider edge position is on a down slope,
        // then apply the movement that has the least negative vertical motion.
        if (info.currentMovement.y < -k_ErrorMargin && info.predictMovement.y < -k_ErrorMargin)
        {
            Vector3 movement = info.currentMovement.y < info.predictMovement.y ? info.predictMovement : info.currentMovement;
            RigidbodyApplication(movement);
            return;
        }

        // If the height difference is negative then there's a step down so motion should be applied normally to step off.
        if (heightDiff < 0f)
        {
            RigidbodyApplication(m_Animator.deltaPosition);
            return;
        }

        // The only remaining possibility is that there is an up step, as such handle the step movement and apply movement appropriately.
        Vector3 stepMovement = HandleStepUp(info.predictHit.point);
        RigidbodyApplication(stepMovement);
    }


    public void ApplyRotation()
    {
        // Rotate the character by the deltaRotation.
        m_Transform.rotation *= m_Animator.deltaRotation;
    }
}
