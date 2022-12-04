using System;
using UnityEngine;

public class DraggableButton : MonoBehaviour
{
    public event Action<bool> OnHover;
    public event Action OnDragStart;
    public event Action OnDragEnd;

    Vector3 lastMousePos = Vector3.zero;
    bool isBeingDragged = false;

    [SerializeField] bool enableRotation = false;
    float rotationSpeed = 40f;

    private void Update()
    {
        if (enableRotation && !isBeingDragged)
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnMouseEnter() => OnHover?.Invoke(true);

    private void OnMouseDown()
    {
        isBeingDragged = true;

        lastMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        OnDragStart?.Invoke();
    }

    private void OnMouseDrag()
    {
        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        Vector3 delta = currentMousePos - lastMousePos;

        lastMousePos = currentMousePos;

        transform.position += delta;
    }

    private void OnMouseUpAsButton()
    {
        isBeingDragged = false;

        OnDragEnd?.Invoke();
    }

    private void OnMouseExit()
    {
        OnHover?.Invoke(false);
    }
}
