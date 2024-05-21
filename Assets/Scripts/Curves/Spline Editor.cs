using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spline))]
[CanEditMultipleObjects]
public class SplineEditor : Editor {
    SerializedProperty curveType;
    SerializedProperty strength;
    SerializedProperty controlPoints;
    SerializedProperty raysLength;

    void OnEnable() {
        curveType = serializedObject.FindProperty("curveType");
        strength = serializedObject.FindProperty("strength");
        controlPoints = serializedObject.FindProperty("controlPoints");
        raysLength = serializedObject.FindProperty("raysLength");
    }

    public override void OnInspectorGUI() {
        Spline script = (Spline)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(curveType);

        if (script.GetCurveType() == Curve.CurveType.Cardinal) {
            EditorGUILayout.PropertyField(strength);
        }

        EditorGUILayout.PropertyField(controlPoints);
        EditorGUILayout.PropertyField(raysLength);

        if (GUILayout.Button("Reset Control Points")) {
            script.ResetControlPoints();
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI() {
        Spline script = (Spline)target;

        
        for (int i = 0; i < script.controlPoints.Count; i++) {
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(script.controlPoints[i], Quaternion.identity);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Control Point Updated");
                script.controlPoints[i] = newPos;
            }
        }
        
    }
}