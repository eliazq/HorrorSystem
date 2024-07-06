using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public event EventHandler OnInteractionHolded;
    public event EventHandler OnInteractionClicked;
    public event EventHandler OnReloadHolded;
    public event EventHandler OnWeaponDropClicked;
    public event EventHandler OnInventoryToggle;
    public event EventHandler OnAcceptClicked;
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
    public bool sprintPressed { get { return playerInputActions.Movement.Sprint.IsPressed(); } }
    public bool jumpPressed { get { return playerInputActions.Movement.Jump.IsPressed(); } }
    public bool aimPressed { get { return playerInputActions.WeaponHandling.Aim.IsPressed(); } }
    public bool shootPressed { get { return playerInputActions.WeaponHandling.Shoot.IsPressed(); } }

    private float interactHoldTime = 0.35f;
    private float reloadHoldTime = 0.2f;

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

            playerInputActions.UI.Arrows.started += Arrows_started;

            playerInputActions.Interaction.Interact.started += Interact_started;

            playerInputActions.WeaponHandling.Reload.started += Reload_started;

            playerInputActions.WeaponHandling.Drop.started += Drop_started;

            playerInputActions.Inventory.ToggleInventory.started += ToggleInventory_started;

            playerInputActions.UI.Accept.started += Accept_started;

        }
        playerInputActions.Enable();
    }

    private void Arrows_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
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
            
    }

    private void Accept_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAcceptClicked?.Invoke(this, EventArgs.Empty);
    }

    private void ToggleInventory_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInventoryToggle?.Invoke(this, EventArgs.Empty);
    }

    private void Drop_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnWeaponDropClicked?.Invoke(this, EventArgs.Empty);
    }

    private void Reload_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(ReloadCheckHold());
    }

    private void Interact_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractionClicked?.Invoke(this, EventArgs.Empty);
        StartCoroutine(InteractCheckHold());
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
