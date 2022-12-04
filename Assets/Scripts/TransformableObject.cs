using System;
using UnityEngine;

public class TransformableObject : MonoBehaviour
{
    public static event Action<Transform> OnSelected;

    private void OnMouseDown()
    {
        OnSelected?.Invoke(transform);
    }
}
