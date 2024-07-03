using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using NaughtyAttributes;

public class Inventory : MonoBehaviour
{
    [SerializeField] private const int size = 8;
    List<Item> items = new List<Item>(size);

    bool isDropItemOnCooldown = false;

    public void AddItem(Item item)
    {
        if (!CanAddItem(item)) return;

        items.Add(item);
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.gameObject.SetActive(false);
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
        if (isDropItemOnCooldown) return;

        item.transform.SetParent(null);
        item.gameObject.SetActive(true);
        if (item.GetComponent<Rigidbody>() == null)
            item.AddComponent<Rigidbody>();
        DisablePhysicsFromObject(item.gameObject, 3f);
        items.Remove(item);
    }

    private void DisablePhysicsFromObject(GameObject gameObject, float waitTime)
    {
        if (!isDropItemOnCooldown)
            StartCoroutine(DisablePhysicsFromObjectEnumerator(waitTime, gameObject));
    }
    IEnumerator DisablePhysicsFromObjectEnumerator(float waitTime, GameObject gameObject)
    {
        isDropItemOnCooldown = true;
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject.GetComponent<Rigidbody>());
        isDropItemOnCooldown = false;
    }

}
