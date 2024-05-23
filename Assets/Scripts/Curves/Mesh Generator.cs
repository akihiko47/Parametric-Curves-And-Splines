using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Spline))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {

    [SerializeField, Range(0.05f, 10f)]
    private float meshWidth = 1f;

    [SerializeField, Range(0.01f, 1f)]
    private float meshStep = 0.03f;

    private Mesh mesh;

    private void OnEnable() {
        SplineEditor.onSplineEdited += GenerateMesh;
    }
    private void OnDisable() {
        SplineEditor.onSplineEdited -= GenerateMesh;
    }

    private void Update() {
        GenerateMesh();
    }

    public void GenerateMesh() {
        Spline spline = GetComponent<Spline>();

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Mesh";

        int meshPointsNum = Mathf.FloorToInt(spline.GetMaxPointInd() / meshStep);

        Vector3[] vertices = new Vector3[meshPointsNum * 2];
        Vector3[] normals = new Vector3[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[2 * (meshPointsNum - 1) * 3];

        int vertIndex = 0;
        int trisIndex = 0;

        for(int i = 0; i < meshPointsNum; i++) {
            spline.P(i * meshStep, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal);

            vertices[vertIndex] = vertex + binormal * meshWidth * 0.5f;
            vertices[vertIndex + 1] = vertex - binormal * meshWidth * 0.5f;

            float completed = i / (float)(meshPointsNum - 1);
            uv[vertIndex] = new Vector2(0, completed);
            uv[vertIndex + 1] = new Vector2(1, completed);

            normals[vertIndex] = normal;
            normals[vertIndex + 1] = normal;

            tangents[vertIndex] = new Vector4(tangent.x, tangent.y, tangent.z, -1);
            tangents[vertIndex + 1] = new Vector4(tangent.x, tangent.y, tangent.z, -1);

            if (i < meshPointsNum - 1) {
                triangles[trisIndex] = vertIndex;
                triangles[trisIndex + 1] = vertIndex + 2;
                triangles[trisIndex + 2] = vertIndex + 1;

                triangles[trisIndex + 3] = vertIndex + 1;
                triangles[trisIndex + 4] = vertIndex + 2;
                triangles[trisIndex + 5] = vertIndex + 3;
            }

            vertIndex += 2;
            trisIndex += 6;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
