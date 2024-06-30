using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public WeaponHandling WeaponHandling { get; private set; }
    public Weapon Weapon { get { return WeaponHandling.Weapon; } }

    private void Start()
    {
        WeaponHandling = GetComponent<WeaponHandling>();
    }

}
