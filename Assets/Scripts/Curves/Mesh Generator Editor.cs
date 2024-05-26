using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(MeshGenerator))]
[CanEditMultipleObjects]
public class MeshGeneratorEditor : Editor {

    SerializedProperty meshWidth;
    SerializedProperty meshStep;
    SerializedProperty roundness;
    SerializedProperty animated;
    SerializedProperty animationTime;

    public static event Action onMeshEdited;

    private void OnEnable() {
        meshWidth = serializedObject.FindProperty("meshWidth");
        meshStep = serializedObject.FindProperty("meshStep");
        roundness = serializedObject.FindProperty("endDetailNum");
        animated = serializedObject.FindProperty("animated");
        animationTime = serializedObject.FindProperty("animationTime");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        MeshGenerator script = (MeshGenerator)target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(meshWidth);
        EditorGUILayout.PropertyField(meshStep);
        EditorGUILayout.PropertyField(roundness, new GUIContent("Ends roundness"));
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(animated);

        if (animated.boolValue) {

            EditorGUILayout.PropertyField(animationTime);

            if (GUILayout.Button("Play Animation")) {
                script.AnimateMeshGeneration();
            }
        }

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Mesh Changed");
            onMeshEdited?.Invoke();
        }
    }
}