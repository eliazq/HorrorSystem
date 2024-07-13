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
            if (!SoundManager.IsSoundPlaying(SoundManager.Sound.Walking))
                SoundManager.PlaySound(SoundManager.Sound.Walking);
        }
        else
        {
            if (SoundManager.IsSoundPlaying(SoundManager.Sound.Walking))
                SoundManager.StopPlayingSound(SoundManager.Sound.Walking);
        }
        if (firstPersonController.IsRunning)
        {
            // If Sound Is Playing It Wont Play It
            if (SoundManager.IsSoundPlaying(SoundManager.Sound.Running)) return;
            else
            {
                SoundManager.PlaySound(SoundManager.Sound.Running);
            }

        }
        else
        {
            if (SoundManager.IsSoundPlaying(SoundManager.Sound.Running)) 
                SoundManager.StopPlayingSound(SoundManager.Sound.Running);

        }
        
    }
}
