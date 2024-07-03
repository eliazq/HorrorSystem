using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{
    private void Start()
    {
        interactText = $"Pick up {Data.name}";
    }
    public override void OnInteract(Transform interactorTransform)
    {
        interactorTransform.GetComponent<Inventory>().AddItem(this);
    }
}
