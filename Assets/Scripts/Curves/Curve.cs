using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class Curve{

    public enum CurveType {
        Bezier,
        BSpline,
        Cardinal,
    }

    private static Matrix4x4 bezierBasis = new Matrix4x4(
        new Vector4(1f, 0f, 0f, 0f), 
        new Vector4(-3f, 3f, 0f, 0f), 
        new Vector4(3f, -6f, 3f, 0f), 
        new Vector4(-1f, 3f, -3f, 1f)
    );

    private static Matrix4x4 bsplineBasis = new Matrix4x4(
        new Vector4(1 / 6f, 4 / 6f, 1f / 6, 0f / 6),
        new Vector4(-3 / 6f, 0f / 6, 3f / 6, 0f / 6),
        new Vector4(3f / 6, -6f / 6, 3f / 6, 0f / 6),
        new Vector4(-1f / 6, 3f / 6, -3f / 6, 1f / 6)
    );

    public static Dictionary<CurveType, Matrix4x4> basises = new Dictionary<CurveType, Matrix4x4>() {
        {CurveType.Bezier, bezierBasis},
        {CurveType.BSpline, bsplineBasis}
    };

    public static Dictionary<CurveType, int> stepInd = new Dictionary<CurveType, int>() {
        {CurveType.Bezier, 3},
        {CurveType.BSpline, 1},
        {CurveType.Cardinal, 1}
    };

    public static Matrix4x4 GetCardinalBasis(float s) {
        return new Matrix4x4(
            new Vector4(0f, -s, 2f*s, -s),
            new Vector4(1f, 0f, s-3f, 2f-s),
            new Vector4(0f, s, 3f-2f*s, s-2f),
            new Vector4(0f, 0f, -s, s)
        ).transpose;
    }

    public static void P(float t, CurveType type, List<Vector3> controlPoints, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal, float s=0.5f) {
        if (t < 0 || t > controlPoints.Count - (4 - stepInd[type])) {
            Debug.LogError("t value is out of bounds!");
            throw new Exception();
        }

        if (controlPoints.Count < 4) {
            Debug.LogError("There must be atleast 4 control points!");
            throw new Exception();
        }

        if (((controlPoints.Count - 1) % 3) != 0 && type == CurveType.Bezier) {
            Debug.LogError("There must be 3n+1 control points in Bezier curve!");
            throw new Exception();
        }

        t = t / stepInd[type];
        int segment = Mathf.FloorToInt(t) * stepInd[type];
        float tSegment = t % 1;

        Matrix4x4 G = new Matrix4x4(
            new Vector4(controlPoints[segment].x, controlPoints[segment].y, controlPoints[segment].z, 0f),
            new Vector4(controlPoints[segment + 1].x, controlPoints[segment + 1].y, controlPoints[segment + 1].z, 0f),
            new Vector4(controlPoints[segment + 2].x, controlPoints[segment + 2].y, controlPoints[segment + 2].z, 0f),
            new Vector4(controlPoints[segment + 3].x, controlPoints[segment + 3].y, controlPoints[segment + 3].z, 0f)
        );

        // Get basis matrix (special case for Cardinal curve)
        Matrix4x4 B;
        if (type == CurveType.Cardinal) {
            B = GetCardinalBasis(s);
        } else {
            B = basises[type];
        }

        // Monomial basis and derivatives
        Vector4 T = new Vector4(1, tSegment, tSegment * tSegment, tSegment * tSegment * tSegment);
        Vector4 T_1 = new Vector4(0, 1, 2 * tSegment, 3 * tSegment * tSegment);
        Vector4 T_2 = new Vector4(0, 0, 2, 6 * tSegment);

        // Position, Velocity, Acceleration
        Vector4 V = G * B * T;
        Vector4 Vel = G * B * T_1;
        Vector4 Acc = G * B * T_2;

        // Calculating Normal
        Vector3 a = new Vector3(Vel[0], Vel[1], Vel[2]);
        Vector3 b = (a + new Vector3(Acc[0], Acc[1], Acc[2])).normalized;
        Vector3 r = Vector3.Cross(b, a).normalized;
        normal = Vector3.Cross(r, a).normalized;

        vertex = new Vector3(V[0], V[1], V[2]);
        tangent = Vel.normalized;
        binormal = Vector3.Cross(tangent, normal);
    }
}
