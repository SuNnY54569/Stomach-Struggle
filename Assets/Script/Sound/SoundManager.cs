using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        if (instance == null)
        {
            Debug.LogError("SoundManager instance is missing.");
            return;
        }

        if (instance.audioSource == null)
        {
            Debug.LogError("SoundManager: AudioSource is not assigned.");
            return;
        }

        if ((int)sound >= instance.soundList.Length || instance.soundList[(int)sound].Sounds == null || instance.soundList[(int)sound].Sounds.Length == 0)
        {
            Debug.LogError($"SoundManager: No sounds assigned for SoundType {sound}.");
            return;
        }
        
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomCilp = clips[Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomCilp);
        Debug.Log($"play {randomCilp}");
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
