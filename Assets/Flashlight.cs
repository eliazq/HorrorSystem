using UnityEngine;

public class Flashlight : Item, IEquippable, IUnEquippable
{
    public bool IsEquipped { get; set; }

    public void Equip()
    {
        Debug.Log("Flashlight Equipped"); //   JOS PAINAA EQUIP JA DROPPAAN NII TEE ETTÄ UNEQUIPPAA SAMALLA KU TIPUTTAA 
        IsEquipped = true;
    }
    public void UnEquip()
    {
        Debug.Log("Flashlight UnEquipped");
        IsEquipped = false;
    }
    public override void OnInteract(Transform interactorTransform)
    {
        Player.Instance.Inventory.AddItem(this);
    }
}
