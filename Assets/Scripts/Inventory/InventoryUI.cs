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

        GetComponent<Inventory>().OnInventoryChanged += InventoryUI_OnInventoryChanged;
        InputManager.Instance.OnInventoryToggle += Instance_OnInventoryToggle;
    }

    private void Instance_OnInventoryToggle(object sender, System.EventArgs e)
    {
        ToggleInventory();
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
            ItemSlot itemSlot = itemSloths[i];
            if (inventory.TryGetItem(i, out Item item))
            {
                itemSlot.item = item;
            }
            else
            {
                itemSlot.ClearItem();
            }
        }
    }
}
