using System.Security.Cryptography;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BuddyBotMove : MonoBehaviour
{

    public Vector3 Target;

    private Transform m_Transform;
	private Rigidbody m_Rigidbody;
    private Thruster[] m_Thrusters;

    public float MaximumSafeSpeed = 1f;
    public float LookAheadTime = 1f;
    public float MaximumAcceleration = 5f;

    public void Awake()
    {
        m_Transform = GetComponent<Transform>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Thrusters = GetComponentsInChildren<Thruster>();

        Target = transform.position;
    }

	void FixedUpdate ()
	{
        var targetDelta = Target - m_Transform.position;

	    // Calculate the acceleration needed to put us at the target position in LookAheadTime seconds
	    Vector3 acceleration = 2*(targetDelta - m_Rigidbody.velocity*LookAheadTime)/(LookAheadTime*LookAheadTime);

	    if (targetDelta.y >= 0)
	    {
	        // balance out gravity if we need to go upwards/maintain height - otherwise let us drop
	        acceleration -= Physics.gravity;
	    }

	    // limit maximum acceleration
	    acceleration = Vector3.ClampMagnitude(acceleration, MaximumAcceleration);

        // apply result
        m_Rigidbody.AddForce(acceleration, ForceMode.Acceleration);

	    foreach (var thruster in m_Thrusters)
	    {
	        thruster.Level = Mathf.Clamp01(Vector3.Dot(acceleration, -thruster.transform.forward)/MaximumAcceleration);
	    }
	}
}
