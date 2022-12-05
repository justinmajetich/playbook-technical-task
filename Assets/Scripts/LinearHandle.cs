using System;
using UnityEngine;

/// <summary>
/// Provide behavior for linear drag handles.
/// </summary>
public class LinearHandle : ControlHandle
{
    public static event Action<Transformation, Axis, Vector3> OnDrag;

    [SerializeField] Axis axis;
    public Transformation transformation;

    private void OnMouseDown()
    {
        // Recrod mouse position at beginning of drag.
        lastMousePos = MousePositionToWorldSpace();
    }

    private void OnMouseDrag()
    {
        OnDrag?.Invoke(transformation, axis, GetMouseMovementDelta());
    }
}