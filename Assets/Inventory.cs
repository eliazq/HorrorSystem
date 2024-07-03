using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    List<Item> items = new List<Item>(8);

    public void AddItem(Item item)
    {
        if (items.Count < 8)
        {
            items.Add(item);
        }
        else
        {
            Debug.Log("Not enough space in inventroy", gameObject);
            return;
        }
    }

    public Item GetItem(string name)
    {
        foreach (Item item in items)
        {
            if (item.Data.name == name)
            {
                return item;
            }
        }
        Debug.LogWarning("Not item in inventroy but tried to acess it", gameObject);
        return null;
    }
    
    public void DropItem(Item item)
    {
        item.transform.SetParent(null);
        item.AddComponent<Rigidbody>();
        items.Remove(item);
    }

}
