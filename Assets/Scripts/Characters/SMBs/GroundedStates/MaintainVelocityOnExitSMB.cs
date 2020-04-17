using UnityEngine;

public class MaintainVelocityOnExitSMB : CustomSMB
{
   private Rigidbody m_Rigidbody;      // Reference to the rigidbody component.
    private bool m_ExitHappened;        // Whether either OnStateExit has been called yet.
    private Vector3 m_Velocity;         // For storing the velocity.


    public override void Init (Animator anim)
    {
        // Get a reference to the rigidbody component.
        m_Rigidbody = anim.GetComponent<Rigidbody> ();
    }


    public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset whether exit has happened or not.
        m_ExitHappened = false;
    }


    public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Store the most recent velocity.
        m_Velocity = m_Rigidbody.velocity;
    }


    public override void OnStateMove (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If the exit has happened, this is the last OnStateMove so set the velocity.
        if (m_ExitHappened)
            m_Rigidbody.velocity = m_Velocity;
    }


    public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Exit has happened.
        m_ExitHappened = true;
    }
}
