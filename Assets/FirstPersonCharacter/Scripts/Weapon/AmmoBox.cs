using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour, IInteractable
{
    [SerializeField] private Ammo ammo;
    [SerializeField] private int ammoAmount = 16;

    private void Start()
    {
        ammo.Data.itemName = ammo.AmmoType.ToString() + " Ammo";
    }
    public string GetInteractText()
    {
        return "Pick up " + ammo.Data.itemName;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        for (int i = 0; i < ammoAmount; i++)
        {
            Item ammoItem = Instantiate(ammo);
            ammoItem.transform.position = Player.Instance.transform.position;
            Player.Instance.Inventory.AddItem(ammoItem);
        }
        Destroy(gameObject);
    }
}
