using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Spline))]
[CanEditMultipleObjects]
public class SplineEditor : Editor {

    public static event Action onSplineEdited;

    public override void OnInspectorGUI() {
        Spline script = (Spline)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("curveType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("normals2D"));

        if (serializedObject.FindProperty("normals2D").boolValue) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("normalsRotation"));
        }

        if (script.GetCurveType() == Curve.CurveType.Cardinal) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("strength"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("controlPoints"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("raysLength"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("drawStep"));

        if (GUILayout.Button("Reset Control Points")) {
            script.ResetControlPoints();
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI() {
        Spline script = (Spline)target;

        Handles.matrix = script.transform.localToWorldMatrix;
        for (int i = 0; i < script.GetControlPoints().Count; i++) {
            EditorGUI.BeginChangeCheck();

            Vector3 newPos = Handles.PositionHandle(script.GetControlPoints()[i], Quaternion.identity);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Control Point Updated");
                script.GetControlPoints()[i] = newPos;

                onSplineEdited?.Invoke();
            }
        }
        
    }
}