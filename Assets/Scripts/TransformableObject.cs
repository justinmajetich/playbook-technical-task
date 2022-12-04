using System;
using UnityEngine;

/// <summary>
/// Sends a message on mouse down events on transformable objects.
/// </summary>
public class TransformableObject : MonoBehaviour
{
    public static event Action<Transform> OnSelected;

    private void OnMouseDown()
    {
        OnSelected?.Invoke(transform);
    }
}
