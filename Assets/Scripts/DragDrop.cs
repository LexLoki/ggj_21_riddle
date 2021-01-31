using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform Container;

    public Action<Transform, Action> ProcessDrop;

    private Transform _parent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _parent = transform.parent;
        transform.SetParent(Container, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position - new Vector2(Screen.width/2,Screen.height/2);
        //Debug.Log(eventData.position);
    }

    private void ReturnToParent()
    {
        transform.SetParent(_parent);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ProcessDrop != null)
        {
            ProcessDrop.Invoke(transform, ReturnToParent);
        }
        else
        {
            ReturnToParent();
        }
    }
    
}
