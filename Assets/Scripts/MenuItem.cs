using System;
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

    // Update the button meshes to current item.
    public void RefreshAppearance()
    {
        button.GetComponent<MeshFilter>().mesh = itemPrefab.GetComponent<MeshFilter>().sharedMesh;
    }

    private void OnButtonDragStart()
    {
        StartCoroutine(ActiveDock());
    }

    private void OnButtonDragEnd()
    {
        buttonIsBeingDragged = false;

        if (!buttonIsDocked)
            SpawnPrefab(button.transform);

        ReturnButtonToDock();
    }

    private void OnButtonHover(bool isHovered)
    {
        m_Animator.SetBool("isHovered", isHovered);
    }

    private void CheckDockForButton()
    {
        buttonIsDocked = Physics.OverlapSphere(transform.position, dockRadius, dockableLayer).Length > 0;

        m_Animator.SetBool("isDocked", buttonIsDocked);
    }

    private void SpawnPrefab(Transform spawnTransform)
    {
        Instantiate(itemPrefab, spawnTransform.position, Quaternion.identity);
    }

    private void ReturnButtonToDock()
    {
        m_Animator.SetBool("isDocked", true);
        button.transform.position = transform.position;
    }

    IEnumerator ActiveDock()
    {
        buttonIsBeingDragged = true;

        while (buttonIsBeingDragged)
        {
            CheckDockForButton();

            yield return null;
        }
    }
}
