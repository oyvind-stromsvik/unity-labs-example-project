using UnityEngine;
using System.Collections;

public class ScanTarget : MonoBehaviour
{
    public Transform ScanFromPosition;
    public Transform OverrideTargetPosition;
    public float MinScanTime;
    public float MaxScanTime;
    public float ScanHorizontalAngle = 30f;
    public float ScanVerticalAngle = 60f;
    public bool ShouldSweepHorizontally = false;

    public float LastScannedTime;

    public void OnValidate()
    {
        if (!ScanFromPosition)
            Debug.LogErrorFormat(this, "ScanTarget {0} has no ScanFromPosition set.", gameObject.name);
    }

    public Vector3 GetScanPosition()
    {
        return ScanFromPosition.position;
    }

    public Vector3 GetScanDirection()
    {
        if (OverrideTargetPosition)
            return (OverrideTargetPosition.position - GetScanPosition()).normalized;

        return (transform.position - GetScanPosition()).normalized;
    }

    public float GetScanTime()
    {
        return Random.Range(MinScanTime, MaxScanTime);
    }

    public float GetScanHorizontalMax()
    {
        return ScanHorizontalAngle;
    }

    public float GetScanVerticalMax()
    {
        return ScanVerticalAngle;
    }

    public Vector3 GetScanSurfacePoint(Vector3 fromPosition, LayerMask blockingLayers)
    {
        if (OverrideTargetPosition)
            return OverrideTargetPosition.position;

        RaycastHit hitPoint;
        if (!Physics.Raycast(fromPosition, transform.position - fromPosition, out hitPoint, 20f, blockingLayers))
            return GetScanPosition();
        return hitPoint.point;
    }

    public bool GetShouldSweepHorizontally()
    {
        return ShouldSweepHorizontally;
    }
}
