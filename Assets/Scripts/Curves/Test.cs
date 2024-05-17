using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    

    [SerializeField]
    private List<Vector3> controlPoints;

    private Curve curve;

    private void Start() {
        curve = new Curve(controlPoints);
    }

    private void OnDrawGizmos() {
        
        float t = 0;
        for (int i = 0; i < 20; i++) {
            curve.P(t, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(vertex, 0.1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(vertex, tangent);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(vertex, normal);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(vertex, binormal);

            t += 0.05f;
        }
    }
}
