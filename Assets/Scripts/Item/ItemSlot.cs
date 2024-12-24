using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public static event EventHandler OnAnyItemSlotSelected;
    [SerializeField] private TextMeshProUGUI itemAmountText;
    [SerializeField] private Image itemIcon;
    Item slotItem;
    [SerializeField] private Button itemRemoveButton;
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private GameObject equipButton;
    [SerializeField] private GameObject unEquipButton;
    public bool Selected { get; private set; }
    public Sprite itemSprite { get { return itemIcon.sprite; } }
    public Button removeButton { get; private set; }
    public Item item { 
        get
        {
            return slotItem;
        }
        set
        {
            slotItem = value;
            itemIcon.sprite = slotItem.Data.itemIcon;
            itemAmountText.text = slotItem.Amount.ToString();
        }
    }

    public bool hasItem { get { return item != null; } }

    public void Start()
    {
        if (itemRemoveButton != null) removeButton = itemRemoveButton;
        if (itemIcon == null) itemIcon = transform.GetChild(0).GetComponent<Image>();
        if (removeButton == null) removeButton = transform.GetChild(1).GetComponent<Button>();

        itemRemoveButton.onClick.AddListener(() =>
        {
            if (hasItem)
                Player.Instance.Inventory.DropItem(slotItem);
        });
    }

    public void ClearItem()
    {
        slotItem = null;
        itemIcon.sprite = InventoryUI.emptyInventoryIcon;
        itemAmountText.text = "0";
    }

    private void HideSelectedVisual()
    {
        selectedVisual.SetActive(false);
    }
    public static void Select(ItemSlot itemSlot)
    {
        itemSlot.Selected = true;
        itemSlot.ShowSelectedVisual();

        IEquippable itemIEquippable = itemSlot.item as IEquippable;
        if (itemIEquippable != null && !itemIEquippable.IsEquipped)
        {
            itemSlot.equipButton.SetActive(true);
        }
        else if (itemIEquippable != null && itemIEquippable.IsEquipped)
        {
            itemSlot.unEquipButton.SetActive(true);
        }
        OnAnyItemSlotSelected?.Invoke(itemSlot, EventArgs.Empty);
    }

    public static void DeSelect(ItemSlot itemSlot)
    {
        itemSlot.Selected = false;
        itemSlot.HideSelectedVisual();
        if (itemSlot.equipButton.activeSelf) itemSlot.equipButton.SetActive(false);
    }
    private void ShowSelectedVisual()
    {
        selectedVisual.SetActive(true);
    }

    public void TryEquipItem()
    {
        if (item is IEquippable) item.GetComponent<IEquippable>().Equip();
    }
    public void UnEquipItem()
    {
        if (item is IUnEquippable) item.GetComponent<IUnEquippable>().UnEquip();
    }
}
