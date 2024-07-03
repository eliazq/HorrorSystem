using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour, IInteractable
{
    [SerializeField] internal ItemSO Data;
    private int amount = 1;
    public int Amount { get { return amount; } set { amount = value; } }
    [SerializeField] internal string interactText;
   
    public string GetInteractText()
    {
        return interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        OnInteract(interactorTransform);
    }

    public abstract void OnInteract(Transform interactorTransform);
    
}
