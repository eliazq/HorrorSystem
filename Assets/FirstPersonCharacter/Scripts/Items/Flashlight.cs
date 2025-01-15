using UnityEngine;

public class Flashlight : Item, IEquippable, IUnEquippable
{
    public bool IsEquipped { get; set; }
    [SerializeField] private GameObject lightObject;

    public void Equip()
    {
        gameObject.SetActive(true);
        TurnLightOn();
        transform.SetParent(Player.Instance.flashLightHoldingTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        IsEquipped = true;
    }
    public void UnEquip()
    {
        TurnLightOff();
        IsEquipped = false;
    }
    public override void OnInteract(Transform interactorTransform)
    {
        Player.Instance.Inventory.AddItem(this);
    }

    private void TurnLightOn()
    {
        if (!lightObject.activeSelf) lightObject.SetActive(true);
    }

    private void TurnLightOff()
    {
        if (lightObject.activeSelf) lightObject.SetActive(false);
    }
}
