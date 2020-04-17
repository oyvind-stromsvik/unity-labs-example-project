using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(DoorwayTrigger))]
[CanEditMultipleObjects]
public class DoorwayTriggerEditor : Editor {

    [DrawGizmo(GizmoType.InSelectionHierarchy)]
    public static void DrawGizmo(DoorwayTrigger doorway, GizmoType gizmo)
    {
        // Don't draw the gizmo while moving things, it gets in the way
        if (Tools.current == Tool.Move) return;

        Handles.color = Color.green;
        Handles.ArrowCap(-1, doorway.transform.position, doorway.transform.rotation, 2f);
        Handles.color = Color.red;
        Handles.ArrowCap(-1, doorway.transform.position, doorway.transform.rotation, -2f);
    }
}
