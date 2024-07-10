using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandHeadBob : MonoBehaviour
{
    [SerializeField] private float handBobStopTime = 3f;
    [Range(0.001f, 0.1f)]
    [SerializeField] private float idleAmount = 0.004f;
    [Range(1f, 30f)]
    [SerializeField] private float idleFrequency = 1.0f;
    [Range(0.001f, 0.1f)]
    [SerializeField] private float aimingAmount = 0.001f;
    [Range(1f, 30f)]
    [SerializeField] private float aimingFrequency = 2.9f;
    [Range(0.001f, 0.1f)]
    [SerializeField] private float walkingAmount = 0.035f; 
    [Range(1f, 30f)]
    [SerializeField] private float walkingFrequency = 10.0f;
    [Range(0.001f, 0.1f)]
    [SerializeField] private float runningAmount = 0.025f; 

    [Range(1f, 30f)]
    [SerializeField] private float runningFrequency = 19.6f; 

    private float Amount = 0; 

    private float Frequency = 0; 

    private float Smooth = 0;

    [Range(0f, 100f)]
    [SerializeField] private float walkSmooth = 10.0f; 

    [Range(0f, 100f)]
    [SerializeField] private float runningSmooth = 10.0f;

    [Range(0, 100f)]
    [SerializeField] private float idleSmooth = 10.0f;

    [Range(0, 100f)]
    [SerializeField] private float aimingSmooth = 10.0f;

    [SerializeField] private float LeanAmount = 1.8f;
    [SerializeField] private float LeanTime = 3f;

    Vector3 StartPos;
    Quaternion StartRot;
    [SerializeField] FirstPersonController firstPersonController;
    bool isWalkingSettings = true;
    bool setFirstTime = true;

    void Start()
    {
        StartPos = transform.localPosition;
        StartRot = transform.localRotation;

        if (firstPersonController == null) 
            firstPersonController = GetComponentInParent<FirstPersonController>();
        SetWalkingHeadBobSettings();
    }

    void Update()
    {
        if (firstPersonController.IsWalking || firstPersonController.IsRunning || firstPersonController.IsFalling)
        {
            CheckForHeadbobTrigger();
            StopHeadbob();
            CameraLean();
        }
        else if (!Player.Instance.WeaponHandling.IsAiming)
        {
            CheckForHeadbobTrigger();
            StopHeadbob();
            CameraLean();
        }
        else
        {
            CheckForHeadbobTrigger();
            StopHeadbob();
            CameraLean();
        }
    }

    private void CameraLean()
    {
        Vector3 moveDirection = firstPersonController.MoveDirection;
        Quaternion targetRotation = Quaternion.identity;

        if (moveDirection.x > 0.1f)
        {
            // Lean right -z
            targetRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, -LeanAmount);
        }
        else if (moveDirection.x < -0.1f)
        {
            // Lean left = +z
            targetRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, LeanAmount);
        }
        else if (moveDirection.z > 0.1f)
        {
            // Lean forward +x
            targetRotation = Quaternion.Euler(LeanAmount, transform.localEulerAngles.y, targetRotation.z);
        }
        else if (moveDirection.z < -0.1f)
        {
            // Lean backwards -x
            targetRotation = Quaternion.Euler(-LeanAmount, transform.localEulerAngles.y, targetRotation.z);
        }
        else
        {
            targetRotation = StartRot;
        }
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, LeanTime * Time.deltaTime);
    }

    private void CheckForHeadbobTrigger()
    {
        if (firstPersonController.IsWalking)
        {
            if (!isWalkingSettings) SetWalkingHeadBobSettings();
            StartHeadBob();
        }
        else if (firstPersonController.IsRunning)
        {
            if (isWalkingSettings) SetRunningHeadBobSettings();
            StartHeadBob();
        }
        else if (!Player.Instance.WeaponHandling.IsAiming)
        {
            SetIdleHeadBobSettings();
            StartHeadBob();
        }
        else
        {
            SetAimingHeadBobSettings();
            StartHeadBob();
        }
    }

    private void SetWalkingHeadBobSettings()
    {
        Amount = walkingAmount;
        Frequency = walkingFrequency;
        Smooth = walkSmooth;
        isWalkingSettings = true;
    }

    private void SetRunningHeadBobSettings()
    {
        Amount = runningAmount;
        Frequency = runningFrequency;
        Smooth = runningSmooth;
        isWalkingSettings = false;
    }

    private void SetIdleHeadBobSettings()
    {
        Amount = idleAmount;
        Frequency = idleFrequency;
        Smooth = idleSmooth;
        isWalkingSettings = false;
    }
    private void SetAimingHeadBobSettings()
    {
        Amount = aimingAmount;
        Frequency = aimingFrequency;
        Smooth = aimingSmooth;
        isWalkingSettings = false;
    }

    private Vector3 StartHeadBob()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * Frequency) * Amount * 1.4f, Smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * Frequency / 2f) * Amount * 1.6f, Smooth * Time.deltaTime);
        transform.localPosition += pos;
        return pos;
    }

    private void StopHeadbob()
    {
        if (transform.localPosition == StartPos) return;
        transform.localPosition = Vector3.Slerp(transform.localPosition, StartPos, handBobStopTime * Time.deltaTime);
    }

}
