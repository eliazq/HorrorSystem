using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    private static InputManager _instance;
    private static readonly object _lock = new object();
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        InputManager prefab = Resources.Load<InputManager>("InputManager");

                        if (prefab == null)
                        {
                            Debug.LogError("InputManager prefab not found in Resources!");
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

    public event EventHandler OnUsingDeviceChanged;

    public event EventHandler OnInteractionHolded;
    public event EventHandler OnInteractionClicked;
    public event EventHandler OnReloadHolded;
    public event EventHandler OnWeaponDropClicked;
    public event EventHandler OnInventoryToggle;
    public event EventHandler OnAcceptClicked;
    public event EventHandler OnCancelClicked;
    public event EventHandler<OnArrowClickedEventArgs> OnArrowClicked;
    public class OnArrowClickedEventArgs : EventArgs
    {
        public ArrowKey arrowKey;
    }
    public enum ArrowKey
    {
        Left,
        Right,
        Up,
        Down,
        None,
    }
    public Vector2 movementInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public Vector2 arrowInput { get; private set; }
    public bool sprintPressed { get { return playerInputActions.Movement.Sprint.IsPressed() && PlayerMovementInputsActive; } }
    public bool jumpPressed { get { return playerInputActions.Movement.Jump.IsPressed() && PlayerMovementInputsActive; } }
    public bool aimPressed { get { return playerInputActions.WeaponHandling.Aim.IsPressed() && WeaponHandlingInputsActive; } }
    public bool shootPressed { get { return playerInputActions.WeaponHandling.Shoot.IsPressed() && WeaponHandlingInputsActive; } }

    private float interactHoldTime = 0.35f;
    private float reloadHoldTime = 0.2f;

    public bool WeaponHandlingInputsActive { get; set; } = true;
    public bool PlayerMovementInputsActive { get; set; } = true;
    public bool InventoryInputsActive { get; set; } = true;
    public static bool isUsingController
    {
        get {
            if (usingDevice == null) return false;
            bool usingKeyboardMouse = usingDevice.description.deviceClass.Equals("Keyboard") || usingDevice.description.deviceClass.Equals("Mouse");
            return !usingKeyboardMouse;
        }
    }
    public static bool isUsingKeyboardOrMouse { get { return !isUsingController; } }
    private static InputDevice usingDevice;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (playerInputActions == null)
        {
            playerInputActions = new PlayerInputActions();

            playerInputActions.Movement.Move.performed += i => movementInput = i.ReadValue<Vector2>();

            playerInputActions.Movement.Look.performed += i => lookInput = i.ReadValue<Vector2>();

            playerInputActions.FindAction("Move").performed += InputManager_MovePerformed;
            playerInputActions.FindAction("Look").performed += InputManager_performed;

            playerInputActions.UI.Arrows.started += Arrows_started;

            playerInputActions.Interaction.Interact.started += Interact_started;

            playerInputActions.WeaponHandling.Reload.started += Reload_started;

            playerInputActions.WeaponHandling.Drop.started += Drop_started;

            playerInputActions.Inventory.ToggleInventory.started += ToggleInventory_started;

            playerInputActions.UI.Accept.started += Accept_started;

            playerInputActions.UI.Delete.started += Delete_started;

        }
        playerInputActions.Enable();
    }

    private void InputManager_performed(InputAction.CallbackContext obj)
    {
        usingDevice = obj.action.activeControl.device;
    }

    private void InputManager_MovePerformed(InputAction.CallbackContext obj)
    {
        usingDevice = obj.action.activeControl.device;
    }

    private void Delete_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!InventoryInputsActive) return;
        OnCancelClicked?.Invoke(this, EventArgs.Empty);
        usingDevice = obj.action.activeControl.device; 
    }

    private void Arrows_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!InventoryInputsActive) return;
        arrowInput = obj.ReadValue<Vector2>();
        ArrowKey clickedArrowKey = ArrowKey.None;
        switch (arrowInput.x)
        {
            case > 0:
                clickedArrowKey = ArrowKey.Right;
                break;
            case < 0:
                clickedArrowKey = ArrowKey.Left;
                break;
        }
        switch (arrowInput.y)
        {
            case > 0:
                clickedArrowKey = ArrowKey.Up;
                break;
            case < 0:
                clickedArrowKey = ArrowKey.Down;
                break;
        }
        OnArrowClicked?.Invoke(this, new OnArrowClickedEventArgs {arrowKey = clickedArrowKey});
        usingDevice = obj.action.activeControl.device; 
    }

    private void Accept_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!InventoryInputsActive) return;
        OnAcceptClicked?.Invoke(this, EventArgs.Empty);
        usingDevice = obj.action.activeControl.device;
    }

    private void ToggleInventory_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!InventoryInputsActive) return;
        OnInventoryToggle?.Invoke(this, EventArgs.Empty);
        usingDevice = obj.action.activeControl.device;
    }

    private void Drop_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!WeaponHandlingInputsActive) return;
        OnWeaponDropClicked?.Invoke(this, EventArgs.Empty);
        usingDevice = obj.action.activeControl.device;
    }

    private void Reload_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!WeaponHandlingInputsActive) return;
        StartCoroutine(ReloadCheckHold());
        usingDevice = obj.action.activeControl.device;
    }

    private void Interact_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractionClicked?.Invoke(this, EventArgs.Empty);
        StartCoroutine(InteractCheckHold());
        usingDevice = obj.action.activeControl.device;
    }

    IEnumerator InteractCheckHold()
    {
        float timer = 0;
        bool interactHolded = true;
        while (timer < interactHoldTime)
        {
            timer += Time.deltaTime;
            if (!playerInputActions.Interaction.Interact.IsPressed())
            {
                interactHolded = false;
                break;
            }
            yield return null;
        }
        if (interactHolded) OnInteractionHolded?.Invoke(this, EventArgs.Empty);
    }

    IEnumerator ReloadCheckHold()
    {
        float timer = 0;
        bool reloadHolded = true;
        while (timer < reloadHoldTime)
        {
            timer += Time.deltaTime;
            if (!playerInputActions.WeaponHandling.Reload.IsPressed())
            {
                reloadHolded = false;
                break;
            }
            yield return null;
        }
        if (reloadHolded) OnReloadHolded?.Invoke(this, EventArgs.Empty);
    }
}
