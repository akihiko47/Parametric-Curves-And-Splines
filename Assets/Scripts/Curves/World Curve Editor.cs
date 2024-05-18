using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldCurve))]
[CanEditMultipleObjects]
public class LookAtPointEditor : Editor {
    SerializedProperty curveType;
    SerializedProperty controlPoints;
    SerializedProperty raysLength;

    void OnEnable() {
        curveType = serializedObject.FindProperty("curveType");
        controlPoints = serializedObject.FindProperty("controlPoints");
        raysLength = serializedObject.FindProperty("raysLength");
    }

    public override void OnInspectorGUI() {
        WorldCurve script = (WorldCurve)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(curveType);
        EditorGUILayout.PropertyField(controlPoints);
        EditorGUILayout.PropertyField(raysLength);

        if (GUILayout.Button("Reset Control Points")) {
            script.ResetControlPoints();
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI() {
        WorldCurve script = (WorldCurve)target;

        
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