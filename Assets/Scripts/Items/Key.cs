using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{
    private void Start()
    {
        interactText = $"Pick up {Data.itemName}";
    }
    public override void OnInteract(Transform interactorTransform)
    {
        Player.Instance.Inventory.AddItem(this);
    }
}
