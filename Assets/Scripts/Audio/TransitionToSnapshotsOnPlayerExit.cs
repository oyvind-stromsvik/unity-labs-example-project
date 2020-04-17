using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class TransitionToSnapshotsOnPlayerExit : MonoBehaviour {

	public List<AudioMixerSnapshot> Snapshots;
	public float OverTime;
	
	void OnTriggerExit(Collider other)
	{
		if(!other.CompareTag ("Player")) return;
		
		foreach(var snapshot in Snapshots)
			snapshot.TransitionTo(OverTime);
	}
}
