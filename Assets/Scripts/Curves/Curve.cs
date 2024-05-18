using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Curve{

    public enum CurveType {
        Bezier,
        BSpline,
    }

    private CurveType _curveType;

    private static Matrix4x4 bezierBasis = new Matrix4x4(
        new Vector4(1f, 0f, 0f, 0f), 
        new Vector4(-3f, 3f, 0f, 0f), 
        new Vector4(3f, -6f, 3f, 0f), 
        new Vector4(-1f, 3f, -3f, 1f)
    );

    private static Matrix4x4 bsplineBasis = new Matrix4x4(
        new Vector4(1f, 4f, 1f, 0f),
        new Vector4(-3f, 0f, 3f, 0f),
        new Vector4(3f, -6f, 3f, 0f),
        new Vector4(-1f, 3f, -3f, 1f)
    );


    private Dictionary<CurveType, Matrix4x4> basises = new Dictionary<CurveType, Matrix4x4>() {
        {CurveType.Bezier, bezierBasis},
        {CurveType.BSpline, bsplineBasis}
    };

    private List<Vector3> _controlPoints;

    public Curve(List<Vector3> controlPoints) {
        if (controlPoints.Count < 4) {
            Debug.LogError("There must be atleast 4 points!");
            throw new Exception();
        }
        if ((controlPoints.Count - 1) % 3 != 0) {
            Debug.LogError("There must be 3*n+1 control points in bezier curve!");
            throw new Exception();
        }

            _curveType = CurveType.Bezier;
        _controlPoints = controlPoints;
    }
    public Curve(CurveType type, List<Vector3> controlPoints) {
        if (controlPoints.Count < 4) {
            Debug.LogError("There must be atleast 4 points!");
            throw new Exception();
        }
        if (_curveType == CurveType.Bezier && (controlPoints.Count - 1) % 3 != 0) {
            Debug.LogError("There must be 3*n+1 control points in bezier curve!");
            throw new Exception();
        }

        _curveType = type;
        _controlPoints = controlPoints;
    }

    public void P(float t, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal) {
        if (t < 0 || t > _controlPoints.Count) {
            Debug.LogError("t value is out of bounds!");
            throw new Exception();
        }

        int segment = 0;
        float tSegment = 0f;
        if (_curveType == CurveType.Bezier) {
            t = t / 3;
            segment = Mathf.FloorToInt(t) * 3;
            tSegment = t % 1;

            if (segment == _controlPoints.Count - 1) {
                segment -= 3;
            }

        } else {
            segment = Mathf.FloorToInt(t);
            tSegment = (t % 3f);
        }

        Matrix4x4 G = new Matrix4x4(
            new Vector4(_controlPoints[segment].x, _controlPoints[segment].y, _controlPoints[segment].z, 0f),
            new Vector4(_controlPoints[segment + 1].x, _controlPoints[segment + 1].y, _controlPoints[segment + 1].z, 0f),
            new Vector4(_controlPoints[segment + 2].x, _controlPoints[segment + 2].y, _controlPoints[segment + 2].z, 0f),
            new Vector4(_controlPoints[segment + 3].x, _controlPoints[segment + 3].y, _controlPoints[segment + 3].z, 0f)
        );

        Matrix4x4 B = basises[_curveType];
        Vector4 T = new Vector4(1, tSegment, tSegment * tSegment, tSegment * tSegment * tSegment);
        Vector4 T_1 = new Vector4(0, 1, 2 * tSegment, 3 * tSegment * tSegment);
        Vector4 T_2 = new Vector4(0, 0, 2, 6 * tSegment);

        Vector4 V = G * B * T;
        Vector4 Vel = G * B * T_1;
        Vector4 Acc = G * B * T_2;

        Vector3 a = new Vector3(Vel[0], Vel[1], Vel[2]);
        Vector3 b = (a + new Vector3(Acc[0], Acc[1], Acc[2])).normalized;
        Vector3 r = Vector3.Cross(b, a).normalized;
        normal = Vector3.Cross(r, a).normalized;

        vertex = new Vector3(V[0], V[1], V[2]);
        tangent = Vel.normalized;
        binormal = Vector3.Cross(tangent, normal);
    }
}
