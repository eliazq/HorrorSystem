using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    Item slotItem;
    [SerializeField] private Button itemRemoveButton;
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
        }
    }

    public bool hasItem { get { return item != null; } }

    private void Start()
    {
        if (itemRemoveButton != null) removeButton = itemRemoveButton;
        if (itemIcon == null) itemIcon = transform.GetChild(0).GetComponent<Image>();
        if (removeButton == null) removeButton = transform.GetChild(1).GetComponent<Button>();
    }

    public void ClearItem()
    {
        slotItem = null;
        itemIcon.sprite = InventoryUI.emptyInventoryIcon;
    }
}
