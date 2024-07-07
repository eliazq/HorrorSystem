using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemAmountText;
    [SerializeField] private Image itemIcon;
    Item slotItem;
    [SerializeField] private Button itemRemoveButton;
    [SerializeField] private GameObject selectedVisual;
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

    private void Start()
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

    public void HideSelectedVisual()
    {
        selectedVisual.SetActive(false);
    }
    public void ShowSelectedVisual()
    {
        selectedVisual.SetActive(true);
    }
}
