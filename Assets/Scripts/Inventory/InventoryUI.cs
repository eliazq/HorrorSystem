using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject itemSlotParent;
    [SerializeField] public static Sprite emptyInventoryIcon;
    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    public bool isVisible { get { return inventoryUI.activeSelf; } }

    private int currentIndex = 0;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        for (int i = 0; i < itemSlotParent.transform.childCount; i++)
        {
            itemSlots.Add(itemSlotParent.transform.GetChild(i).GetComponent<ItemSlot>());
        }

        GetComponent<Inventory>().OnInventoryChanged += InventoryUI_OnInventoryChanged;
        InputManager.Instance.OnInventoryToggle += Instance_OnInventoryToggle;
        InputManager.Instance.OnAcceptClicked += Instance_OnAcceptClicked;
        InputManager.Instance.OnArrowClicked += Instance_OnArrowClicked;
        InputManager.Instance.OnCancelClicked += Instance_OnCancelClicked;

        ControllerManager.OnControllerDeviceChanged += Instance_OnUsingDeviceChanged;

        emptyInventoryIcon = itemSlots[0].itemSprite;

        ItemSlot.OnAnyItemSlotSelected += ItemSlot_OnAnyItemSlotSelected;
    }

    private void ItemSlot_OnAnyItemSlotSelected(object sender, EventArgs e)
    {
        // Ensure the sender is a valid ItemSlot
        if (sender is not ItemSlot selectedItemSlot) return;

        foreach (ItemSlot itemSlot in itemSlots)
        {
            // Deselect all item slots except the one that triggered the event
            if (itemSlot.Selected && itemSlot != selectedItemSlot)
            {
                ItemSlot.DeSelect(itemSlot);
            }
        }
    }

    private void OnDestroy()
    {
        ControllerManager.OnControllerDeviceChanged -= Instance_OnUsingDeviceChanged;
        ItemSlot.OnAnyItemSlotSelected -= ItemSlot_OnAnyItemSlotSelected;
    }

    private void Instance_OnUsingDeviceChanged(object sender, EventArgs e)
    {
        if (InputManager.isUsingController && isVisible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Instance_OnCancelClicked(object sender, System.EventArgs e)
    {
        if (!isVisible) return;

        if (EventSystem.current.TryGetComponent(out ItemSlot itemSlot) && itemSlot.hasItem)
        {
            Player.Instance.Inventory.DropItem(itemSlot.item);
        }
    }

    private void Instance_OnArrowClicked(object sender, InputManager.OnArrowClickedEventArgs e)
    {
        switch (e.arrowKey)
        {
            case InputManager.ArrowKey.Left:
                SelectPreviousItem();
                break;
            case InputManager.ArrowKey.Right:
                SelectNextItem();
                break;
            case InputManager.ArrowKey.Up:
                break;
            case InputManager.ArrowKey.Down:
                break;
        }
    }

    private void Instance_OnAcceptClicked(object sender, System.EventArgs e)
    {
        if (!isVisible) return; // Inventory not visible
        // TODO: Select itemSlot
    }

    private void Instance_OnInventoryToggle(object sender, System.EventArgs e)
    {
        ToggleInventory();
        if (!isVisible)
        {
            InputManager.Instance.WeaponHandlingInputsActive = true;
            return;
        }
        
        // INVENTORY VISIBLE
        EventSystem.current.SetSelectedGameObject(itemSlots[currentIndex].gameObject);
        ItemSlot.Select(EventSystem.current.currentSelectedGameObject.GetComponent<ItemSlot>());

        InputManager.Instance.WeaponHandlingInputsActive = false;
        
        if (InputManager.isUsingController)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

    private void InventoryUI_OnInventoryChanged(object sender, System.EventArgs e)
    {
        UpdateInventoryUI();        
    }

    private void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        Cursor.lockState = inventoryUI.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = inventoryUI.activeSelf; 
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            ItemSlot itemSlot = itemSlots[i];
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

    private void SelectNextItem()
    {
        currentIndex = (currentIndex + 1) % itemSlots.Count;
        EventSystem.current.SetSelectedGameObject(itemSlots[currentIndex].gameObject);
        ItemSlot.Select(EventSystem.current.currentSelectedGameObject.GetComponent<ItemSlot>());
    }

    private void SelectPreviousItem()
    {
        currentIndex = (currentIndex - 1 + itemSlots.Count) % itemSlots.Count;
        EventSystem.current.SetSelectedGameObject(itemSlots[currentIndex].gameObject);
        ItemSlot.Select(EventSystem.current.currentSelectedGameObject.GetComponent<ItemSlot>());
    }

}
