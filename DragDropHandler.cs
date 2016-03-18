using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class DragDropHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;
        if (transform.parent != startParent)
        {
            transform.position = startPosition;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
