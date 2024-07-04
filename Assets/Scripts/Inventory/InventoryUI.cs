using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject itemSlothParent;
    [SerializeField] public static Sprite emptyInventoryIcon;
    private List<ItemSlot> itemSloths = new List<ItemSlot>();

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        for (int i = 0; i < itemSlothParent.transform.childCount; i++)
        {
            itemSloths.Add(itemSlothParent.transform.GetChild(i).GetComponent<ItemSlot>());
        }

        foreach (ItemSlot itemSlot in itemSloths)
        {
            // Use a local variable to capture the current itemSlot correctly
            ItemSlot currentSlot = itemSlot;
            currentSlot.removeButton.onClick.AddListener(() =>
            {
                if (!currentSlot.hasItem)
                {
                    return;
                }
                inventory.DropItem(currentSlot.item);
                currentSlot.ClearItem();
            });
        }

        GetComponent<Inventory>().OnInventoryChanged += InventoryUI_OnInventoryChanged;
    }

    private void InventoryUI_OnInventoryChanged(object sender, System.EventArgs e)
    {
        UpdateInventoryUI();        
    }

    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        Cursor.lockState = inventoryUI.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = inventoryUI.activeSelf;
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < itemSloths.Count; i++)
        {
            if (inventory.ItemCount() > i)
            {
                itemSloths[i].item = inventory.GetItem(i);
            }
            else if (itemSloths[i].hasItem)
            {
                itemSloths[i].ClearItem();
            }
        }
    }
}
