using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Spline))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {

    [SerializeField, Min(0.05f)]
    private float meshWidth = 1f;

    [SerializeField, Min(0.01f)]
    private float meshStep = 0.03f;

    [SerializeField]
    private bool animated = false;

    [SerializeField, Min(0.01f)]
    private float animationTime = 3f;

    private float animatedMeshStep;
    private bool animating = false;

    private int endDetailNum = 8;

    private Mesh mesh;

    private void Start() {
        AnimateMeshGeneration();
    }

    private void OnEnable() {
        SplineEditor.onSplineEdited += GenerateMesh;
        MeshGeneratorEditor.onMeshEdited += GenerateMesh;
    }
    private void OnDisable() {
        SplineEditor.onSplineEdited -= GenerateMesh;
        MeshGeneratorEditor.onMeshEdited -= GenerateMesh;
    }

    private void OnDrawGizmos() {
        if (mesh.vertices == null) {
            return;
        }
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.black;
        for (int i = 0; i < mesh.vertices.Length; i++) {
            Gizmos.DrawSphere(mesh.vertices[i], 0.05f);
        }
    }

    public void GenerateMesh() {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Spline Mesh";

        mesh.vertices = CreateVertices();
        mesh.triangles = CreateTriangles();
        mesh.RecalculateNormals();
    }

    private Vector3[] CreateVertices() {
        Spline spline = GetComponent<Spline>();
        int meshPointsNum = Mathf.CeilToInt(spline.GetMaxPointInd() / meshStep);

        Vector3[] vertices = new Vector3[meshPointsNum * 2 + (endDetailNum + 1) * 2];
        int vertIndex = 0;

        for (int i = 0; i < meshPointsNum; i++) {
            spline.P(
                i * (animating ? animatedMeshStep : meshStep),
                out Vector3 vertex,
                out Vector3 tangent,
                out Vector3 normal,
                out Vector3 binormal
            );

            if (i == 0 || i == meshPointsNum - 1) {
                vertIndex = CreateSemicircle(vertices, vertIndex, vertex, vertIndex == 0 ? -tangent : tangent, binormal);
            } else {
                vertIndex = CreateRegularStep(vertices, vertIndex, vertex, binormal);
            }
        }

        return vertices;
    }

    private int CreateSemicircle(Vector3[] vertices, int i, Vector3 vertex, Vector3 tangent, Vector3 binormal) {
        vertices[i] = vertex;
        for (int j = 1; j < endDetailNum + 1; j++) {
            vertices[i + j] =
                vertices[i] +
                (((binormal * Mathf.Cos(Mathf.PI / (endDetailNum - 1) * (j - 1))) + (tangent * Mathf.Sin(Mathf.PI / (endDetailNum - 1) * (j - 1)))).normalized * 
                meshWidth * 0.5f);
        }
        return i + endDetailNum + 1;
    }

    private int CreateRegularStep(Vector3[] vertices, int i, Vector3 vertex, Vector3 binormal) {
        vertices[i] = vertex + binormal * meshWidth * 0.5f;
        vertices[i + 1] = vertex - binormal * meshWidth * 0.5f;
        return i + 2;
    }

    private int[] CreateTriangles() {
        Spline spline = GetComponent<Spline>();
        int meshPointsNum = Mathf.CeilToInt(spline.GetMaxPointInd() / meshStep);

        int[] triangles = new int[2 * (meshPointsNum) * 3 + 2 * endDetailNum * 3];

        int trisIndex = 0;

        for (int i = 0; i < meshPointsNum; i++) {

            if (i == 0 || i == meshPointsNum - 1) {
                for (int j = 0; j < endDetailNum; j++) {
                    int centerVertInd = (i == 0 ? 0 : meshPointsNum * 2 + endDetailNum - 3);
                    if (j < endDetailNum) {
                        triangles[trisIndex] = centerVertInd;
                        triangles[trisIndex + 2] = centerVertInd + j + (i == 0 ? 0 : 1);
                        triangles[trisIndex + 1] = centerVertInd + j + (i == 0 ? 1 : 0);
                    }
                    trisIndex += 3;
                }
            } 

            if (i == 0) {
                triangles[trisIndex] = endDetailNum + 2;
                triangles[trisIndex + 1] = endDetailNum;
                triangles[trisIndex + 2] = 1;

                triangles[trisIndex + 3] = 1;
                triangles[trisIndex + 4] = endDetailNum + 1;
                triangles[trisIndex + 5] = endDetailNum + 2;

                trisIndex += 6;
            }

            if (i == meshPointsNum - 3) {
                triangles[trisIndex] = endDetailNum + 1 + i * 2;
                triangles[trisIndex + 1] = endDetailNum + 1 + i * 2 + 3;
                triangles[trisIndex + 2] = endDetailNum + 1 + i * 2 + 1;

                triangles[trisIndex + 3] = endDetailNum + 1 + i * 2 + 3;
                triangles[trisIndex + 4] = endDetailNum * 2 + i * 2 + 3;
                triangles[trisIndex + 5] = endDetailNum + 1 + i * 2 + 1;

                trisIndex += 6;
            }

            if (i < meshPointsNum - 3) {
                triangles[trisIndex] = endDetailNum + 1 + i * 2;
                triangles[trisIndex + 1] = endDetailNum + 1 + i * 2 + 2;
                triangles[trisIndex + 2] = endDetailNum + 1 + i * 2 + 1;

                triangles[trisIndex + 3] = endDetailNum + 1 + i * 2 + 1;
                triangles[trisIndex + 4] = endDetailNum + 1 + i * 2 + 2;
                triangles[trisIndex + 5] = endDetailNum + 1 + i * 2 + 3;

                trisIndex += 6;
            }
        }
        return triangles;
    }

    public void AnimateMeshGeneration() {
        StartCoroutine(MeshGenerationAnimation());
    }

    private IEnumerator MeshGenerationAnimation() {
        animating = true;

        float t = 0f;
        while (t <= animationTime) {
            if(!animated) {
                break;
            }

            animatedMeshStep = Mathf.Lerp(0f, meshStep, Mathf.SmoothStep(0f, 1f, t / animationTime));
            GenerateMesh();

            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        animating = false;
        GenerateMesh();
    }
}
