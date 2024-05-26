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
    private int meshPointsNum;

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
        if (mesh == null || mesh.vertices == null) {
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

        CreateVertices();
        CreateTriangles();
        mesh.RecalculateNormals();
    }

    private void CreateVertices() {
        Spline spline = GetComponent<Spline>();
        meshPointsNum = Mathf.CeilToInt(spline.GetMaxPointInd() / meshStep);

        Vector3[] vertices = new Vector3[meshPointsNum * 2 + (endDetailNum + 1) * 2];
        Vector2[] uv = new Vector2[vertices.Length];
        
        int vertIndex = 0;

        for (int i = 0; i < meshPointsNum; i++) {
            spline.P(
                i * (animating ? animatedMeshStep : meshStep),
                out Vector3 vertex,
                out Vector3 tangent,
                out Vector3 normal,
                out Vector3 binormal
            );

            if (i == 0) {
                vertIndex = CreateSemicircle(vertices, uv, vertIndex, vertex, i == 0 ? -tangent : tangent, binormal);
            }

            vertIndex = CreateRegularStep(vertices, uv, vertIndex, vertex, binormal);

            if (i == meshPointsNum - 1) {
                vertIndex = CreateSemicircle(vertices, uv, vertIndex, vertex, i == 0 ? -tangent : tangent, binormal);
            }

        }

        mesh.vertices = vertices;
        mesh.uv = uv;
    }

    private int CreateSemicircle(Vector3[] vertices, Vector2[] uv, int i, Vector3 vertex, Vector3 tangent, Vector3 binormal) {
        float completed = i / (float)(meshPointsNum - 1);
        vertices[i] = vertex;
        uv[i] = new Vector2(0.5f, completed);
        for (int j = 1; j < endDetailNum + 1; j++) {
            vertices[i + j] =
                vertices[i] +
                (((binormal * Mathf.Cos(Mathf.PI / (endDetailNum - 1) * (j - 1))) + (tangent * Mathf.Sin(Mathf.PI / (endDetailNum - 1) * (j - 1)))).normalized * 
                meshWidth * 0.5f);
            uv[i + j] = new Vector2(1f, completed);
        }
        return i + endDetailNum + 1;
    }

    private int CreateRegularStep(Vector3[] vertices, Vector2[] uv, int i, Vector3 vertex, Vector3 binormal) {
        vertices[i] = vertex + binormal * meshWidth * 0.5f;
        vertices[i + 1] = vertex - binormal * meshWidth * 0.5f;

        float completed = i / (float)(meshPointsNum - 1);
        uv[i] = new Vector2(0, completed);
        uv[i + 1] = new Vector2(1, completed);

        return i + 2;
    }

    private void CreateTriangles() {
        Spline spline = GetComponent<Spline>();
        int meshPointsNum = Mathf.CeilToInt(spline.GetMaxPointInd() / meshStep);

        int[] triangles = new int[2 * (meshPointsNum + 1) * 3 + 2 * (endDetailNum - 1) * 3];
        int trisIndex = 0;

        for (int i = 0; i < meshPointsNum; i++) {

            if (i == 0 || i == meshPointsNum - 1) {
                trisIndex = CreateSemicircleTris(triangles, trisIndex, i);
            } 

            if (i < meshPointsNum - 1) {
                trisIndex = CreateRegularStepQuad(triangles, trisIndex, i);
            }

        }
        mesh.triangles = triangles;
    }

    private int CreateRegularStepQuad(int[] triangles, int trisIndex, int i) {
        triangles[trisIndex] = endDetailNum + 1 + i * 2;
        triangles[trisIndex + 1] = endDetailNum + 1 + i * 2 + 2;
        triangles[trisIndex + 2] = endDetailNum + 1 + i * 2 + 1;

        triangles[trisIndex + 3] = endDetailNum + 1 + i * 2 + 1;
        triangles[trisIndex + 4] = endDetailNum + 1 + i * 2 + 2;
        triangles[trisIndex + 5] = endDetailNum + 1 + i * 2 + 3;

        return trisIndex + 6;
    }

    private int CreateSemicircleTris(int[] triangles, int trisIndex, int i) {
        for (int j = 0; j < endDetailNum; j++) {
            int centerVertInd = (i == 0 ? 0 : meshPointsNum * 2 + endDetailNum + 1);
            if (j < endDetailNum) {
                triangles[trisIndex] = centerVertInd;
                triangles[trisIndex + 2] = centerVertInd + j + (i == 0 ? 0 : 1);
                triangles[trisIndex + 1] = centerVertInd + j + (i == 0 ? 1 : 0);
            }
            trisIndex += 3;
        }
        return trisIndex;
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
