using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create SoundsData/AllSoundClipArrays")]
public class SoundClipArraysSO : ScriptableObject
{
    public SoundManager.SoundAudioClipArray[] soundAudioClipArrays;
}
