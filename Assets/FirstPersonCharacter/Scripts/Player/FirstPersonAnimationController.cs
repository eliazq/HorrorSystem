using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonAnimationController : MonoBehaviour
{
    [SerializeField] FirstPersonController firstPersonController;
    [SerializeField] Animator animator;
    [SerializeField] private float blendTreeSmoothTime = 0.1f;

    private const string isWalking = "IsWalking";
    private const string isRunning = "IsRunning";
    private const string horizontalVelocity = "X";
    private const string verticalVelocity = "Y";
    private const string jumpUp = "JumpUp";
    private const string jumpForward = "JumpForward";
    private const string isFalling = "Falling";
    private const string isLanding = "Landing";
    private const string isTurningLeft = "TurningLeft";
    private const string isTurningRight = "TurningRight";

    private void Start()
    {
        if (animator == null) { }
            animator = GetComponent<Animator>();

        if (firstPersonController == null)
            firstPersonController = GetComponentInParent<FirstPersonController>();

        firstPersonController.OnJump += FirstPersonController_OnJump;
    }

    private void FirstPersonController_OnJump(object sender, FirstPersonController.OnJumpEventArgs e)
    {
        if (e.jumpUp) animator.SetTrigger(jumpUp);
        else if (e.jumpForward) animator.SetTrigger(jumpForward);
    }

    private void Update()
    {
        animator.SetFloat(horizontalVelocity, firstPersonController.MoveDirection.x, blendTreeSmoothTime, Time.deltaTime);
        animator.SetFloat(verticalVelocity, firstPersonController.MoveDirection.z, blendTreeSmoothTime, Time.deltaTime);
        animator.SetBool(isWalking, firstPersonController.IsWalking);
        animator.SetBool(isRunning, firstPersonController.IsRunning);
        animator.SetBool(isFalling, firstPersonController.IsFalling);
        animator.SetBool(isLanding, firstPersonController.IsLanding);
        animator.SetBool(isTurningLeft, firstPersonController.IsTurningLeft);
        animator.SetBool(isTurningRight, firstPersonController.IsTurningRight);
    }
}