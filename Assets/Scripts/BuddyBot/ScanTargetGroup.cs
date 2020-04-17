using System.Linq;
using UnityEngine;

public class ScanTargetGroup : MonoBehaviour
{
    private ScanTarget[] _targets;

    public float TimeBeforeRescan = 30f;

    public void Awake()
    {
        _targets = GetComponentsInChildren<ScanTarget>();
    }

    public void ResetAllScanTimes()
    {
        foreach (var target in _targets)
            target.LastScannedTime = 0;
    }

    public ScanTarget SelectNewTarget()
    {
        var eligibleTargets =
            _targets.Where(t => t.gameObject.activeInHierarchy && (t.LastScannedTime <= 0 || t.LastScannedTime < Time.time - TimeBeforeRescan)).ToList();
        if (eligibleTargets.Count == 0) return null;

        return eligibleTargets[Random.Range(0, eligibleTargets.Count)];
    }
}
