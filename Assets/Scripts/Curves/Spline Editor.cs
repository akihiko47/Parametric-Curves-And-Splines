using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Spline))]
[CanEditMultipleObjects]
public class SplineEditor : Editor {

    SerializedProperty curveType;
    SerializedProperty normals2D;
    SerializedProperty normalsRotation;
    SerializedProperty strength;
    SerializedProperty controlPoints;
    SerializedProperty raysLength;
    SerializedProperty drawStep;

    public static event Action onSplineEdited;

    private void OnEnable() {
        curveType = serializedObject.FindProperty("curveType");
        normals2D = serializedObject.FindProperty("normals2D");
        normalsRotation = serializedObject.FindProperty("normalsRotation");
        strength = serializedObject.FindProperty("strength");
        controlPoints = serializedObject.FindProperty("controlPoints");
        raysLength = serializedObject.FindProperty("raysLength");
        drawStep = serializedObject.FindProperty("drawStep");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        Spline script = (Spline)target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(curveType);

        if (script.GetCurveType() == Curve.CurveType.Cardinal) {
            EditorGUILayout.PropertyField(strength);
        }
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(normals2D);

        if (normals2D.boolValue) {
            EditorGUILayout.PropertyField(normalsRotation);
        }
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(controlPoints);
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(raysLength);
        EditorGUILayout.PropertyField(drawStep);

        if (GUILayout.Button("Reset Control Points")) {
            script.ResetControlPoints();
        }

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Spline Changed");

            onSplineEdited?.Invoke();
        }
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