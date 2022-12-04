using System;
using UnityEngine;

/// <summary>
/// Draggable 3D button used with MenuItem for drag-and-drop object instantiation.
/// </summary>
public class DraggableButton : MonoBehaviour
{
    public event Action<bool> OnHover;
    public event Action OnDragStart;
    public event Action OnDragEnd;

    Vector3 lastMousePos = Vector3.zero;

    private void OnMouseEnter() => OnHover?.Invoke(true);

    private void OnMouseDown()
    {
        lastMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        OnDragStart?.Invoke();
    }

    /// <summary>
    /// Tracks the delta of current and last recorded mouse position on mouse drag.
    /// </summary>
    private void OnMouseDrag()
    {
        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        Vector3 delta = currentMousePos - lastMousePos;

        lastMousePos = currentMousePos;

        transform.position += delta;
    }

    private void OnMouseUpAsButton()
    {
        OnDragEnd?.Invoke();
    }

    private void OnMouseExit()
    {
        OnHover?.Invoke(false);
    }
}
