using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponHandling : MonoBehaviour
{
    public Weapon Weapon {get; private set;}

    public event EventHandler OnBodyHit;

    [Header("Settings")]
    [SerializeField] private Transform handTransform;
    [SerializeField] private float WeaponThrowForce = 300f;
    [SerializeField] private float ShotImpactForce = 200f;

    [Header("Hand IK")]
    [SerializeField] private Animator handAnimatorIK;
    [SerializeField] private AvatarMask avatarMask;
    [SerializeField] private TwoBoneIKConstraint twoBoneIKConstraint;
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private Transform emptyHandTransform;

    public int pistolMags {get; set;} = 0;
    public int subMachineMags {get; set;} = 0;
    public event EventHandler OnShoot;
    private float shootingCooldown;
    private float dropWeaponCooldown;
    private float weaponThrowRate = 1f;
    public bool HasWeapon {
        get {
            return Weapon != null;
        }
    }
    public bool IsAiming
    {
        get
        {
            return InputManager.Instance.aimPressed;
        }
    }
    private void Start()
    {
        InputManager.Instance.OnReloadHolded += Instance_OnReloadHolded;
        InputManager.Instance.OnWeaponDropClicked += Instance_OnWeaponDropClicked;
    }

    private void Instance_OnWeaponDropClicked(object sender, EventArgs e)
    {
        if (HasWeapon && Time.time >= dropWeaponCooldown && !Weapon.isReloading)
        {
            dropWeaponCooldown = Time.time + 1f/weaponThrowRate;
            DropWeapon(); 
        }
    }

    private void Instance_OnReloadHolded(object sender, EventArgs e)
    {
        if (HasWeapon)
        {
            ReloadWithRightMag();
        }
    }

    private void Update() {
        HandleWeapon();
    }

    private void HandleWeapon(){
        // Has weapon
        if (HasWeapon)
        {
            if (IsRightHandAvatarMaskActive())
            {
                SetRightHandAvatarMaskActive(false);
            }
            handAnimatorIK.enabled = true;

            CheckShooting();

        }
        else if (!IsRightHandAvatarMaskActive())
        {
            SetRightHandAvatarMaskActive(true);
            handAnimatorIK.enabled = false;
        }
    }

    private void CheckShooting(){
        // Check if player shoots and if its possible
        if (InputManager.Instance.shootPressed && Time.time >= shootingCooldown && Weapon.magSize > 0 && !Weapon.isReloading){
            shootingCooldown = Time.time + 1f/Weapon.Data.fireRate;
            OnShoot?.Invoke(this, EventArgs.Empty);

            if (WeaponSystem.Instance.Shoot(Camera.main.transform.position, Weapon.ShootingPoint.forward,
                transform.position, Weapon.ShootingPoint.position, Weapon.Data.shootingDistance, ShotImpactForce, out RaycastHit hit))
            {
                // Damage Logic Here, IDamageable.Damage !!!
                OnBodyHit?.Invoke(this, EventArgs.Empty);
            }
            
        }
    }

    private void ReloadWithRightMag(){
        if (!Player.Instance.Inventory.TryGetItem(Weapon.Data.weaponType.ToString() + " Ammo", out Item ammo)) return;
        
        if (ammo.Amount > 0 && Weapon.magSize < Weapon.Data.maxMagSize){
            Weapon.Reload(ammo.GetComponent<Ammo>());
        }
    }

    public void SetWeapon(Weapon weapon){
        if (HasWeapon){
            if (!Weapon.isReloading) {
                Weapon.transform.parent = null;
                Weapon.AddComponent<Rigidbody>();
                if (weapon.GetComponent<Rigidbody>() != null) Destroy(weapon.GetComponent<Rigidbody>());
                Weapon = weapon;

                StartCoroutine(MoveWeaponDynamically(5f));
                StartCoroutine(RotateWeaponDynamically(5f));

                Weapon.transform.parent = handTransform;
            }
        }
        else {
            if (weapon.GetComponent<Rigidbody>() != null) Destroy(weapon.GetComponent<Rigidbody>());
            Weapon = weapon;

            StartCoroutine(MoveWeaponDynamically(5f));
            StartCoroutine(RotateWeaponDynamically(5f));

            Weapon.transform.parent = handTransform;
        }

        SetHandIKTarget(Weapon.handlerGrip);
        StartCoroutine(RightHandIKResetAnimator());

    }

    IEnumerator RightHandIKResetAnimator()
    {
        handAnimatorIK.enabled = false;
        yield return new WaitForSeconds(0.1f);
        handAnimatorIK.enabled = true;
    }

    private void DropWeapon(){
        WeaponSystem.DropWeapon(Weapon, Camera.main.transform.forward, WeaponThrowForce);
        Weapon = null;
        SetHandIKTarget(null);
        StartCoroutine(RightHandIKResetAnimator());
    }

    private void SetHandIKTarget(Transform target)
    {
        twoBoneIKConstraint.data.target = target;
        rigBuilder.Build();
    }

    private IEnumerator MoveWeaponDynamically(float smoothness){
        float elapsedTime = 0f;

        while (elapsedTime < smoothness){
            Vector3 targetPosition = handTransform.position;
            Weapon.transform.position = Vector3.Lerp(Weapon.transform.position, targetPosition, elapsedTime / smoothness);
            float positionThreshold = 0.01f;
            float distance = Vector3.Distance(Weapon.transform.position, targetPosition);
            if (distance < positionThreshold){
                Weapon.transform.position = targetPosition;
                break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RotateWeaponDynamically(float smoothness){
        float elapsedTime = 0f;

        while (elapsedTime < smoothness){
            Quaternion targetRotation = handTransform.rotation;
            Weapon.transform.rotation = Quaternion.Slerp(Weapon.transform.rotation, targetRotation, elapsedTime / smoothness);
            float rotationThreshold = 0.01f;
            float angle = Quaternion.Angle(Weapon.transform.rotation, targetRotation);
            if (angle < rotationThreshold){
                Weapon.transform.rotation = targetRotation;
                break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void SetRightHandAvatarMaskActive(bool active)
    {
        if (active)
        {
            avatarMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightArm, true);
            avatarMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightHandIK, true);
            avatarMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightFingers, true);
        }
        else
        {
            avatarMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightArm, false);
            avatarMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightHandIK, false);
            avatarMask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightFingers, false);
        }
    }

    private bool IsRightHandAvatarMaskActive()
    {
        return avatarMask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.RightArm);
    }


}
