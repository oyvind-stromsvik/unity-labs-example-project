using UnityEngine;

public class FreezeColliderSMB : CustomSMB
{
    private Rigidbody m_Rigidbody;          // Reference to the rigidbody component.
    private bool entryTransitionFinished;   // Whether the transition to this state has finished or not.
    private bool exitTransitionStarted;     // Whether the transition from this state has started or not.


    public override void Init (Animator anim)
    {
        // Setting up rigidbody reference.
        m_Rigidbody = anim.GetComponent<Rigidbody> ();
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If the entry transition hasn't been recorded as finished but the animator isn't in transition...
        if (!entryTransitionFinished && !animator.IsInTransition (layerIndex))
        {
            // ... then the entry transition has finished so make the rigidbody kinematic.
            entryTransitionFinished = true;
            m_Rigidbody.isKinematic = true;
        }

        // If the exit transition hasn't been recorded as started, but the entry transition has finished and the animator is in transition...
        if (!exitTransitionStarted && entryTransitionFinished && animator.IsInTransition (layerIndex))
        {
            // ... then the exit transition has started so make the rigidbody unkinematic.
            exitTransitionStarted = true;
            m_Rigidbody.isKinematic = false;
        }
    }


    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        // Reset whether the transitions have finished and started.
        entryTransitionFinished = false;
        exitTransitionStarted = false;
    }
}
