using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandage : Item, IEquippable
{
    [SerializeField] private int healthBonus = 10;

    public bool IsEquipped { get; set; }

    public void Equip()
    {
        // Heal
        Debug.Log("Bandage Used, player got +" + healthBonus + " health");
        Player.Instance.Inventory.DestroyItem(this);
        IsEquipped = true;
    }

    public override void OnInteract(Transform interactorTransform)
    {
        Player.Instance.Inventory.AddItem(this);
    }
}
