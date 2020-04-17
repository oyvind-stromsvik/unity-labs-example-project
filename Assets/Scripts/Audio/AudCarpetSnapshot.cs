using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudCarpetSnapshot : MonoBehaviour {

	public AudioMixerSnapshot carpet;
	public AudioMixerSnapshot floor;
	public float TransitionTime;

	void OnTriggerEnter (Collider other)
	{
		if(carpet)
			carpet.TransitionTo (TransitionTime);
	}

	void OnTriggerExit (Collider other)
	{
		if(floor)
			floor.TransitionTo (TransitionTime);
	}

}
