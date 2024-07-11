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
            if (SoundManager.Instance.playingSounds.ContainsKey(SoundManager.Sound.Walking)) return;
            SoundManager.PlaySound(SoundManager.Sound.Walking);
        }
        else
        {
            if (SoundManager.Instance.playingSounds.ContainsKey(SoundManager.Sound.Walking))
                SoundManager.StopPlayingSound(SoundManager.Sound.Walking);
        }
        
    }
}
