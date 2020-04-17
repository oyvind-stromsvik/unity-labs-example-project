using UnityEngine;

public class VerticalSpeedSMB : CustomSMB
{
    private Rigidbody m_Rigidbody;          // Reference to the rigidbody component.
    private float m_FastestVerticalSpeed;   // The largest negative vertical speed so far recorded.


    private readonly int m_HashVerticalSpeedPara = Animator.StringToHash("VerticalSpeed");      // For referencing the VerticalSpeed animator parameter.


    public override void Init(Animator anim)
    {
        // Setting the rigidbody reference.
        m_Rigidbody = anim.GetComponent<Rigidbody>();
    }


    public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Initially the fastest vertical speed is the current vertical speed.
        m_FastestVerticalSpeed = m_Rigidbody.velocity.y;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If the current vertical speed is lower than was recorded last frame, record a new one.
        if (m_Rigidbody.velocity.y < m_FastestVerticalSpeed)
            m_FastestVerticalSpeed = m_Rigidbody.velocity.y;

        // Set the vertical speed animator parameter.
        animator.SetFloat(m_HashVerticalSpeedPara, m_FastestVerticalSpeed);
	}
}
