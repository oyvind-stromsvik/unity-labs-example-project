using UnityEngine;

public class JumpingActivationSMB : CustomSMB
{
	public LayerMask groundLayer;                                                       // The layer on which the character needs to jump.


    private GroundedManager m_GroundedManager = new GroundedManager();                  // Object for handling whether the character is grounded.


    private readonly int m_HashJumpPara = Animator.StringToHash("Jump");                // For referencing the Jump animator parameter.
    private readonly int m_HashPivotWeightPara = Animator.StringToHash("PivotWeight");  // For referencing the PivotWeight animator parameter.
    private readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");        // For referencing the Grounded animator parameter.
    private readonly int m_HashMidairTag = Animator.StringToHash ("Midair");            // For referencing the animator states tagged Midair.


    public override void Init(Animator anim)
    {
        // Setting up the grounded manager.
        m_GroundedManager.Init(anim, groundLayer);
    }


	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        // Whether or not the character is transitioning to a jump already.
	    bool alreadyJumping = animator.GetNextAnimatorStateInfo (layerIndex).tagHash == m_HashMidairTag;

        // If the user has pressed jump, the character is grounded and hasn't already started a transition to jumping...
	    if (userInput.JumpDown && m_GroundedManager.IsGrounded && !alreadyJumping)
	    {
            //... set the Jump, PivotWeight and Grounded animator parameters.
	        animator.SetTrigger(m_HashJumpPara);
            animator.SetFloat(m_HashPivotWeightPara, animator.pivotWeight);
            animator.SetBool(m_HashGroundedPara, false);
	    }
	}

	
	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check whether the character is grounded.
	    m_GroundedManager.CheckGroundedWithVelocity ();
	}
}
