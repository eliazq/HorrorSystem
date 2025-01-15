using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    private static WeaponSystem _instance;

    #region Make Instance When Called
    private static readonly object _lock = new object();
    public static WeaponSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        WeaponSystem prefab = Resources.Load<WeaponSystem>("WeaponSystem");

                        if (prefab == null)
                        {
                            Debug.LogError("WeaponSystem prefab with WeaponSystem Component not found in Resources Folder!"); // Assets/Resourses/WeaponSystem
                            Debug.Log("Assets/Resourses/WeaponSystem");
                        }
                        else
                        {
                            _instance = Instantiate(prefab);
                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                }
            }
            return _instance;
        }
    }
    #endregion

    #region Data References
    [Header("Shooting Data")]

    [Header("Bullet Impact")]
    [SerializeField] LayerImpactList layerImpactList;
    [SerializeField] float impactDestroyTime = 50f;

    [Header("Muzzle Flash")]
    [SerializeField] GameObject muzzleFlashPrefab;
    [SerializeField] float muzzleFlashDestroyTime;

    [Header("Bullet Trail")]
    [SerializeField] GameObject trailPrefab;
    [SerializeField] float trailTime = 0.15f;

    #endregion
    
    private void Awake() {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
    public static void DropWeapon(Weapon weapon){
        if (weapon != null){
            weapon.transform.parent = null;
            weapon.AddComponent<Rigidbody>();
        }
    }
    public static void DropWeapon(Weapon weapon, Vector3 throwDirection, float throwForce){
        if (weapon != null){
            weapon.transform.parent = null;
            weapon.AddComponent<Rigidbody>().AddForce(throwDirection * throwForce);
        }
    }

    public bool Shoot(Vector3 shootingStartPostition, Vector3 shootDirection, Vector3 ShooterPosition, Vector3 weaponShootingPoint, float shotDistance, float shotImpactForce, out RaycastHit hit){

        SpawnMuzzleFlash(muzzleFlashPrefab, weaponShootingPoint, Quaternion.LookRotation(shootDirection, Vector3.up), muzzleFlashDestroyTime);

        if (Physics.Raycast(shootingStartPostition, shootDirection, out RaycastHit hitInfo, shotDistance))
        {
            hit = hitInfo;
            foreach (LayerImpactPair pair in layerImpactList.layerImpactPairs)
            {
                if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(pair.layer)))
                {
                    SpawnBulletImpact(pair.impactEffect, hit.point, Quaternion.LookRotation(hit.normal), impactDestroyTime);
                }
            }
            ShootBulletTrail(trailPrefab, weaponShootingPoint, hitInfo.point, trailTime);
            if (hitInfo.transform.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(-hitInfo.normal * shotImpactForce);
                Vector3 ForceDir = (hitInfo.transform.position - ShooterPosition).normalized;
                rigidbody.AddForce(ForceDir * shotImpactForce);
            }
            return true;
        }
        else {
            // If didnt hit anything, still shoot the trail
            Vector3 targetPos = weaponShootingPoint + shootDirection * 100f;
            ShootBulletTrail(trailPrefab, weaponShootingPoint, targetPos, trailTime);
            hit = new RaycastHit();
            return false;
        }
    }
    
    public void SpawnMuzzleFlash(GameObject muzzleFlashParticle, Vector3 position, Quaternion rotation, float destroyTime)
    {
        GameObject muzzleFlash = Instantiate(muzzleFlashParticle, position, rotation);
        Destroy(muzzleFlash, destroyTime);
    }
    public void SpawnBulletImpact(GameObject impactEffect, Vector3 position, Quaternion rotation, float impactDestroyTime){
        GameObject impact = Instantiate(impactEffect, position, rotation);
        Destroy(impact, impactDestroyTime);
    }

    public void ShootBulletTrail(GameObject bulletTrailPrefab, Vector3 startPosition, Vector3 endPosition, float trailFlyTime){
        GameObject trail = Instantiate(bulletTrailPrefab, startPosition, Quaternion.identity);
        MoveTrailSmooth(trail, endPosition, trailFlyTime);
    }

    private void MoveTrailSmooth(GameObject trail, Vector3 targetPos, float trailTime) {
    // You can adjust the duration based on how fast you want the trail to move
    float duration = trailTime;

    // Store the initial position of the trail
    Vector3 initialPos = trail.transform.position;

    // Use a coroutine to smoothly move the trail over time
    StartCoroutine(MoveTrailCoroutine(trail, initialPos, targetPos, duration));
    }

    private IEnumerator MoveTrailCoroutine(GameObject trail, Vector3 initialPos, Vector3 targetPos, float duration) {
        float elapsed = 0f;

        while (elapsed < duration) {
            // Interpolate the position between initialPos and targetPos over time
            trail.transform.position = Vector3.Lerp(initialPos, targetPos, elapsed / duration);

            // Increment the elapsed time
            elapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the trail reaches the exact target position
        trail.transform.position = targetPos;
        Destroy(trail);
    }
    

}
