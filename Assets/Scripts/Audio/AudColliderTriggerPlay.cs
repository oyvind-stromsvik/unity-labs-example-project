using UnityEngine;

public class AudColliderTriggerPlay : MonoBehaviour 
{
	public AudioSource PlayThisSource;

	void OnTriggerEnter(Collider other)
	{
		PlayThisSource.Play ();
	}

	void OnTriggerExit (Collider other)
	{
		PlayThisSource.Stop ();
	}

}
