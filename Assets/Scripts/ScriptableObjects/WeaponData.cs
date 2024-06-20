using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponData/NewWeaponData", order = 0)]
public class WeaponData : ScriptableObject
{
    public float fireRate = 15f;
    public float  shootingDistance = 100f;
    public int maxMagSize = 30;
    public WeaponType weaponType;
    public enum WeaponType{
        Pistol,
        SubMachine
    }

}
