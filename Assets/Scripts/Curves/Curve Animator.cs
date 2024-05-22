using UnityEngine;

public class CurveAnimator : MonoBehaviour {
    [SerializeField]
    GameObject animatedObject;

    [SerializeField]
    Spline spline;

    [SerializeField]
    float animationTime;

    float time = 0f;

    private void Update() {
        if (time < animationTime) {
            float t = time / animationTime * spline.GetMaxPointInd();
            spline.P(t, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal, true);

            animatedObject.transform.position = vertex;
            animatedObject.transform.forward = tangent;
            animatedObject.transform.rotation = Quaternion.LookRotation(tangent, normal);

            time += Time.deltaTime;
        } else {
            time = 0;
        }
    }
}
