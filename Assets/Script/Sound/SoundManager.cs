using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public enum VolumeType
{
    Background,
    SFX,
    Dialog
}

public enum SoundType
{
    Hurt
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;
    
    private Dictionary<VolumeType, float> volumeLevels = new Dictionary<VolumeType, float>()
    {
        { VolumeType.Background, 1f },
        { VolumeType.SFX, 1f },
        { VolumeType.Dialog, 1f }
    };

    private void Awake()
    {
        if (!Application.isPlaying) return;
        
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("SoundManager: AudioSource component is missing.");
        }
        
        LoadVolumeSettings();
    }

    public static void PlaySound(SoundType sound, VolumeType volumeType)
    {
        if (instance == null || instance.audioSource == null)
        {
            Debug.LogError("SoundManager: AudioSource or instance is missing.");
            return;
        }

        if ((int)sound >= instance.soundList.Length || instance.soundList[(int)sound].Sounds == null || instance.soundList[(int)sound].Sounds.Length == 0)
        {
            Debug.LogError($"SoundManager: No sounds assigned for SoundType {sound}.");
            return;
        }
        
        float adjustedVolume = instance.volumeLevels[volumeType];
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, adjustedVolume);
    }
    
    public static void SetVolume(VolumeType volumeType, float volume)
    {
        if (instance != null)
        {
            instance.volumeLevels[volumeType] = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(volumeType.ToString(), volume);
        }
    }
    
    public static float GetVolume(VolumeType volumeType)
    {
        if (instance != null && instance.volumeLevels.ContainsKey(volumeType))
        {
            return instance.volumeLevels[volumeType];
        }
        return 1f;
    }
    
    private void LoadVolumeSettings()
    {
        foreach (VolumeType type in Enum.GetValues(typeof(VolumeType)))
        {
            volumeLevels[type] = PlayerPrefs.GetFloat(type.ToString(), 1f);
        }
    }
    
#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
