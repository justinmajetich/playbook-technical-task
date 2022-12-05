using UnityEngine;

/// <summary>
/// Handles translation, rotation, and scaling operations on a target object. Responds to events registered through Control Handles.
/// </summary>
public class TransformControls : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField, Tooltip("Parent of handle objects")] GameObject handles;

    [Space]
    [SerializeField] bool useLocalSpace = true;

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

        // Check for change in transformation space.
        CheckForTransformationSpaceToggle();

        if (targetTransform != null)
        {
            // Position gizmo at a set distance from the camera and in line with the target object.
            Vector3 cameraToTarget = targetTransform.position - Camera.main.transform.position;

            Quaternion rotation = useLocalSpace ? targetTransform.rotation : Quaternion.identity;

            transform.SetPositionAndRotation(Camera.main.transform.position + (cameraToTarget.normalized * distanceFromCamera), rotation);
        }
        else
        {
            handles.SetActive(false);
        }
    }

    private void OnLinearHandleDrag(Transformation transformation, Axis axis, Vector3 mouseDelta)
    {
        Vector3 transformationVector = GetLinearTransformationVector(axis, mouseDelta);

        if (transformation == Transformation.Translation)
            Translate(transformationVector, mouseDelta);
        else
            Scale(transformationVector, mouseDelta);
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
    /// <param name="translationVector">Vector representing both the direction of transformation and the influence of mouse movement relative to the transformation axis.</param>
    /// <param name="mouseDelta">Mouse movement delta for current increment of drag.</param>
    private void Translate(Vector3 translationVector, Vector3 mouseDelta)
    {
        // Scale translation speed modifier by distance between camera plane and target transform.
        float translationModifier = Vector3.Distance(Camera.main.transform.position, targetTransform.position) * translationStrengthModifier;

        // Multipy translation direction by magnitude of mouse movement and apply to object transform.
        targetTransform.position += mouseDelta.magnitude * translationModifier * translationVector;
    }

    /// <summary>
    /// Scales the target object along the given axis.
    /// </summary>
    /// <param name="scalingVector">Vector representing both the direction of transformation and the influence of mouse movement relative to the transformation axis.</param>
    /// <param name="mouseDelta">Mouse movement delta for current increment of drag.</param>
    private void Scale(Vector3 scalingVector, Vector3 mouseDelta)
    {
        // Convert scalingDir to local space and scale vector by magnitude and modifier.
        Vector3 localScalingDelta = mouseDelta.magnitude * scalingStrengthModifier * targetTransform.InverseTransformDirection(scalingVector);

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
        Vector3 rotationAxis = ConvertAxisToWorldSpace(axis);

        // Get offset from control rig center to point on gimbal where mouse began drag.
        Vector3 gimbalOffset = initialDragPos - transform.position;

        // Since the rotation axis and gimbal offset will always be perpendicular,
        // the cross product can be used to find a third vector perpendicular to the first two
        // and represent the axis at which the rotation gimbal can be intuitively "dragged".
        Vector3 dragAxis = Vector3.Cross(rotationAxis, gimbalOffset.normalized);

        // Take the dot product of the mouse delta and drag axis to determine the direction and influence of the mouse input on rotation.
        targetTransform.Rotate(rotationAxis, Vector3.Dot(mouseDelta.normalized, dragAxis) * rotationStrengthModifier * mouseDelta.magnitude, Space.World);
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

    private void CheckForTransformationSpaceToggle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            useLocalSpace = !useLocalSpace;
            ToggleScaleHandles();
        }
    }

    /// <summary>
    /// Converts an Axis enum into world space.
    /// </summary>
    /// <param name="axis">Axis enum to be converted</param>
    /// <returns></returns>
    private Vector3 ConvertAxisToWorldSpace(Axis axis)
    {
        if (useLocalSpace)
        {
            return axis switch
            {
                Axis.X => targetTransform.right,
                Axis.Y => targetTransform.up,
                Axis.Z => targetTransform.forward,
                _ => Vector3.zero
            };
        }
        else
        {
            return axis switch
            {
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => Vector3.zero
            };
        }
    }

    /// <summary>
    /// Calculates the direction and strength of mouse delta influence along the transformation axis using the dot product of mouse delta and perceived transformation axis.
    /// </summary>
    /// <param name="axis">Enum representing the axis along which to transform.</param>
    /// <param name="mouseDelta">Mouse movement delta for current operation.</param>
    /// <returns>A vector representing both the direction of transformation and the influence of mouse movement relative to the transformation axis.</returns>
    private Vector3 GetLinearTransformationVector(Axis axis, Vector3 mouseDelta)
    {
        Vector3 transformationAxis = ConvertAxisToWorldSpace(axis);

        // Get the perceived operating axis relative to camera perpective for comparison with mouseDelta. This is useful when the transformation axis is parallel but offset from camera forward.
        // For example, if the screen plane is perfectly perpendicular to a operating axis, the dot product of mouse delta and axis will be 0, and thus, mouse movement will have no effect.
        // This interaction may be unintuitive to the user if the gizmo is offset from camera forward (i.e. because of perspective, it can be seen as a line rather than a point on the screen).
        // By finding the perceived operating axis, we can compare it to the mouse delta and receive a more intuitive and apparent result.
        Vector3 perceivedAxis = Quaternion.Euler(Vector3.Angle(Camera.main.transform.forward, (transform.position - Camera.main.transform.position).normalized), 0f, 0f) * transformationAxis;

        // Use dot product to determine the direction and influence of the mouse delta along the transformation axis.
        Vector3 transformationVector = transformationAxis * Vector3.Dot(mouseDelta.normalized, perceivedAxis);

        return transformationVector;
    }

    /// <summary>
    /// Toggle scaling handles' activation state. World space scale transformations are not supported and will be deativated when world space transformation is active.
    /// </summary>
    private void ToggleScaleHandles()
    {
        for (int i = 0; i < handles.transform.childCount; i++)
        {
            LinearHandle handle = handles.transform.GetChild(i).gameObject.GetComponent<LinearHandle>();

            if (handle != null && handle.transformation == Transformation.Scale)
            {
                handles.transform.GetChild(i).gameObject.SetActive(!handles.transform.GetChild(i).gameObject.activeSelf);
            }
        }
    }
}
