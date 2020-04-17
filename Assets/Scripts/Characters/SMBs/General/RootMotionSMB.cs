using UnityEngine;

public class RootMotionSMB : CustomSMB
{
    // Enum for defining when root motion should be applied.
    public enum MotionApplication
    {
        ThisStateOnly, ThisStateAndTransitionToIt, ThisStateAndTransitionFromIt, ThisStateAndAllTransitions, None,
    }


    public MotionApplication motionApplication;                         // When root motion should be applied relative to this state.
    public RootMotionManager motionManager = new RootMotionManager();   // Object for controlling the application or root motion.


    public override void Init(Animator anim)
    {
        // Call the setup function of the motion manager.
        motionManager.Init(anim);
    }
    
    
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        // Some booleans to determine where in the state's timeline we are.
	    bool inTransition = animator.IsInTransition(layerIndex);
	    bool transitioningTo = inTransition && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash;
        bool transitioningFrom = inTransition && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash;

        // By default don't apply root motion.
	    bool applyRootMotion = false;

        // If any of the conditions are met for root motion to be applied, set applyRootMotion to true.
	    applyRootMotion |= motionApplication == MotionApplication.ThisStateOnly && !inTransition;
	    applyRootMotion |= motionApplication == MotionApplication.ThisStateAndTransitionToIt && !transitioningFrom;
	    applyRootMotion |= motionApplication == MotionApplication.ThisStateAndTransitionFromIt && !transitioningTo;
        applyRootMotion |= motionApplication == MotionApplication.ThisStateAndAllTransitions;
        
        // If root motion should be applied, do so.
	    if (applyRootMotion)
	    {
	        motionManager.ApplyToRigidbodyAlongSurface();
            motionManager.ApplyRotation();
	    }
	}
}
