using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Spline : MonoBehaviour {

    [SerializeField]
    private Curve.CurveType curveType = Curve.CurveType.Bezier;

    [SerializeField]
    private bool normals2D = false;

    [SerializeField, Range(0, 360)]
    private float normalsRotation = 0f;

    [SerializeField, Range(0f, 1f)]
    private float strength;

    [SerializeField]
    private List<Vector3> controlPoints = new List<Vector3>();

    [SerializeField, Range(0f, 10f)]
    private float raysLength = 0.3f;

    [SerializeField, Range(0.05f, 1f)]
    private float drawStep = 0.05f;

    public void ResetControlPoints() {
        controlPoints[0] = Vector3.zero;
        for (int i = 1; i < controlPoints.Count; i++) {
            controlPoints[i] = new Vector3(5f * i, 0f, (i % 2 == 0) ? 5f : -5f);
        }
    }

    public void P(float t, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal, bool inWorld = false) {
        Curve.P(t, curveType, controlPoints, out vertex, out tangent, out normal, out binormal, strength);

        if (normals2D) {
            normal = new Vector3(-tangent.y, tangent.x, 0f).normalized;  // construct normal
            normal = Quaternion.AngleAxis(normalsRotation, tangent) * normal;  // rotate normal based on normalsRotation parameter
            binormal = Vector3.Cross(tangent, normal);
        }

        if (inWorld) {
            vertex = transform.TransformPoint(vertex);
            tangent = transform.TransformDirection(tangent);
            normal = transform.TransformDirection(normal);
            binormal = transform.TransformDirection(binormal);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix;

        float t = 0;
        while (t < GetMaxPointInd()) {
            P(t, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal);

            Gizmos.color = new Color(233f/255f, 216f/255f, 166f/255f);
            Gizmos.DrawSphere(vertex, 0.1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(vertex, vertex + tangent * raysLength);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(vertex, vertex + normal * raysLength);

            if (!normals2D) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(vertex, vertex + binormal * raysLength);
            }

            t += drawStep;
        }

        for (int i = 0; i < controlPoints.Count; i++) {
            Gizmos.color = new Color(174/255f, 32 / 255f, 18 / 255f);
            Gizmos.DrawSphere(controlPoints[i], 0.2f);
        }
    }

    public Curve.CurveType GetCurveType() {
        return curveType;
    }

    public List<Vector3> GetControlPoints() {
        return controlPoints;
    }

    public List<Vector3> GetWorldControlPoints() {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < controlPoints.Count; i++) {
            points.Add(transform.TransformPoint(controlPoints[i]));
        }
        return points;
    }

    public int GetMaxPointInd() {
        return controlPoints.Count - (4 - Curve.stepInd[curveType]);
    }

}