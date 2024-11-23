using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    Hurt,
    StartSceneBG,
    UIClick,
    TestBG,
    CorrectAnswer,
    WrongAnswer,
    Win,
    Lose,
    FinishDay,
    PickUpMeat,
    ClawBG,
    BadShopBG,
    CleanShopSFX,
    WashHandBG,
    BBExpolde,
    BBWarning,
    Clock,
    flipMeat,
    PlaceOnPlate,
    PlaceOnTrash,
    GrillBg,
    SteakBg,
    CheckBox,
    ClockTicking,
    meatInBag,
    BgInBuild,
    BgOutHome,
    BgInHome,
    BgDay,
    BgNight,
    PressCard,
    OpenHomeDoor,
    CloseHomeDoor,
    BusOpenDoor,
    BuildOpenDoor,
    walk,
    BusCome,
    BusRun,
    BgMarket,
    BusRunTenS,
    PhoneRing,
    Dialouge1Day1,
    Dialouge2Day1,
    Dialouge3Day1,
    Dialouge4Day1,
    Dialouge5Day1,
    Dialouge6Day1,
    Dialouge7Day1,
    Dialouge8Day1,
    Dialouge9Day1,
    Dialouge10Day1,
    Dialouge11Day1,
    Dialouge12Day1,
    Dialouge13Day1,
    Dialouge14Day1,
    Dialouge3Day2,
    Dialouge4Day2,
    Dialouge5Day2,
    Dialouge6Day2,
    Dialouge7Day2,
    Dialouge1Day3,
    Dialouge2Day3,
    Dialouge3Day3,
    Dialouge4Day3,
    Dialouge5Day3,
    Dialouge6Day3,
    Dialouge7Day3,
    Dialouge8Day3
}

[Serializable]
public class LevelBGM
{
    public string LevelName;
    public SoundType[] BGMSoundTypes;
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
    private string currentLevel = "";

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

    #region Play Sound Methods

    public static void PlaySound(SoundType sound, VolumeType volumeType, float volumeScale = 1f)
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
        source.PlayOneShot(randomClip, instance.volumeLevels[volumeType] * volumeScale);
    }

    #endregion

    #region Multi-BGM Methods

    public static void PlayRandomBGMForLevel(string levelName, float crossfadeDuration = 1f, float volumeScale = 0.5f)
    {
        if (instance == null || !instance.audioSources.ContainsKey(VolumeType.Background))
        {
            return;
        }

        LevelBGM levelBGM = instance.levelBGMs.Find(bgm => bgm.LevelName == levelName);

        if (levelBGM == null || levelBGM.BGMSoundTypes.Length == 0)
        {
            Debug.LogError($"SoundManager: No BGM defined for level {levelName}.");
            return;
        }

        // Randomly select a BGM for the level
        SoundType randomSound = levelBGM.BGMSoundTypes[Random.Range(0, levelBGM.BGMSoundTypes.Length)];
        instance.CrossfadeBGM(randomSound, crossfadeDuration);
    }

    public void CrossfadeBGM(SoundType sound, float duration)
    {
        if (crossfadeCoroutine != null)
            StopCoroutine(crossfadeCoroutine);

        crossfadeCoroutine = StartCoroutine(CrossfadeCoroutine(sound, duration));
    }

    private IEnumerator CrossfadeCoroutine(SoundType sound, float duration)
    {
        AudioSource bgmSource = audioSources[VolumeType.Background];

        if (bgmSource.isPlaying)
        {
            // Fade out current BGM
            float initialVolume = bgmSource.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(initialVolume, 0, t / duration);
                yield return null;
            }
            bgmSource.Stop();
            bgmSource.volume = initialVolume;
        }

        // Play new BGM
        AudioClip[] clips = soundList[(int)sound].Sounds;
        if (clips == null || clips.Length == 0) yield break;

        AudioClip newClip = clips[Random.Range(0, clips.Length)];
        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.Play();

        // Fade in new BGM
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, volumeLevels[VolumeType.Background], t / duration);
            yield return null;
        }

        bgmSource.volume = volumeLevels[VolumeType.Background];
    }

    #endregion

    #region Volume Management

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

    public void PlayUIClick()
    {
        PlaySound(SoundType.UIClick, VolumeType.SFX);
    }
    #endregion

    #region Level Detection and BGM Change

    private void Start()
    {
        // Initialize the level tracking and play BGM for the current level
        //UpdateLevelBGM();
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update BGM whenever a new scene is loaded
        UpdateLevelBGM();
    }

    public void UpdateLevelBGM()
    {
        string newLevel = SceneManager.GetActiveScene().name;

        if (currentLevel != newLevel)
        {
            currentLevel = newLevel;
            PlayRandomBGMForLevel(currentLevel, 1f); // 1f for crossfade duration
        }
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

    #region StopSound
    public static void StopAllSounds()
    {
        if (instance == null)
        {
            Debug.LogError("SoundManager: Instance is missing.");
            return;
        }

        foreach (var source in instance.audioSources.Values)
        {
            if (source.isPlaying)
            {
                source.Stop();
                source.clip = null;
            }
        }
    }


    #endregion
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds => sounds;
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
