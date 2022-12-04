using System.Collections;
using UnityEngine;

public class MenuItem : MonoBehaviour
{
    public GameObject itemPrefab;
    [SerializeField] DraggableButton button;

    Animator m_Animator;
    LayerMask dockableLayer;
    [SerializeField] float dockRadius = 0.5f;
    bool buttonIsBeingDragged = false;
    bool buttonIsDocked = true;


    private void OnEnable()
    {
        button.OnDragStart += OnButtonDragStart;
        button.OnDragEnd += OnButtonDragEnd;
        button.OnHover += OnButtonHover;
    }

    private void OnDisable()
    {
        button.OnDragStart -= OnButtonDragStart;
        button.OnDragEnd -= OnButtonDragEnd;
        button.OnHover -= OnButtonHover;
    }

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        dockableLayer = LayerMask.GetMask("Dockable");

        RefreshAppearance();
    }

    // Refresh this item's button mesh and rotation to reflect current object.
    public void RefreshAppearance()
    {
        button.GetComponent<MeshFilter>().mesh = itemPrefab.GetComponent<MeshFilter>().sharedMesh;
        button.transform.rotation = itemPrefab.transform.rotation;
    }

    private void OnButtonDragStart()
    {
        StartCoroutine(ActiveDock());
    }

    private void OnButtonDragEnd()
    {
        buttonIsBeingDragged = false;

        // If button is dropped within the docking area, do not spawn.
        if (!buttonIsDocked)
            SpawnPrefab(button.transform);

        ReturnButtonToDock();
    }

    private void OnButtonHover(bool isHovered)
    {
        m_Animator.SetBool("isHovered", isHovered);
    }

    private void SpawnPrefab(Transform spawnTransform)
    {
        Instantiate(itemPrefab, spawnTransform.position, itemPrefab.transform.rotation);
    }

    private void ReturnButtonToDock()
    {
        m_Animator.SetBool("isDocked", true);
        button.transform.position = transform.position;
    }

    /// <summary>
    /// While the button is being dragged, this coroutine monitor whether or not it lies within the dock area.
    /// </summary>
    /// <returns></returns>
    IEnumerator ActiveDock()
    {
        buttonIsBeingDragged = true;

        while (buttonIsBeingDragged)
        {
            CheckDockForButton();

            yield return null;
        }
    }

    private void CheckDockForButton()
    {
        buttonIsDocked = Physics.OverlapSphere(transform.position, dockRadius, dockableLayer).Length > 0;

        m_Animator.SetBool("isDocked", buttonIsDocked);
    }
}
