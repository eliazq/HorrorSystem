using UnityEngine;

public class Flashlight : Item, IEquippable, IUnEquippable
{
    public bool IsEquipped { get; set; }

    public void Equip()
    {
        IsEquipped = true;
        transform.SetParent(Player.Instance.flashLightHoldingTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
    }
    public void UnEquip()
    {
        IsEquipped = false;
    }
    public override void OnInteract(Transform interactorTransform)
    {
        Player.Instance.Inventory.AddItem(this);
    }
}
