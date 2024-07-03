using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject itemSlothParent;
    [SerializeField] private Sprite emptyInventoryIcon;
    private List<GameObject> itemSloths = new List<GameObject>();

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        for (int i = 0; i < itemSlothParent.transform.childCount; i++)
        {
            itemSloths.Add(itemSlothParent.transform.GetChild(i).gameObject);
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
    }

    private void UpdateInventoryUI()
    {
        foreach (GameObject item in itemSloths)
        {
            item.transform.GetChild(0).GetComponent<Image>().sprite = emptyInventoryIcon;
        }
        for (int i = 0; i <= inventory.ItemCount() - 1; i++)
        {
            itemSloths[i].transform.GetChild(0).GetComponent<Image>().sprite = inventory.GetItem(i).Data.itemIcon;
        }
    }
}
