using System;
using UnityEngine;

/// <summary>
/// 3D button functionality for menu arrows.
/// </summary>
public class MenuArrow : MonoBehaviour
{
    public event Action OnClick;
    Animator m_Animator;

    private void Start() => m_Animator = GetComponent<Animator>();

    private void OnMouseEnter()
    {
        m_Animator.SetBool("isHovered", true);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke();
            m_Animator.SetTrigger("onClick");
        }
    }

    private void OnMouseExit()
    {
        m_Animator.SetBool("isHovered", false);
    }
}
