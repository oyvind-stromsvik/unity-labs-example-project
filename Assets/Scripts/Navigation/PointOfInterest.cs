using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PointOfInterest : MonoBehaviour
{
	public string DisplayName;

    [Serializable]
    public class POIEvent : UnityEvent<PointOfInterest> { }

    public POIEvent OnPlayerArrived;
    public POIEvent OnPlayerDeparted;

    public void OnTriggerEnter(Collider other)
    {
        if (!enabled || !other.CompareTag("Player")) return;
        OnPlayerArrived.Invoke(this);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!enabled || !other.CompareTag("Player")) return;
        OnPlayerDeparted.Invoke(this);
    }
}
