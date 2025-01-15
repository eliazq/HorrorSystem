using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HideCursor : MonoBehaviour
{

    [SerializeField] private bool hideCursor = true;
    [SerializeField] private bool lockCursor = false;

    private void Update()
    {
        if (hideCursor && Cursor.visible) Cursor.visible = false;
        if (lockCursor && Cursor.lockState == CursorLockMode.None) Cursor.lockState = CursorLockMode.Locked;
        else if (!lockCursor && Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
    }
}
