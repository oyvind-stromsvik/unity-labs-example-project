using UnityEngine;

public class MidairControlSMB : CustomSMB
{
    public float acceleration = 0.1f;   // How much the air control speed increases by per second.
    public float jogSpeed = 2.822f;     // The maximum speed the character can obtain in the air.


    private Transform m_Transform;      // Reference to the transform component.
    private Rigidbody m_Rigidbody;      // Reference to the rigidbody component.


    public override void Init(Animator anim)
    {
        // Setting up the references.
        m_Transform = anim.transform;
        m_Rigidbody = anim.GetComponent<Rigidbody>();
    }

	
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        // Find how fast the character is moving laterally.
	    float lateralSpeed = new Vector3 (m_Rigidbody.velocity.x, 0f, m_Rigidbody.velocity.z).magnitude;

        // If the character is under the maximum speed...
	    if (lateralSpeed < jogSpeed)
	    {
            // ... find the intended acceleration and the current velocity in local coordinates.
	        Vector3 localAcceleration = new Vector3 (userInput.Horizontal, 0f, userInput.Vertical) * acceleration * Time.deltaTime;
            Vector3 localVelocity = m_Transform.InverseTransformDirection(m_Rigidbody.velocity);

            // Increase the velocity by the acceleration.
	        localVelocity += localAcceleration;

            // Set the velocity based on the modified local velocity.
            m_Rigidbody.velocity = m_Transform.TransformDirection (localVelocity);
	    }
	}
}
