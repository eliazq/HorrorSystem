using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{
    private void Start()
    {
        interactText = $"Pick up {itemData.itemName}";
    }
    public override void OnInteract(Transform interactorTransform)
    {
        Debug.Log(interactorTransform);
    }
}
