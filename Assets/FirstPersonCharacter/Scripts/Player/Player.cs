using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public FirstPersonController FirstPersonController { get; private set; }
    public WeaponHandling WeaponHandling { get; private set; }
    public Weapon Weapon { get { return WeaponHandling.Weapon; } }

    [SerializeField] private Inventory inventory;
    public Inventory Inventory { get; private set; }

    public Transform flashLightHoldingTransform;

    private void Awake()
    {
        WeaponHandling = GetComponent<WeaponHandling>();
        FirstPersonController = GetComponent<FirstPersonController>();
        Inventory = inventory;
        
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

}
