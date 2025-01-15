using UnityEngine;


public class PlayerSounds : MonoBehaviour
{
    FirstPersonController firstPersonController;
    [SerializeField] float walkingVolume = 1f; // 1 = 100% of original volume
    [SerializeField] float runningVolume = 1f;
    [SerializeField] float landingVolume = 1f;

    private void Start()
    {
        firstPersonController = Player.Instance.FirstPersonController;
        firstPersonController.OnLanding += FirstPersonController_OnLanding;
    }

    private void FirstPersonController_OnLanding(object sender, System.EventArgs e)
    {
        SoundManager.PlaySound(SoundManager.Sound.Landing, landingVolume);
    }

    private void Update()
    {
        if (firstPersonController.IsWalking)
        {
            if (!SoundManager.IsSoundPlaying(SoundManager.Sound.Walking))
                SoundManager.PlaySound(SoundManager.Sound.Walking, walkingVolume);
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
                SoundManager.PlaySound(SoundManager.Sound.Running, runningVolume);
            }

        }
        else
        {
            if (SoundManager.IsSoundPlaying(SoundManager.Sound.Running)) 
                SoundManager.StopPlayingSound(SoundManager.Sound.Running);
        }
        
    }
}
