using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandHeadBob : MonoBehaviour
{
    [SerializeField] private float handBobStopTime = 3f;
    [Range(0.001f, 0.1f)]
    [SerializeField] private float walkingAmount = 0.002f; 
    [Range(1f, 30f)]
    [SerializeField] private float walkingFrequency = 10.0f; 
    [Range(0.001f, 0.1f)]
    [SerializeField] private float runningAmount = 0.002f; 

    [Range(1f, 30f)]
    [SerializeField] private float runningFrequency = 10.0f; 

    private float Amount = 0; 

    private float Frequency = 0; 

    private float Smooth = 0;

    [Range(0f, 100f)]
    [SerializeField] private float walkSmooth = 10.0f; 

    [Range(0f, 100f)]
    [SerializeField] private float runningSmooth = 10.0f; 

    [SerializeField] private float LeanAmount = 1.8f;
    [SerializeField] private float LeanTime = 3f;

    Vector3 StartPos;
    Quaternion StartRot;
    [SerializeField] FirstPersonController firstPersonController;
    bool isWalkingSettings = true;

    [Header("Breathing In Aiming")]
    [SerializeField] private float breathingMovementRadius = 0.01f; // Radius of the circle
    [SerializeField] private float breathingMovementSpeed = 2f; // Speed of movement

    private Vector3 breathingMovementCenter = Vector3.zero; // Center of the circle
    private Vector3 breathingMovementTargetPosition;

    bool SetFirstTime = true;

    void Start()
    {
        StartPos = transform.localPosition;
        StartRot = transform.localRotation;

        if (firstPersonController == null) 
            firstPersonController = GetComponentInParent<FirstPersonController>();
        breathingMovementCenter = StartPos;
        GetNewBreathingTargetPosition();
        SetWalkingHeadBobSettings();
    }

    void Update()
    {
        if (firstPersonController.IsWalking || firstPersonController.IsRunning || firstPersonController.IsFalling)
        {
            CheckForHeadbobTrigger();
            StopHeadbob();
            CameraLean();
            SetFirstTime = true;
        }
        else
        {
            BreathingHeadBob();
            CameraLean();
        }
    }

    private void BreathingHeadBob()
    {
        if (Player.Instance.WeaponHandling.IsAiming)
        {
            MoveTowardsBreathingTarget();
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

    void GetNewBreathingTargetPosition()
    {
        SetFirstTime = false;
        breathingMovementTargetPosition = new Vector3(StartPos.x + Random.Range(-breathingMovementRadius, breathingMovementRadius),
            StartPos.y + Random.Range(-breathingMovementRadius, breathingMovementRadius), transform.localPosition.z);
    }

    void MoveTowardsBreathingTarget()
    {
        if (SetFirstTime) transform.localPosition = breathingMovementTargetPosition;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, breathingMovementTargetPosition, breathingMovementSpeed * Time.deltaTime);

        // Check if the object has reached the target position
        if (Vector3.Distance(transform.localPosition, breathingMovementTargetPosition) <= 0.001f)
        {
            GetNewBreathingTargetPosition();
        }
    }

}
