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
    [SerializeField] private GameObject selectedItemSlot;
    private GameObject SelectedItemSlot
    {
        get
        {
            return selectedItemSlot;
        }
        set
        {
            if (selectedItemSlot != null) selectedItemSlot.GetComponent<ItemSlot>().HideSelectedVisual();
            selectedItemSlot = value;
            selectedItemSlot.GetComponent<ItemSlot>().ShowSelectedVisual();
        }
    }

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

        emptyInventoryIcon = itemSlots[0].itemSprite;
    }

    private void Instance_OnCancelClicked(object sender, System.EventArgs e)
    {
        if (!isVisible) return;

        if (SelectedItemSlot.TryGetComponent(out ItemSlot itemSlot) && itemSlot.hasItem)
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
        if (isVisible)
        {
            EventSystem.current.SetSelectedGameObject(itemSlots[currentIndex].gameObject);
            SelectedItemSlot = EventSystem.current.currentSelectedGameObject;
        }
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

    public void SelectNextItem()
    {
        currentIndex = (currentIndex + 1) % itemSlots.Count;
        EventSystem.current.SetSelectedGameObject(itemSlots[currentIndex].gameObject);
        SelectedItemSlot = EventSystem.current.currentSelectedGameObject;
    }

    public void SelectPreviousItem()
    {
        currentIndex = (currentIndex - 1 + itemSlots.Count) % itemSlots.Count;
        EventSystem.current.SetSelectedGameObject(itemSlots[currentIndex].gameObject);
        SelectedItemSlot = EventSystem.current.currentSelectedGameObject;
    }
}
