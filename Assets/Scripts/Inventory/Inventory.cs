using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event EventHandler OnInventoryChanged;
    private const int size = 8;
    List<Item> items = new List<Item>(size);

    public int ItemCount()
    {
        return items.Count;
    }

    public void AddItem(Item item)
    {
        if (!CanAddItem(item)) return;
        // If item thats stackable already in inventory, add to that item stack, forexamle: 5x coin
        if (item.Data.stackable)
        {
            if (TryAddToItemStack(item)) return;
        }
        
        // Add item to inventory
        items.Add(item);
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.gameObject.SetActive(false);
        OnInventoryChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool TryAddToItemStack(Item item)
    {
        foreach (Item i in items)
        {
            if (i.Data.itemName == item.Data.itemName)
            {
                i.Amount++;
                Destroy(item.gameObject);
                OnInventoryChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }
        return false;
    }

    private bool CanAddItem(Item item)
    {
        if (items.Contains(item))
        {
            Debug.LogWarning("Tried to add item to inventory that was already in inventory");
            return false;
        };
        if (items.Count > 8)
        {
            Debug.LogWarning("Not enough space in inventroy");
            return false;
        }
        return true;
    }

    public Item GetItem(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.Data.itemName == itemName)
            {
                return item;
            }
        }
        Debug.LogWarning("Not item in inventroy but tried to acess it", gameObject);
        return null;
    }

    public Item GetItem(int index)
    {
        if (items.Count - 1 < index) { Debug.LogError("Tried to access item with too long index"); return null; }
        return items[index];
    }

    public bool TryGetItem(int index, out Item outItem)
    {
        if (items.Count - 1 < index) 
        {
            outItem = null;
            return false;
        }
        outItem = items[index];
        return true;
    }

    public bool TryGetItem(string itemName, out Item outItem)
    {
        foreach (Item item in items)
        {
            if (item.Data.itemName == itemName)
            {
                outItem = item;
                return true;
            }
        }
        outItem = null;
        return false;
    }

    public bool HasItem(Item itemToCheck)
    {
        if (items.Contains(itemToCheck)) return true;
        return false;
    }

    public void DestroyItem(Item item)
    {
        bool removeItemFromList = true;
        if (item.Amount > 1)
        {
            item.Amount--;
            removeItemFromList = false;
        }
        if (removeItemFromList)
        {
            items.Remove(item);
            Destroy(item);
        }
        OnInventoryChanged?.Invoke(this, EventArgs.Empty);
    }

    public void DropItem(Item item)
    {
        bool removeItemFromList = true;
        if (item.Amount > 1)
        {
            item.Amount--;
            Vector3 itemDropPos = item.transform.position;
            item = Instantiate(item);
            item.transform.position = itemDropPos;
            removeItemFromList = false;
        }
        item.transform.SetParent(null);
        item.gameObject.SetActive(true);
        if (item.GetComponent<Rigidbody>() == null)
            item.AddComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        DisablePhysicsFromObject(item.gameObject, 3f);
        if (removeItemFromList)
            items.Remove(item);
        OnInventoryChanged?.Invoke(this, EventArgs.Empty);
    }

    private void DisablePhysicsFromObject(GameObject gameObject, float waitTime)
    {
        StartCoroutine(DisablePhysicsFromObjectEnumerator(waitTime, gameObject));
    }
    IEnumerator DisablePhysicsFromObjectEnumerator(float waitTime, GameObject gameObject)
    {
        yield return new WaitForSeconds(waitTime);
        if (gameObject != null && gameObject.TryGetComponent(out Rigidbody rb))
            Destroy(rb);
    }

}
