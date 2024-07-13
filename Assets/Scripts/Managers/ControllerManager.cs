using System;
using System.Collections;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public static event EventHandler OnControllerDeviceChanged;
    bool isKeyboardOrMouse;

    private void Start()
    {
        isKeyboardOrMouse = InputManager.isUsingKeyboardOrMouse;
    }
    private void Update()
    {
        if (isKeyboardOrMouse != InputManager.isUsingKeyboardOrMouse)
        {
            isKeyboardOrMouse = InputManager.isUsingKeyboardOrMouse;
            OnControllerDeviceChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
