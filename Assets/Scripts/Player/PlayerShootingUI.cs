using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShootingUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject hitMarkUI;
    [SerializeField] private Image crosshair;
    WeaponHandling weaponHandling;
    [Header("Settings")]
    [SerializeField] private float CrosshairFadeTime = 0.3f;
    [SerializeField] private float HitmarkShowingTime = 1;
    bool isColorChanging = false;
    bool aiming = true;
    private void Start()
    {
        weaponHandling = GetComponent<WeaponHandling>();
        weaponHandling.OnBodyHit += WeaponHandling_OnBodyHit;

        hitMarkUI.SetActive(false);
    }

    private void Update()
    {
        if (weaponHandling.HasWeapon) {
            weaponHandling.Weapon.OnAiming -= Weapon_OnAiming;
            weaponHandling.Weapon.OnAiming += Weapon_OnAiming;
        }
    }

    private void Weapon_OnAiming(object sender, System.EventArgs e)
    {
        if (!isColorChanging)
            StartCoroutine(FadeCrosshair(CrosshairFadeTime));
    }

    private IEnumerator FadeCrosshair(float time)
    {
        isColorChanging = true;
        float a = time;

        while (true)
        {
            aiming = Input.GetMouseButton(1);
            a -= Time.deltaTime;
            if (a < 0) a = 0;
            crosshair.color = new Color(crosshair.color.r,crosshair.color.g,crosshair.color.b, a);
            if (!aiming)
            {
                crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, 1f);
                break;
            }
            yield return null;
        }
        isColorChanging = false;
        
    }

    private void WeaponHandling_OnBodyHit(object sender, System.EventArgs e)
    {
        if (hitMarkUI.activeSelf == false)
            StartCoroutine(HitmarkUIActivation());
    }

    private IEnumerator HitmarkUIActivation()
    {
        hitMarkUI.SetActive(true);
        yield return new WaitForSeconds(HitmarkShowingTime);
        hitMarkUI.SetActive(false);
    }
}
