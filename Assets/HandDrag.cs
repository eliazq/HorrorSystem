using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDrag : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth = 8f;
    [SerializeField] private float swayMultiplier = 2f;

    void Update()
    {
        float mouseX = InputManager.Instance.lookInput.x * swayMultiplier;
        float mouseY = InputManager.Instance.lookInput.y * swayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
