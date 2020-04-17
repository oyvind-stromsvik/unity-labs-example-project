using UnityEngine;
using System.Collections;

public class PlaySourceOnPlayerEntry : MonoBehaviour {

	public AudioSource Source;
	public bool OnlyOnce;
	
	private bool _hasPlayedOnce;
	
	void OnTriggerEnter(Collider other)
	{
		if(OnlyOnce && _hasPlayedOnce) return;
		if(!other.CompareTag ("Player")) return;
		
		Source.Play ();
		_hasPlayedOnce = true;
	}
}
