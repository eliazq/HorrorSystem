using UnityEngine;

public class ItemManager : MonoBehaviour
{
    static ItemManager Instance;

    [SerializeField] ItemSO[] itemDatas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    static public Item CreateItem(string itemName, int amount = 1)
    {
        ItemSO itemData = GetItemData(itemName);
        if (itemData == null) return null;
        Item item = Instantiate(itemData.itemPrefab, null).GetComponent<Item>();
        item.Amount = amount;
        return item;
    }

    static private ItemSO GetItemData(string itemName)
    {
        foreach (ItemSO itemData in Instance.itemDatas)
        {
            if (itemData.itemName == itemName)
            {
                return itemData;
            }
        }
        Debug.LogWarning("Not match in ItemDatas", Instance.gameObject);
        return null;
    }

}
