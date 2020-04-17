using UnityEngine;

public class LandingAudioSMB : CustomSMB
{
    // An enum to define when exactly the clips is played.
    public enum Timing
    {
        OnStateEnter, AfterEntryTransition, AtExitTransition, OnStateExit
    }


    public Timing whenToPlayClip;       // When the clip is played.
    public LayerMask layerMask;         // The layer which defines the surface materials.
    public AudioSurface[] surfaces =    // A collection of the different potential surfaces and their audioclips.
    {
        new AudioSurface ("Carpet"), new AudioSurface ("Glass"),
        new AudioSurface ("MetalHeavy"), new AudioSurface ("MetalLight"),
        new AudioSurface ("Rubber")
    };


    private Transform m_Transform;          // Reference to the transform component.
    private bool entryTransitionFinished;   // Whether the transition to this state has finished or not.
    private bool exitTransitionStarted;     // Whether the transition from this state has started or not.

    private const float k_RayLength = 2f;   // The length of the raycast for detecting the surface.
    

    public override void Init (Animator anim)
    {
        // Setting up the transform component and the audio sources of the surfaces.
        m_Transform = anim.transform;

        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].SetSource (anim);
        }
    }


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset whether or not the entry and exit transitions have finished and started.
        entryTransitionFinished = false;
        exitTransitionStarted = false;

        // If the clip is to be played OnStateEnter, play it.
	    if(whenToPlayClip == Timing.OnStateEnter)
            PlayClip ();
	}

	
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        // If the entry transition hasn't yet been recorded as finished but the animator is not in transition...
	    if (!entryTransitionFinished && !animator.IsInTransition (layerIndex))
	    {
            // ... the entry transition has finished.
	        entryTransitionFinished = true;
            
            // If the clip is to be played after the entry transition, play it.
            if(whenToPlayClip == Timing.AfterEntryTransition)
                PlayClip ();
	    }

        // If the exit transition hasn't yet been recorded as started but the animator is in transition and the entry transition is finished...
	    if (!exitTransitionStarted && animator.IsInTransition (layerIndex) && entryTransitionFinished)
	    {
            // ... the exit transition has started.
	        exitTransitionStarted = true;

            // If the clip is to be played at the exit transition, play it.
            if(whenToPlayClip == Timing.AtExitTransition)
                PlayClip ();
	    }
	}

	
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        // If the clip is to be played OnStateExit, play it.
	    if(whenToPlayClip == Timing.OnStateExit)
            PlayClip ();
	}


    void PlayClip ()
    {
        Ray ray = new Ray(m_Transform.position + Vector3.up, -Vector3.up);
        RaycastHit hit;

        // Cast a ray straight down from the character.
        if (Physics.Raycast (ray, out hit, k_RayLength, layerMask))
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                // If the hit collider's tag matches the surface's tag, play a random clip and break the loop.
                if (surfaces[i].tag == hit.collider.tag)
                {
                    surfaces[i].PlayRandomClip ();
                    break;
                }
            }
        }
    }
}
