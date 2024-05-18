using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldCurve : MonoBehaviour {

    [SerializeField]
    private Curve.CurveType curveType = Curve.CurveType.Bezier;

    [SerializeField]
    public List<Vector3> controlPoints = new List<Vector3>();

    [SerializeField, Range(0f, 10f)]
    private float raysLength;

    private Curve curve;

    private void OnEnable() {
        curve = new Curve(curveType, controlPoints);
    }

    public void ResetControlPoints() {
        controlPoints[0] = Vector3.zero;
        for (int i = 1; i < controlPoints.Count; i++) {
            controlPoints[i] = new Vector3(5f * i, (i % 2 == 0) ? 5f : -5f, 0f);
        }
    }

    private void OnDrawGizmos() {
        if (curve == null) {
            return;
        }

        float t = 0;
        while (t <= controlPoints.Count) {
            curve.P(t, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal);

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
}