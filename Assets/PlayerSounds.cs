using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    FirstPersonController firstPersonController;

    private void Start()
    {
        firstPersonController = Player.Instance.FirstPersonController;
    }
    private void Update()
    {
        if (firstPersonController.IsWalking)
        {
            SoundManager.PlaySoundRandomWithCooldown(SoundManager.Sound.Walking, SoundManager.GetSoundLenght(SoundManager.Sound.Walking));
            SoundManager.PlaySoundRandomWithCooldown(SoundManager.Sound.WalkingBreathing, SoundManager.GetSoundLenght(SoundManager.Sound.WalkingBreathing));
        }
        else if (firstPersonController.IsRunning)
        {
            SoundManager.PlaySoundRandomWithCooldown(SoundManager.Sound.Running, SoundManager.GetSoundLenght(SoundManager.Sound.Running));
            SoundManager.PlaySoundRandomWithCooldown(SoundManager.Sound.RunningBreathing, SoundManager.GetSoundLenght(SoundManager.Sound.RunningBreathing));
        }
        else if (firstPersonController.IsGrounded)
        {
            SoundManager.PlaySoundRandomWithCooldown(SoundManager.Sound.IdleBreathing, SoundManager.GetSoundLenght(SoundManager.Sound.IdleBreathing));
        }
    }
}
