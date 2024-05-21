using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Spline : MonoBehaviour {

    [SerializeField]
    private Curve.CurveType curveType = Curve.CurveType.Bezier;

    [SerializeField, Range(0f, 1f)]
    private float strength;

    [SerializeField]
    public List<Vector3> controlPoints = new List<Vector3>();

    [SerializeField, Range(0f, 10f)]
    private float raysLength;

    public void ResetControlPoints() {
        controlPoints[0] = Vector3.zero;
        for (int i = 1; i < controlPoints.Count; i++) {
            controlPoints[i] = new Vector3(5f * i, (i % 2 == 0) ? 5f : -5f, 0f);
        }
    }

    public void P(float t, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal) {
        Curve.P(t, curveType, controlPoints, out vertex, out tangent, out normal, out binormal, strength);
    }

    private void OnDrawGizmos() {
        float t = 0;
        while (t <= GetMaxPointInd()) {
            Curve.P(t, curveType, controlPoints, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal, strength);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(vertex, 0.1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(vertex, vertex + tangent * raysLength);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(vertex, vertex + normal * raysLength);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(vertex, vertex + binormal * raysLength);

            t += 0.05f;
        }

        for (int i = 0; i < controlPoints.Count; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(controlPoints[i], 0.2f);
        }
    }

    public Curve.CurveType GetCurveType() {
        return curveType;
    }

    public List<Vector3> GetControlPoints() {
        return controlPoints;
    }

    public int GetMaxPointInd() {
        return controlPoints.Count - (4 - Curve.stepInd[curveType]);
    }

}