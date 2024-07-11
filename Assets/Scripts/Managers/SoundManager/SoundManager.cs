using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    //  !  ALL  GAME  SOUNDS  HERE  !
    #region Game Sounds
    public enum Sound
    {
        Test,
        Click,
        Weapons,
        WeaponPickUp,
        WeaponDrop,
        MauserC96Shoot,
        MauserC96Reload,
        Walking,
        Running,
        Jump,
        Landing,
        RunningBreathing,
        WalkingBreathing,
        IdleBreathing,
        // Add more sounds here
    }
    #endregion
    
    private static SoundManager _instance;

    [SerializeField] private string audioMixerGroup = "Master";
    [SerializeField] private AudioMixer audioMixer;

    #region Create Instance When Called First Time
    private static readonly object _lock = new object();
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        SoundManager prefab = Resources.Load<SoundManager>("SoundManager");

                        if (prefab == null)
                        {
                            Debug.LogError("SoundManager prefab not found in Resources!");
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
    #endregion

    #region Nested Classes
    [System.Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public class SoundAudioClipArray
    {
        public Sound sound;
        public AudioClip[] audioClips;
    }
    #endregion

    #region Variables

    public SoundClipsSO soundAudioClipsSO;
    public SoundClipArraysSO soundClipsArraysSO;
    private static AudioSource oneShotAudioSource;
    public Dictionary<Sound, GameObject> playingSounds = new Dictionary<Sound, GameObject>();
    public static Dictionary<Sound, float> soundTimerDictionary;

    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize(); // Ensure Initialize is called
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        foreach (Sound sound in System.Enum.GetValues(typeof(Sound)))
        {
            soundTimerDictionary[sound] = 0f;
        }
    }
    public static void PlaySound(Sound sound, Vector3 position, float volume = 1f)
    {
        GameObject oneShotGameObject = new GameObject("One Shot Sound");
        oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        oneShotAudioSource.volume = volume;
        oneShotAudioSource.outputAudioMixerGroup = Instance.audioMixer.outputAudioMixerGroup;
        DontDestroyOnLoad(oneShotGameObject);
        AudioClip audioClip = GetAudioClip(sound);
        AudioMixerGroup[] audioMixerGroups = Instance.audioMixer.FindMatchingGroups(Instance.audioMixerGroup);
        if (audioMixerGroups.Length > 0)
        {
            oneShotAudioSource.outputAudioMixerGroup = audioMixerGroups[0];
        }
        else
        {
            Debug.LogError("AudioMixerGroup not found");
        }
        oneShotAudioSource.clip = audioClip;
        if (audioClip != null)
        {
            // Debug.Log($"Playing sound: {sound} as one-shot");
            oneShotAudioSource.gameObject.transform.position = position;
            oneShotAudioSource.spatialBlend = 1f; // 3d sound
            oneShotAudioSource.clip = audioClip;
            oneShotAudioSource.Play();
            Instance.AddPlayingSound(oneShotAudioSource, sound);
        }
        else
        {
            Debug.LogError($"AudioClip for sound: {sound} is null");
            Destroy(oneShotGameObject);
        }
        Destroy(oneShotAudioSource, audioClip.length);
    }
    public static void PlaySoundRandom(Sound sound, Vector3 position, float volume = 1f)
    {
        GameObject oneShotGameObject = new GameObject("One Shot Sound");
        oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        oneShotAudioSource.volume = volume;
        oneShotAudioSource.outputAudioMixerGroup = Instance.audioMixer.outputAudioMixerGroup;
        DontDestroyOnLoad(oneShotGameObject);
        AudioClip[] audioClipArray = GetAudioClipArray(sound);
        AudioClip audioClip;
        if (audioClipArray != null)
             audioClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
        else audioClip = GetAudioClip(sound);
        AudioMixerGroup[] audioMixerGroups = Instance.audioMixer.FindMatchingGroups(Instance.audioMixerGroup);
        if (audioMixerGroups.Length > 0)
        {
            oneShotAudioSource.outputAudioMixerGroup = audioMixerGroups[0];
        }
        else
        {
            Debug.LogError("AudioMixerGroup not found");
        }
        oneShotAudioSource.clip = audioClip;
        if (audioClip != null)
        {
            // Debug.Log($"Playing sound: {sound} as one-shot");
            oneShotAudioSource.gameObject.transform.position = position;
            oneShotAudioSource.spatialBlend = 1f; // 3d sound
            oneShotAudioSource.clip = audioClip;
            oneShotAudioSource.Play();
            Instance.AddPlayingSound(oneShotAudioSource, sound);
        }
        else
        {
            Debug.LogError($"AudioClip for sound: {sound} is null");
            Destroy(oneShotGameObject);
        }
        Destroy(oneShotAudioSource, audioClip.length);
    }
    public static float GetSoundLenght(Sound sound)
    {
        AudioClip clip = GetAudioClip(sound);
        if (clip == null) clip = GetAudioClipArray(sound)[0];
        if (clip == null) return 0f;
        return clip.length;
    }
    public static void PlaySound(Sound sound, float volume = 1f)
    {
        GameObject oneShotGameObject = new GameObject("One Shot Sound");
        oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        oneShotAudioSource.volume = volume;
        DontDestroyOnLoad(oneShotGameObject);
        AudioClip audioClip = GetAudioClip(sound);
        AudioMixerGroup[] audioMixerGroups = Instance.audioMixer.FindMatchingGroups(Instance.audioMixerGroup);
        if (audioMixerGroups.Length > 0)
        {
            oneShotAudioSource.outputAudioMixerGroup = audioMixerGroups[0];
        }
        else
        {
            Debug.LogError("AudioMixerGroup not found");
        }
        if (audioClip != null)
        {
            // Debug.Log($"Playing sound: {sound} as one-shot");
            oneShotAudioSource.clip = audioClip;
            oneShotAudioSource.Play();
            Instance.AddPlayingSound(oneShotAudioSource, sound);
        }
        else
        {
            Debug.LogError($"AudioClip for sound: {sound} is null");
            Destroy(oneShotAudioSource);
        }
    }
    private void AddPlayingSound(AudioSource oneShotAudioSource, Sound sound)
    {
        if (playingSounds.ContainsKey(sound)) return;
        StartCoroutine(AddPlayingSoundCoroutine(oneShotAudioSource, sound));
    }
    IEnumerator AddPlayingSoundCoroutine(AudioSource oneShotAudioSource, Sound sound)
    {
        Debug.Log("Playing sound Added");
        playingSounds.Add(sound, oneShotAudioSource.gameObject);
        float timer = 0;
        bool soundHasStopped = false;
        float clipLenght = oneShotAudioSource.clip.length;

        while (timer < clipLenght)
        {
            timer += Time.deltaTime;
            if (oneShotAudioSource == null)
            {
                soundHasStopped = true;
                break;
            }
            yield return null;
        }
        if (!soundHasStopped)
            StopPlayingSound(sound);
    }
    public static void StopPlayingSound(Sound sound)
    {
        if (Instance.playingSounds.TryGetValue(sound, out GameObject oneShotAudioSource))
        {
            Debug.Log("Playing sound removed");
            Instance.playingSounds.Remove(sound);
            Destroy(oneShotAudioSource);
        }
    }
    public static void PlaySoundRandom(Sound sound, float volume = 1f)
    {
        GameObject oneShotGameObject = new GameObject("One Shot Sound");
        oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        oneShotAudioSource.volume = volume;
        oneShotAudioSource.outputAudioMixerGroup = Instance.audioMixer.outputAudioMixerGroup;
        DontDestroyOnLoad(oneShotGameObject);
        AudioClip[] audioClipArray = GetAudioClipArray(sound);
        AudioClip audioClip;
        if (audioClipArray != null)
             audioClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
        else audioClip = GetAudioClip(sound);
        AudioMixerGroup[] audioMixerGroups = Instance.audioMixer.FindMatchingGroups(Instance.audioMixerGroup);
        if (audioMixerGroups.Length > 0)
        {
            oneShotAudioSource.outputAudioMixerGroup = audioMixerGroups[0];
        }
        else
        {
            Debug.LogError("AudioMixerGroup not found");
        }
        if (audioClip != null)
        {
            oneShotAudioSource.clip = audioClip;
            oneShotAudioSource.Play();
            Instance.AddPlayingSound(oneShotAudioSource, sound);
        }
        else
        {
            Debug.LogError($"AudioClip for sound: {sound} is null");
            Destroy(oneShotGameObject);
        }
        Destroy(oneShotAudioSource, audioClip.length);
    }
    public static void PlaySoundWithCooldown(Sound sound, float cooldown, float volume = 1f)
    {
        if (CanPlaySound(sound, cooldown))
        {
            PlaySound(sound, volume);
        }
    }
    public static void PlaySoundRandomWithCooldown(Sound sound, float cooldown, float volume = 1f)
    {
        if (CanPlaySound(sound, cooldown))
        {
            PlaySoundRandom(sound, volume);
        }
    }
    private static bool CanPlaySound(Sound sound, float cooldown)
    {
        if (soundTimerDictionary == null)
        {
            Initialize(); // Ensure the dictionary is initialized
        }

        if (soundTimerDictionary.ContainsKey(sound))
        {
            float lastTimePlayed = soundTimerDictionary[sound];
            float cooldownTime = cooldown; // Adjust this value as needed
            float timeSinceLastPlayed = Time.time - lastTimePlayed;
            // Debug.Log($"Time since last played {sound}: {timeSinceLastPlayed}s, Cooldown time: {cooldownTime}s");

            if (timeSinceLastPlayed >= cooldownTime)
            {
                soundTimerDictionary[sound] = Time.time;
                // Debug.Log($"Sound {sound} can be played. Updating last played time.");
                return true;
            }
            else
            {
                // Debug.Log($"Sound {sound} cannot be played. Cooldown active.");
                return false;
            }
        }
        else
        {
            // Debug.Log($"Sound {sound} not found in dictionary. Adding to dictionary and allowing play.");
            soundTimerDictionary[sound] = Time.time;
            return true;
        }
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundAudioClip soundAudioClip in Instance.soundAudioClipsSO.soundAudioClips)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        return null;
    }

    private static AudioClip[] GetAudioClipArray(Sound sound)
    {
        foreach (SoundAudioClipArray soundAudioClipArray in Instance.soundClipsArraysSO.soundAudioClipArrays)
        {
            if (soundAudioClipArray.sound == sound)
            {
                return soundAudioClipArray.audioClips;
            }
        }
        return null;
    }
}