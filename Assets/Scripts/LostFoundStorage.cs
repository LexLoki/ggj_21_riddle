using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LostFoundStorage : MonoBehaviour
{

    public GameObject LostFoundItemPrefab;

    public Transform Container;

    public Action<LostFoundItem, Action> ItemDropResolver;

    private List<LostFoundItem> _items;

    public void SetStorage(List<string> items)
    {
        _items = items.ConvertAll(CreateItem);
    }

    public void RemoveItem(LostFoundItem item, bool shouldDestroy)
    {
        _items.Remove(item);
        if(shouldDestroy)
        {
            GameObject.Destroy(item.gameObject);
        }
    }

    private void ProcessDrop(Transform itemTransform, Action rejectCallback)
    {
        LostFoundItem droppedItem = _items.Find(item => item.transform == itemTransform);
        if (droppedItem == null) rejectCallback();
        else ProcessDrop(droppedItem, rejectCallback);
    }

    private void ProcessDrop(LostFoundItem droppedItem, Action rejectCallback)
    {
        if (ItemDropResolver == null) rejectCallback();
        else ItemDropResolver(droppedItem, rejectCallback);
    }

    private LostFoundItem CreateItem(string item)
    {
        GameObject inst = Instantiate(LostFoundItemPrefab, transform);
        LostFoundItem lfItem = inst.GetComponent<LostFoundItem>();
        lfItem.SetItem(item);

        DragDrop dDrop = inst.GetComponent<DragDrop>();
        dDrop.Container = Container;
        dDrop.ProcessDrop = (t,call) => ProcessDrop(lfItem, call);

        return lfItem;
    }

}
