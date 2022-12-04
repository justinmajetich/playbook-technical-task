using UnityEngine;

/// <summary>
/// Provides additional settings and behavior for the camera rendering the transformation gizmo.
/// </summary>
public class TransformGizmoCamera : MonoBehaviour
{
    [SerializeField, Tooltip("Use to offset clipping plane from center of transformGizmo.")]
    float farClippingPlaneOffset = 0.35f;

    [SerializeField] Transform transformGizmo;
    Camera thisCamera;

    private void Start()
    {
        thisCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        // Adjust the clipping plane to the gizmo's scene depth. Using a shader to handle culling back portions of the rotation gimbals
        // would be a more elegant solution, but for the sake of this demo, I'm brute-forcing by manipulating the clipping plane.
        Vector3 offset = transformGizmo.position - transform.position;

        float alphaAngle = Vector3.Angle(transform.forward, offset.normalized);
        
        // Multiply the hypotenuse by the cosine of alpha angle to get the length of adjacent side (i.e. scene depth of gizmo).
        thisCamera.farClipPlane = Vector3.Distance(transform.position, transformGizmo.position) * Mathf.Cos(alphaAngle * Mathf.Deg2Rad) + farClippingPlaneOffset;
    }
}
