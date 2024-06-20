using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    [Range(0.001f, 0.02f)]
    [SerializeField] private float Amount = 0.002f; // Walk: 0.01 / Running: 0.02

    [Range(1f, 30f)]
    [SerializeField] private float Frequency = 10.0f; // Walk: 10 / Running: 20

    [Range(10f, 100f)]
    [SerializeField] private float Smooth = 10.0f; // Walk: 15 / Running: 20

    [SerializeField] private float LeanAmount = 1.8f;
    [SerializeField] private float LeanTime = 3f;

    Vector3 StartPos;
    [SerializeField] FirstPersonController firstPersonController;
    bool isWalkingSettings = true;

    void Start()
    {
        StartPos = transform.localPosition;

        if (firstPersonController == null) 
            firstPersonController = GetComponentInParent<FirstPersonController>();
    }

    void Update()
    {
        CheckForHeadbobTrigger();
        StopHeadbob();
        CameraLean();
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

        if (moveDirection.z > 0.1f)
        {
            // Lean forward +x
            targetRotation = Quaternion.Euler(LeanAmount, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
        else if (moveDirection.z < -0.1f)
        {
            // Lean backwards -x
            targetRotation = Quaternion.Euler(-LeanAmount, transform.localEulerAngles.y, transform.localEulerAngles.z);
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
        Amount = 0.01f;
        Frequency = 10f;
        Smooth = 15f;
        isWalkingSettings = true;
    }

    private void SetRunningHeadBobSettings()
    {
        Amount = 0.02f;
        Frequency = 20f;
        Smooth = 20f;
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
        transform.localPosition = Vector3.Lerp(transform.localPosition, StartPos, 1 * Time.deltaTime);
    }
}
