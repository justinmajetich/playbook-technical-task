using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Provide behavior for gimbal handles.
/// </summary>
public class GimbalHandle : ControlHandle
{
    public static event Action<Axis, Vector3, Vector3> OnDrag;

    [SerializeField] Axis axis;

    Vector3 initialDragPosition;

    private void OnMouseDown()
    {
        // At beginning of drag, store intial mouse position and position along the gimbal control where drag began.
        lastMousePos = MousePositionToWorldSpace();
        initialDragPosition = GetMousePositionOnHandle();
    }

    private void OnMouseDrag()
    {
        OnDrag?.Invoke(axis, GetMouseMovementDelta(), initialDragPosition);
    }

    /// <summary>
    /// Cast a ray from mouse screen position into the handle collider. 
    /// </summary>
    /// <returns>The world space position along the gimbal control where the user initiated the drag</returns>
    private Vector3 GetMousePositionOnHandle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        Physics.Raycast(ray, out hitInfo);

        return hitInfo.point;
    }
}
