using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Item
{
    [SerializeField] WeaponData.WeaponType ammoType;
    public WeaponData.WeaponType Type { get { return ammoType; } }

    private void Start()
    {
        interactText = "Pick up " + Data.itemName;
    }
    public override void OnInteract(Transform interactorTransform)
    {
        Player.Instance.Inventory.AddItem(this);
    }
}
