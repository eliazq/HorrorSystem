using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    [Header("Data")]
    [SerializeField] private WeaponData weaponData;

    [Header("References")]
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform handlerTransform;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootingClip;

    private WeaponHandling weaponHandling;
    
    // Animation triggers
    const string shootTrigger = "Shoot";
    const string ReloadTrigger = "Reload";
    const string OutOfAmmoBool = "OutOfAmmo";
    const string AimBool = "Aim";
    public int magSize{ get; private set; }

    public bool isReloading {get; private set;}
    public event EventHandler OnAiming;
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
            magSize = Data.maxMagSize;
            isReloading = false;
        }

        if (!isReloading){
            if (magSize <= 0)
                animator.SetBool(OutOfAmmoBool, true);
            else animator.SetBool(OutOfAmmoBool, false);
        }

        if (Input.GetKey(KeyCode.Mouse1))
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
            audioSource.clip = shootingClip;
            audioSource.PlayOneShot(shootingClip);
            animator.SetTrigger(shootTrigger);
            magSize -= 1;
        }
    }

    public void Reload(){
        if (!isReloading)
        {
            animator.SetTrigger(ReloadTrigger);
            StartCoroutine(SetReload());
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
        return "PickUpWeapon";
    }
    public Transform GetTransform(){
        return transform;
    }

}
