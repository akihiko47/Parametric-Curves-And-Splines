using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Spline))]
[CanEditMultipleObjects]
public class SplineEditor : Editor {

    public static event Action onSplineEdited;

    SerializedProperty curveType;
    SerializedProperty normals2D;
    SerializedProperty normalsRotation;
    SerializedProperty strength;
    SerializedProperty controlPoints;
    SerializedProperty raysLength;
    SerializedProperty drawStep;

    void OnEnable() {
        curveType = serializedObject.FindProperty("curveType");
        normals2D = serializedObject.FindProperty("normals2D");
        normalsRotation = serializedObject.FindProperty("normalsRotation");
        strength = serializedObject.FindProperty("strength");
        controlPoints = serializedObject.FindProperty("controlPoints");
        raysLength = serializedObject.FindProperty("raysLength");
        drawStep = serializedObject.FindProperty("drawStep");
    }

    public override void OnInspectorGUI() {
        Spline script = (Spline)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(curveType);
        EditorGUILayout.PropertyField(normals2D);

        if (normals2D.boolValue) {
            EditorGUILayout.PropertyField(normalsRotation);
        }

        if (script.GetCurveType() == Curve.CurveType.Cardinal) {
            EditorGUILayout.PropertyField(strength);
        }

        EditorGUILayout.PropertyField(controlPoints);
        EditorGUILayout.PropertyField(raysLength);
        EditorGUILayout.PropertyField(drawStep);

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