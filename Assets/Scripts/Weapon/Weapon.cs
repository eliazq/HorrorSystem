using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    [Header("Scriptable Object")]
    [SerializeField] private WeaponData weaponData;

    [Header("References")]
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform handlerTransform;

    private WeaponHandling weaponHandling;
    
    // Animation triggers
    const string shootTrigger = "Shoot";
    const string ReloadTrigger = "Reload";
    const string AimBool = "Aim";
    public int magSize{ get; private set; }

    public bool isReloading {get; private set;}
    public event EventHandler OnAiming;
    private Ammo realoadWeaponAmmo;

    public Transform ShootingPoint
    {
        get{
            return shootingPoint;
        }
        private set{
            shootingPoint = value;
        }
    }
    public WeaponData Data
    {
        get{
            return weaponData;
        }
        private set{
            weaponData = value;
        }
    }
    public bool IsPlayerWeapon {
        get{
            if (this == weaponHandling.Weapon) return true;
            return false;
        }
    }
    public Transform handlerGrip
    {
        get { return handlerTransform; }
    }

    private void Update() {
        if (weaponHandling == null || !IsPlayerWeapon) return;

        if (isReloading && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Reload")){
            // Just stopped reload animation
            int oldMagSize = magSize;
            magSize += realoadWeaponAmmo.Amount;
            if (magSize > Data.maxMagSize) magSize = Data.maxMagSize;
            for (int i = 0; i < weaponData.maxMagSize - oldMagSize; i++)
            {
                Player.Instance.Inventory.DestroyItem(realoadWeaponAmmo);
            }
            isReloading = false;
        }

        if (weaponHandling.IsAiming)
        {
            animator.SetBool(AimBool, true);
            OnAiming?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            animator.SetBool(AimBool, false);
        }
        
    }

    private void OnPlayerShoot_Action(object sender, EventArgs e){
        if (IsPlayerWeapon)
        {
            SoundManager.PlaySound(Data.shootingSound);
            animator.SetTrigger(shootTrigger);
            magSize -= 1;
        }
    }

    public void Reload(Ammo reloadingWeaponsAmmo){
        if (!isReloading)
        {
            realoadWeaponAmmo = reloadingWeaponsAmmo;
            animator.SetTrigger(ReloadTrigger);
            animator.SetTrigger(shootTrigger); // After reloading shoot trigger would be on if not called here. it makes shoot anim after realoading if not this 
            StartCoroutine(SetReload());
            SoundManager.PlaySound(Data.reloadSound, transform.position);
        }
    }

    IEnumerator SetReload(){
        yield return new WaitForEndOfFrame();
        isReloading = true;
    }

    // IINTERACTABLE INTERFACE
    public void Interact(Transform interactorTransform){

        if (weaponHandling == null)
        {
            weaponHandling = interactorTransform.GetComponent<WeaponHandling>();
            weaponHandling.OnShoot += OnPlayerShoot_Action;
        }

        weaponHandling.SetWeapon(this);
        StartCoroutine(SetWeaponFix(weaponHandling));
    }

    // Setting the IK Target twice fix its bug where it sets it wrong
    private IEnumerator SetWeaponFix(WeaponHandling weaponHandling)
    {
        yield return new WaitForSeconds(0.05f);
        weaponHandling.SetWeapon(this);
    }
    public string GetInteractText(){
        return "Pick Up " + name;
    }
    public Transform GetTransform(){
        return transform;
    }

}
