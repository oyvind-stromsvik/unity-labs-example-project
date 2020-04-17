using UnityEngine;

public class OcclusionPortalSMB : CustomSMB
{
    private OcclusionPortal portal;


    public override void Init (Animator anim)
    {
        portal = anim.GetComponent<OcclusionPortal> ();
    }


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        // Whilst the door is closed, the occulusion portal should be closed.
		if (portal)
			portal.open = false;
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        // Whilst the door is open, the occulusion portal should be open.
		if (portal)
			portal.open = true;
	}
}
