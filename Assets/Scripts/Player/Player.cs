using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public WeaponHandling WeaponHandling { get; private set; }

    private void Start()
    {
        WeaponHandling = GetComponent<WeaponHandling>();
    }

}
