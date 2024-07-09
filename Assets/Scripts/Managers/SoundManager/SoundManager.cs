using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    //  !  ALL  GAME  SOUNDS  HERE  !
    public enum Sound
    {
        Test,
        Click,
        Weapons,
        WeaponPickUp,
        WeaponDrop,
        MauserC96Shoot,
        MauserC96Reload
        // Add more sounds here
    }
    private static SoundManager _instance;
    private static readonly object _lock = new object();

    [SerializeField] private string audioMixerGroup = "Master";

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

    public SoundClipsSO soundAudioClipsSO;
    public SoundClipArraysSO soundClipsArraysSO;

    private static Dictionary<Sound, float> soundTimerDictionary;
    private static AudioSource oneShotAudioSource;
    [SerializeField] private AudioMixer audioMixer;

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
        AudioClip audioClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
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
        }
        else
        {
            Debug.LogError($"AudioClip for sound: {sound} is null");
            Destroy(oneShotGameObject);
        }
        Destroy(oneShotAudioSource, audioClip.length);
    }

    public static void PlaySound(Sound sound)
    {
        GameObject oneShotGameObject = new GameObject("One Shot Sound");
        oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        // Get the AudioMixerGroup
        AudioMixerGroup[] audioMixerGroups = Instance.audioMixer.FindMatchingGroups(Instance.audioMixerGroup);
        if (audioMixerGroups.Length > 0)
        {
            oneShotAudioSource.outputAudioMixerGroup = audioMixerGroups[0];
        }
        else
        {
            Debug.LogError("AudioMixerGroup not found");
        }
        DontDestroyOnLoad(oneShotGameObject);
        AudioClip audioClip = GetAudioClip(sound);
        if (audioClip != null)
        {
            oneShotAudioSource.clip = audioClip;
            oneShotAudioSource.Play();
        }
        else
        {
            Debug.LogError($"AudioClip for sound: {sound} is null");
            Destroy(oneShotAudioSource);
        }
        Destroy(oneShotAudioSource, audioClip.length);
    }

    public static void PlaySoundRandom(Sound sound)
    {
        GameObject oneShotGameObject = new GameObject("One Shot Sound");
        oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        oneShotAudioSource.outputAudioMixerGroup = Instance.audioMixer.outputAudioMixerGroup;
        DontDestroyOnLoad(oneShotGameObject);
        AudioClip[] audioClipArray = GetAudioClipArray(sound);
        AudioClip audioClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
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
            oneShotAudioSource.clip = audioClip;
            oneShotAudioSource.Play();
        }
        else
        {
            Debug.LogError($"AudioClip for sound: {sound} is null");
            Destroy(oneShotGameObject);
        }
        Destroy(oneShotAudioSource, audioClip.length);
    }

    public static void PlaySound(Sound sound, float volume)
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
        }
        else
        {
            Debug.LogError($"AudioClip for sound: {sound} is null");
            Destroy(oneShotAudioSource);
        }
        Destroy(oneShotAudioSource, audioClip.length);
    }

    public static void PlaySoundRandom(Sound sound, float volume)
    {
        GameObject oneShotGameObject = new GameObject("One Shot Sound");
        oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        oneShotAudioSource.volume = volume;
        oneShotAudioSource.outputAudioMixerGroup = Instance.audioMixer.outputAudioMixerGroup;
        DontDestroyOnLoad(oneShotGameObject);
        AudioClip[] audioClipArray = GetAudioClipArray(sound);
        AudioClip audioClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
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
            oneShotAudioSource.clip = audioClip;
            oneShotAudioSource.Play();
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
        Debug.LogError("Sound " + sound + " not found!");
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
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }
}