using UnityEngine;

/// <summary>
/// Base class for transformation control handles. Tracks mouse position and handles basic hover visualization.
/// </summary>
public class ControlHandle : MonoBehaviour
{
    MeshRenderer m_Renderer;
    [SerializeField] Material hoverMaterial;
    Material defaultMaterial;

    protected bool isBeingDragged = false;
    protected Vector3 lastMousePos;

    private void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        defaultMaterial = m_Renderer.material;
    }

    private void OnMouseEnter() => m_Renderer.material = hoverMaterial;

    private void OnMouseExit() => m_Renderer.material = defaultMaterial;

    /// <summary>
    /// Calculates the delta of current and last recorded mouse position.
    /// </summary>
    protected Vector3 GetMouseMovementDelta()
    {
        Vector3 currentMousePos = MousePositionToWorldSpace();

        Vector3 delta = currentMousePos - lastMousePos;

        lastMousePos = currentMousePos;

        return delta;
    }

    /// <summary>
    /// Converts mouse position into world space at a constant z-depth.
    /// </summary>
    protected Vector3 MousePositionToWorldSpace() => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.3f));
}
