using System.Collections.Generic;
using UnityEngine;

public class MenuContentManager : MonoBehaviour
{
    [SerializeField] List<GameObject> objLibrary = new List<GameObject>();

    [SerializeField] MenuArrow forwardArrow;
    [SerializeField] MenuArrow backwardArrow;

    private void Awake()
    {
        AssignLibraryToMenuItems();
    }

    private void OnEnable()
    {
        forwardArrow.OnClick += OnRotateLibraryForward;
        backwardArrow.OnClick += OnRotateLibraryBackward;
    }

    private void OnDisable()
    {
        forwardArrow.OnClick -= OnRotateLibraryForward;
        backwardArrow.OnClick -= OnRotateLibraryBackward;
    }

    private void AssignLibraryToMenuItems()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            MenuItem menuItem = transform.GetChild(i).GetComponent<MenuItem>();
            
            menuItem.itemPrefab = objLibrary[i];
            menuItem.RefreshAppearance();
        }
    }

    private void OnRotateLibraryForward()
    {
        GameObject temp = objLibrary[0];
        objLibrary.RemoveAt(0);
        objLibrary.Add(temp);

        AssignLibraryToMenuItems();
    }

    private void OnRotateLibraryBackward()
    {
        GameObject temp = objLibrary[objLibrary.Count - 1];
        objLibrary.RemoveAt(objLibrary.Count - 1);
        objLibrary.Insert(0, temp);

        AssignLibraryToMenuItems();
    }
}
