using System;
using UnityEngine;
using System.Collections;

public class ObjectivesController : MonoBehaviour
{

    public PointOfInterest[] PointsOfInterest;

    public PointOfInterestMarker UIMarker;
    public float NextObjectiveTimeout = 5f;

    private PointOfInterest _currentPointOfInterest;
    private int _nextObjectiveIndex = 0;
    
    public CurrentObjectiveText UIObjectiveText;

    public IEnumerator Start()
    {
		yield return null;
		
		UIMarker = FindObjectOfType<PointOfInterestMarker>();
		UIObjectiveText = FindObjectOfType<CurrentObjectiveText>();
    
        foreach (var poi in PointsOfInterest)
        {
            poi.OnPlayerArrived.AddListener(OnPlayerArrived);
            poi.OnPlayerDeparted.AddListener(OnPlayerDeparted);
        }

        _currentPointOfInterest = null;
    }
    
    public void BeginFirstObjective()
    {
        SetCurrentPOI(PointsOfInterest[0]);
        _nextObjectiveIndex = 1;
    }

    private void AdvanceObjective()
    {
        SetCurrentPOI(PointsOfInterest[_nextObjectiveIndex]);
        _nextObjectiveIndex = (_nextObjectiveIndex + 1)%PointsOfInterest.Length;
    }

    private Coroutine _timeoutRoutine;

    private void OnPlayerDeparted(PointOfInterest arg0)
    {
        if (_currentPointOfInterest == null)
        {
            if (_timeoutRoutine != null)
            {
                StopCoroutine(_timeoutRoutine);
                _timeoutRoutine = null;
            }

            AdvanceObjective();
        }
    }

    private void OnPlayerArrived(PointOfInterest arg0)
    {
        if (arg0 == _currentPointOfInterest)
        {
            SetCurrentPOI(null);
            _timeoutRoutine = StartCoroutine(TimeoutAndSelectNextPOI(arg0));
        }
    }

    private IEnumerator TimeoutAndSelectNextPOI(PointOfInterest prevPOI)
    {
        yield return new WaitForSeconds(NextObjectiveTimeout);
        if(_currentPointOfInterest == null) AdvanceObjective();
    }

    public void ClearCurrentPOI()
    {
        SetCurrentPOI(null);
    }

    private void SetCurrentPOI(PointOfInterest poi)
    {
        if (poi)
        {
            UIMarker.Activate();
            UIMarker.TargetMarker = poi.transform;
            
       		UIObjectiveText.Display (string.Format ("Objective {0}/{1}: {2}", Array.IndexOf(PointsOfInterest, poi) + 1, PointsOfInterest.Length, poi.DisplayName));
        }
        else
        {
            UIMarker.Deactivate();
            UIObjectiveText.Dismiss();
        }

        _currentPointOfInterest = poi;
        
    }
}
