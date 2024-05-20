using UnityEngine;

public class CurveAnimator : MonoBehaviour {
    [SerializeField]
    GameObject animatedObject;

    [SerializeField]
    WorldCurve curve;

    [SerializeField]
    float animationTime;
    float time = 0f;

    private void Update() {
        if (time < animationTime) {
            float t = time / animationTime * curve.GetMaxPointInd();
            curve.GetCurve().P(t, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal);

            animatedObject.transform.position = vertex;
            animatedObject.transform.right = binormal;
            animatedObject.transform.up = normal;
            animatedObject.transform.forward = tangent;

            time += Time.deltaTime;
        } else {
            time = 0;
        }
    }
}
