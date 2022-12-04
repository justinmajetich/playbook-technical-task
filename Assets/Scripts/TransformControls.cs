using UnityEngine;

/// <summary>
/// Handles translation, rotation, and scaling operations on a target object. Responds to events registered through Control Handles.
/// </summary>
public class TransformControls : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField, Tooltip("Parent of handle objects")] GameObject handles;

    [Space]

    [SerializeField, Tooltip("The transformGizmo will maintain this constant distance from camera, regardless of target object's position in the scene.")] 
    float distanceFromCamera = 6f;

    [Header("Translation")]
    [SerializeField] float translationStrengthModifier = 3f;

    [Header("Rotation")]
    [SerializeField] float rotationStrengthModifier = 1f;

    [Header("Scaling")]
    [SerializeField] bool allowNegativeScaling = false;
    [SerializeField] float scalingStrengthModifier = 40f;

    private void OnEnable()
    {
        LinearHandle.OnDrag += OnLinearHandleDrag;
        GimbalHandle.OnDrag += Rotate;
        TransformableObject.OnSelected += OnTransformableObjectSelected;
    }

    private void OnDisable()
    {
        LinearHandle.OnDrag -= OnLinearHandleDrag;
        GimbalHandle.OnDrag -= Rotate;
    }

    private void Update()
    {
        // Check if a target transform is still selected.
        ValidateTargetTransformStillSelected();

        if (targetTransform != null)
        {
            // Position gizmo at a set distance from the camera and in line with the target object.
            Vector3 cameraToTarget = targetTransform.position - Camera.main.transform.position;

            transform.SetPositionAndRotation(Camera.main.transform.position + (cameraToTarget.normalized * distanceFromCamera), targetTransform.rotation);
        }
        else
        {
            handles.SetActive(false);
        }

    }

    private void OnLinearHandleDrag(Transformation transformation, Axis axis, Vector3 mouseDelta)
    {
        Vector3 transformationAxis = ConvertLocalAxisToWorldSpace(axis);

        if (transformation == Transformation.Translation)
            Translate(transformationAxis, mouseDelta);
        else
            Scale(transformationAxis, mouseDelta);
    }

    private void OnTransformableObjectSelected(Transform selectedTransform)
    {
        targetTransform = selectedTransform;
        
        if (!handles.activeSelf)
            handles.SetActive(true);
    }

    /// <summary>
    /// Translates the target object along the given axis.
    /// </summary>
    /// <param name="translationAxis">Enum representing the axis along which to translate.</param>
    /// <param name="mouseDelta">Mouse movement delta for current increment of drag.</param>
    private void Translate(Vector3 translationAxis, Vector3 mouseDelta)
    {
        // Use dot product to determine the direction and influence of the mouse delta along the translation axis.
        Vector3 translationDir = translationAxis * Vector3.Dot(mouseDelta.normalized, translationAxis);

        // Scale translation speed modifier by distance between camera plane and target transform.
        float translationModifier = Vector3.Distance(Camera.main.transform.position, targetTransform.position) * translationStrengthModifier;

        // Multipy translation direction by magnitude of mouse movement and apply to object transform.
        targetTransform.position += translationDir * translationModifier * mouseDelta.magnitude;
    }

    /// <summary>
    /// Scales the target object along the given axis.
    /// </summary>
    /// <param name="scalingAxis">Enum representing the axis along which to scale.</param>
    /// <param name="mouseDelta">Mouse movement delta for current increment of drag.</param>
    private void Scale(Vector3 scalingAxis, Vector3 mouseDelta)
    {
        // Use dot product to determine the direction and influence of the mouse delta along the scaling axis.
        Vector3 scalingDir = scalingAxis * Vector3.Dot(mouseDelta.normalized, scalingAxis);

        // Convert scalingDir to local space and scale vector by magnitude and modifier.
        Vector3 localScalingDelta = (targetTransform.InverseTransformDirection(scalingDir) * scalingStrengthModifier * mouseDelta.magnitude);

        Vector3 newLocalScale = targetTransform.localScale + localScalingDelta;

        // If negative scaling is disallowed, clamp scale.
        if (!allowNegativeScaling)
        {
            newLocalScale = new Vector3(
                Mathf.Clamp(newLocalScale.x, 0.0001f, float.MaxValue),
                Mathf.Clamp(newLocalScale.y, 0.0001f, float.MaxValue),
                Mathf.Clamp(newLocalScale.z, 0.0001f, float.MaxValue)
            );
        }

        targetTransform.localScale = newLocalScale;
    }


    /// <summary>
    /// Rotates the target object around the given axis.
    /// </summary>
    /// <param name="axis">Enum representing the axis around which to rotate.</param>
    /// <param name="mouseDelta">Mouse movement delta for current increment of drag.</param>
    /// <param name="initialDragPos">The world space position along the gimbal control where the user initiated the drag.</param>
    private void Rotate(Axis axis, Vector3 mouseDelta, Vector3 initialDragPos)
    {
        // Convert local axis to world space.
        Vector3 rotationAxis = ConvertLocalAxisToWorldSpace(axis);

        // Get offset from control rig center to point on gimbal where mouse began drag.
        Vector3 gimbalOffset = initialDragPos - transform.position;

        // Since the rotation axis and gimbal offset will always be perpendicular,
        // the cross product can be used to find a third vector perpendicular to the first two
        // and represent the axis at which the rotation gimbal can be intuitively "dragged".
        Vector3 dragAxis = Vector3.Cross(rotationAxis, gimbalOffset.normalized);

        // Take the dot product of the mouse delta and drag axis to determine the direction and influence of the mouse input on rotation.
        targetTransform.Rotate(rotationAxis, Vector3.Dot(mouseDelta.normalized, dragAxis) * rotationStrengthModifier, Space.World);
    }

    /// <summary>
    /// Check to see if the user has clicked on something other than a transformable object or gizmo handle.
    /// </summary>
    private void ValidateTargetTransformStillSelected()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LayerMask layerMask = LayerMask.GetMask(new string[] { "Transformable", "LinearHandle", "GimbalHandle" });
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, 100f, layerMask))
                targetTransform = null;
        }
    }

    /// <summary>
    /// Converts an Axis enum into the target object's local space.
    /// </summary>
    private Vector3 ConvertLocalAxisToWorldSpace(Axis axis)
    {
        return axis switch
        {
            Axis.X => targetTransform.right,
            Axis.Y => targetTransform.up,
            Axis.Z => targetTransform.forward,
            _ => Vector3.zero
        };
    }
}
