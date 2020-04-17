using UnityEngine;

public class FallingSMB : CustomSMB
{
    public LayerMask groundLayer;       // The layer one which the character is grounded.


    private GroundedManager m_GroundedManager = new GroundedManager();  // Object to manager the character being grounded.
    private Rigidbody m_Rigidbody;                                      // Reference to the rigidbody component.


    private readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");    // For referencing the Grounded animator parameter.


    private const float k_SpeedMargin = 0.1f;           // Margin below which grounded can become true.
    

    public override void Init(Animator anim)
    {
        // Setting up the grounded manager and rigidbody reference.
        m_GroundedManager.Init(anim, groundLayer);
        m_Rigidbody = anim.GetComponent<Rigidbody>();
    }


    public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Make sure feet are not being stabilised in midair.
        //animator.stabilizeFeet = false;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If the character is grounded, the animator is not in transition and the velocity is low enough, set the Grounded animator parameter.
	    if (m_GroundedManager.IsGrounded && !animator.IsInTransition(layerIndex) && m_Rigidbody.velocity.y < k_SpeedMargin)
            animator.SetBool(m_HashGroundedPara, true);
	}

	
	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check if the character is grounded.
	    m_GroundedManager.CheckGroundedWithVelocity();
	}
}
