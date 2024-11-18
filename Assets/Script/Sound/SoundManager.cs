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
    Dialog,
    Tutorial
}

public enum SoundType
{
    Hurt
}

[Serializable]
public class LevelBGM
{
    public string LevelName;
    public SoundType[] BGMSoundTypes; // List of SoundTypes for BGMs in this level.
}

[ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    #region Fields

    [Header("Sound Settings")]
    [Tooltip("List of sounds organized by SoundType.")]
    [SerializeField] private SoundList[] soundList;
    
    [Header("Level BGM Settings")]
    [Tooltip("Define BGMs for each level.")]
    [SerializeField] private List<LevelBGM> levelBGMs;

    public static SoundManager instance;

    public Dictionary<VolumeType, AudioSource> audioSources = new Dictionary<VolumeType, AudioSource>();
    private Dictionary<VolumeType, float> volumeLevels = new Dictionary<VolumeType, float>()
    {
        { VolumeType.Background, 1f },
        { VolumeType.SFX, 1f },
        { VolumeType.Dialog, 1f },
        { VolumeType.Tutorial, 1f }
    };
    
    private Coroutine crossfadeCoroutine;

    #endregion
    
    #region Initialization
    private void Awake()
    {
        if (!Application.isPlaying) return;
        
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void InitializeAudioSources()
    {
        // Create a separate AudioSource for each VolumeType
        foreach (VolumeType type in Enum.GetValues(typeof(VolumeType)))
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = volumeLevels[type]; // Set initial volume from saved settings
            audioSources[type] = source;
        }
    }
    
    private void LoadVolumeSettings()
    {
        foreach (VolumeType type in Enum.GetValues(typeof(VolumeType)))
        {
            float savedVolume = PlayerPrefs.GetFloat(type.ToString(), 1f);
            volumeLevels[type] = savedVolume;
            
            // Update AudioSource volume if it already exists
            if (audioSources.ContainsKey(type))
            {
                audioSources[type].volume = savedVolume;
            }
        }
    }
    
    #endregion
    
    #region Public Methods

    public static void PlaySound(SoundType sound, VolumeType volumeType)
    {
        if (instance == null || !instance.audioSources.ContainsKey(volumeType))
        {
            Debug.LogError("SoundManager: AudioSource or instance is missing.");
            return;
        }

        if ((int)sound >= instance.soundList.Length || instance.soundList[(int)sound].Sounds == null || instance.soundList[(int)sound].Sounds.Length == 0)
        {
            Debug.LogError($"SoundManager: No sounds assigned for SoundType {sound}.");
            return;
        }
        
        AudioSource source = instance.audioSources[volumeType];
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        source.PlayOneShot(randomClip);
    }
    
    public static void CrossfadeBGM(SoundType sound, float duration = 1f)
    {
        if (instance == null || !instance.audioSources.ContainsKey(VolumeType.Background)) return;

        if (instance.crossfadeCoroutine != null)
            instance.StopCoroutine(instance.crossfadeCoroutine);

        instance.crossfadeCoroutine = instance.StartCoroutine(instance.CrossfadeCoroutine(sound, duration));
    }
    
    private IEnumerator CrossfadeCoroutine(SoundType sound, float duration)
    {
        AudioSource bgmSource = audioSources[VolumeType.Background];
        AudioClip[] clips = soundList[(int)sound].Sounds;
        if (clips == null || clips.Length == 0) yield break;

        AudioClip newClip = clips[Random.Range(0, clips.Length)];
        float originalVolume = bgmSource.volume;

        // Fade out
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(originalVolume, 0, t / duration);
            yield return null;
        }

        bgmSource.clip = newClip;
        bgmSource.Play();

        // Fade in
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, originalVolume, t / duration);
            yield return null;
        }

        bgmSource.volume = originalVolume;
    }
    
    public static void SetVolume(VolumeType volumeType, float volume)
    {
        if (instance != null && instance.audioSources.ContainsKey(volumeType))
        {
            volume = Mathf.Clamp01(volume);
            instance.volumeLevels[volumeType] = volume;
            instance.audioSources[volumeType].volume = volume;
            PlayerPrefs.SetFloat(volumeType.ToString(), volume);
        }
    }
    
    public static float GetVolume(VolumeType volumeType)
    {
        return instance != null && instance.volumeLevels.ContainsKey(volumeType) ? instance.volumeLevels[volumeType] : 1f;
    }
    #endregion
    
    #region Editor Only
    
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
    
    #endregion
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds => sounds;
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
