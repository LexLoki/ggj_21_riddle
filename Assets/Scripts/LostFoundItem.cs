using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LostFoundItem : MonoBehaviour
{
    public string Item;
    public Text ItemLabel;

    public void SetItem(string item)
    {
        Item = item;
        ItemLabel.text = item;

    }

}
