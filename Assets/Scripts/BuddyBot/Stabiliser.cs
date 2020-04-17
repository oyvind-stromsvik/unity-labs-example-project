using JetBrains.Annotations;
using UnityEngine;
using System.Collections;

public class Stabiliser : MonoBehaviour
{

    public Quaternion TargetRotation;

    public float RotationSpeed = 360f;

    public Transform RotationTarget;

    public float GetAngleToTarget()
    {
        return Quaternion.Angle(RotationTarget.rotation, TargetRotation);
    }

    public void SetTargetDirection(Vector3 direction)
    {
        TargetRotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public void LevelOff()
    {
        var flatFwd = RotationTarget.forward;
        flatFwd.y = 0; flatFwd.Normalize();
        SetTargetDirection(flatFwd);
    }

    public void Update()
    {
        RotationTarget.rotation = Quaternion.RotateTowards(RotationTarget.rotation, TargetRotation, RotationSpeed * Time.deltaTime);
    }
}
