using UnityEditor;
using UnityEngine;
using System.Collections;

public class ScanTargetGizmo {

    [DrawGizmo(GizmoType.InSelectionHierarchy)]
    public static void DrawScanTargetGizmo(ScanTarget scanTarget, GizmoType gizmoType)
    {
        if (!scanTarget.ScanFromPosition) return;

        var fromPos = scanTarget.GetScanPosition();
        var toPos = scanTarget.GetScanSurfacePoint(fromPos, 1 << 0 | 1 << 8);

        var scannerOrientation = Quaternion.LookRotation(toPos - fromPos, Vector3.up);
        var right = scannerOrientation.GetRight();
        var up = scannerOrientation.GetUp();

        float extensionDist = Vector3.Distance(fromPos, toPos);
        float width = Mathf.Tan(Mathf.Deg2Rad * scanTarget.GetScanHorizontalMax() / 2f) * extensionDist;
        float height = Mathf.Tan(Mathf.Deg2Rad * scanTarget.GetScanVerticalMax() / 2f) * extensionDist;

        Gizmos.DrawLine(fromPos, toPos - width*right + height*up);
        Gizmos.DrawLine(fromPos, toPos - width * right - height * up);
        Gizmos.DrawLine(fromPos, toPos + width * right + height * up);
        Gizmos.DrawLine(fromPos, toPos + width * right - height * up);
    }
}
