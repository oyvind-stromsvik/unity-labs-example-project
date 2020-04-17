using UnityEngine;

public class LandingDamageSMB : CustomSMB
{
    public float damageRatio = 2f;          // How damage is calculated based on falling speed.
    public float speedThreshold = -5f;      // How fast the character needs to be falling before he takes damage.


    private HealthMonitor health;                                                       // A reference to the health monitor.
    private int m_HashVerticalSpeedPara = Animator.StringToHash ("VerticalSpeed");      // For referencing the VerticalSpeed animator parameter.
    

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get the current vertical speed of the character.
        float verticalSpeed = animator.GetFloat (m_HashVerticalSpeedPara);

        // If the threshold isn't breached return.
        if (verticalSpeed > speedThreshold)
            return;
        
        // If the health monitor hasn't been found already, find it.
		if(!health) health = FindObjectOfType<HealthMonitor> ();

        // calculate the amount of damage to be done and reduce the health by that amount (note verticalSpeed is negative).
        float reduction = verticalSpeed * damageRatio;
        health.healthF += reduction;
    }
}
