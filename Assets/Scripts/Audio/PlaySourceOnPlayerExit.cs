using UnityEngine;
using System.Collections;

public class PlaySourceOnPlayerExit : MonoBehaviour {

	public AudioSource Source;
	
	void OnTriggerExit(Collider other)
	{
		if(!other.CompareTag ("Player")) return;
		
		Source.Play ();
	}
}
