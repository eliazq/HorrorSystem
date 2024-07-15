using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    #region Events

    public event EventHandler<OnJumpEventArgs> OnJump;
    public event EventHandler OnLanding;
    public class OnJumpEventArgs : EventArgs
    {
        public bool jumpForward;
        public bool jumpUp;
    }
    #endregion

    #region SerializeField References

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform cameraRootTransform;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    #endregion

    #region Settings

    [Header("Settings Camera")]
    [SerializeField] private float mouseSensitivity = 300f;
    [SerializeField] private float lookMouseSensitivity = 300f;
    [SerializeField] private float aimMouseSensitivity = 100f;
    [SerializeField] private float maxLookUp = -69;
    [SerializeField] private float maxLookDown = 69;
    [SerializeField] private float runningFovAmount = 5f;
    [SerializeField] private float timeToRunningFov = 18f;
    float xRotation = 0;
    float walkingFov;
    float runningFov;

    [Header("Settings Movement")]
    [SerializeField] private float walkingSpeed = 2.5f;
    [SerializeField] private float runningSpeed = 4f;
    [SerializeField] private float jumpHeight = 1.6f;
    [SerializeField] private float groundCheckRadius = 0.4f;
    [SerializeField] private float crouchingSpeed = 3f;
    private float jumpCooldown = 1f;

    [Header("Settings Gravity")]
    [SerializeField] private float gravity = -9.81f;
    #endregion

    #region Variables

    Vector3 velocity;
    private float movementSpeed = 3f;
    Vector3 lastMoveDirection;
    float lastMovementSpeed;
    float jumpTimer;
    bool isWalking;
    #endregion

    #region Properties
    public Vector3 MoveDirection { get; private set; }
    public bool IsWalking { get { return isWalking && IsRunning == false; } }
    public bool IsRunning { get { return movementSpeed > walkingSpeed; } }
    public bool IsFalling
    {
        get
        {
            return IsGrounded == false && velocity.y < -3f;
        }
    }
    public bool IsLanding { get; private set; }
    public bool IsStanding { get { return velocity.x < 0.1 && velocity.z < 0.1f && IsGrounded; } }
    public bool IsTurningLeft { get; private set;}
    public bool IsTurningRight { get; private set;}
    public float Gravity { get { return gravity; } }
    public bool IsGrounded { get; private set; }
    #endregion

    private void Start()
    {
        mouseSensitivity = lookMouseSensitivity;
        // If camera not set, try get it from tag MainCamera
        if (playerCamera == null) playerCamera = Camera.main;
        if (cameraRootTransform == null) cameraRootTransform = playerCamera.transform.parent;

        // If characterController not set, try to get it from this GameObject
        if (characterController == null) characterController = GetComponent<CharacterController>();
        
        // If groundCheck not set, try find it in child
        if (groundCheck == null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.ToLower() == "groundcheck") groundCheck = transform.GetChild(i);
            }
        }
        walkingFov = playerCamera.fieldOfView;
        runningFov = walkingFov + runningFovAmount;
    }

    void Update()
    {
        AddGravity();

        CameraLook();

        Movement();
    }

    private void Movement()
    {
        // Inputs
        float x = InputManager.Instance.movementInput.x;
        float z = InputManager.Instance.movementInput.y;

        Vector3 moveDirection = (transform.right * x + transform.forward * z).normalized;

        MoveDirection = new Vector3(x, velocity.y, z);

        // Running
        if (InputManager.Instance.sprintPressed && z >= 0)
        {
            movementSpeed = runningSpeed;
        }
        else
        {
            movementSpeed = walkingSpeed;
        }

        // Move
        if (IsGrounded)
        {
            if (moveDirection != Vector3.zero) isWalking = true;
            else isWalking = false;
            IsLanding = false;
            lastMovementSpeed = movementSpeed;
            lastMoveDirection = moveDirection;
            characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
        }
        else
        {
            isWalking = false;
            characterController.Move(lastMoveDirection * lastMovementSpeed * Time.deltaTime);
            CheckLanding();
        }

        jumpTimer += Time.deltaTime;
        // Jump
        if (InputManager.Instance.jumpPressed && IsGrounded && jumpTimer >= jumpCooldown)
        {
            jumpTimer = 0;
            bool jumpWithMovement = lastMoveDirection != Vector3.zero;
            OnJump?.Invoke(this, new OnJumpEventArgs
            {
                jumpForward = jumpWithMovement,
                jumpUp = !jumpWithMovement
            });
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

    }

    private void CheckLanding()
    {
        if (Physics.SphereCast(transform.position, characterController.radius, -transform.up, out RaycastHit hit, characterController.height * 2))
        {
            IsLanding = true;
        }
    }

    private void AddGravity()
    {
        bool lastGroundedCheck = IsGrounded;

        IsGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (IsGrounded && lastGroundedCheck != IsGrounded) OnLanding?.Invoke(this,EventArgs.Empty);

        if (IsGrounded && velocity.y < 0)
        { 
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        characterController.Move(velocity * Time.deltaTime);
    }

    private void CameraLook()
    {
        // Sensitivity
        mouseSensitivity = Player.Instance.WeaponHandling.IsAiming ? aimMouseSensitivity : lookMouseSensitivity;
        // Inputs
        float mouseX = InputManager.Instance.lookInput.x * Time.deltaTime * mouseSensitivity;
        float mouseY = InputManager.Instance.lookInput.y * Time.deltaTime * mouseSensitivity;

        CheckTurning(mouseX);

        // Look Horizontal, Left and Right
        transform.Rotate(0, mouseX, 0f);

        // Look Up and Down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxLookUp, maxLookDown);

        cameraRootTransform.localRotation = Quaternion.Euler(xRotation, cameraRootTransform.localRotation.y, cameraRootTransform.localRotation.z);
        UpdateCameraFov();
    }

    private void CheckTurning(float mouseX)
    {
        if (IsGrounded)
        {
            IsTurningLeft = mouseX < -0.3f;
            IsTurningRight = mouseX > 0.3f;
        }
        else
        {
            IsTurningLeft = false;
            IsTurningRight = false;
        };
        
    }

    private void UpdateCameraFov()
    {
        runningFov = walkingFov + runningFovAmount;
        // if running or falling
        if (IsRunning)
        {
            playerCamera.fieldOfView = Mathf.MoveTowards(playerCamera.fieldOfView, runningFov, timeToRunningFov * Time.deltaTime);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.MoveTowards(playerCamera.fieldOfView, walkingFov, timeToRunningFov * Time.deltaTime);
        }
    }

}
