using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemSlothParent;
    private List<GameObject> itemSloths;

    private void Start()
    {
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

    private void UpdateInventoryUI()
    {
        foreach (GameObject itemSlot in itemSloths)
        {
            //itemSlot.transform.GetChild(0).GetComponent<Image>().;
        }
    }
}
