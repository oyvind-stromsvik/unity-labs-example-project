using UnityEngine;

public class TimeSlowingSMB : CustomSMB
{
    [Range(0.001f, 1f)]
    public float slowedTimeScale = 0.3f;        // The timescale for when time is slowed down.
    public float maxDistanceFromGround = 2f;    // The distance from the ground before time speeds up again.
    public LayerMask groundLayer;               // The layer on which the character will land.


    private Transform m_Transform;              // Reference to the transform component.


    public override void Init (Animator anim)
    {
        // Set the transform reference.
        m_Transform = anim.transform;
    }


    public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Don't do anything if the animator is in transition.
        if(animator.IsInTransition (layerIndex))
            return;

        // If we're near the ground...
        if (Physics.Raycast(m_Transform.position, -Vector3.up, maxDistanceFromGround, groundLayer))
        {
            // ... reset the timescales.
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
        else
        {
            // Otherwise slow the timescales based on how much the slow mo button is pressed.
            float targetTimeScale = Mathf.Lerp (1f, slowedTimeScale, userInput.SlowMo);
            Time.timeScale = targetTimeScale;
            Time.fixedDeltaTime = 0.02f * targetTimeScale;
        }
    }


    public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If the timescales haven't been reset by now, do so.
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
