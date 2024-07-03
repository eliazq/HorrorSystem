using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public WeaponHandling WeaponHandling { get; private set; }
    public Weapon Weapon { get { return WeaponHandling.Weapon; } }

    [SerializeField] private Inventory inventory;
    public Inventory Inventory { get; private set; }

    private void Awake()
    {
        WeaponHandling = GetComponent<WeaponHandling>();
        Inventory = inventory;
    }

}
