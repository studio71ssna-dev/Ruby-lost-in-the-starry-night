using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Singleton;

public class AudioManager : SingletonPersistent
{
    public static AudioManager Instance => GetInstance<AudioManager>();

    [System.Serializable]
    public class AudioClipInfo
    {
        public string name;
        public AudioClip clip;
        public bool loop;
    }

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClipInfo> audioClips;

    [Header("Audio Source Pool")]
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private Transform audioSourceParent;

    private Dictionary<string, AudioClipInfo> clipDictionary = new();
    private Queue<AudioSource> audioPool = new();
    void Start()
    {
        if (audioSourceParent == null)
        {
            audioSourceParent = new GameObject("AudioSources").transform;
            audioSourceParent.SetParent(transform);
        }

        foreach (var entry in audioClips)
        {
            if (entry.clip != null)
                clipDictionary[entry.name] = entry;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateAudioSource();
        }
    }

    private AudioSource CreateAudioSource()
    {
        GameObject obj = new GameObject("AudioSource");
        obj.transform.SetParent(audioSourceParent);

        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;

        obj.SetActive(false);
        audioPool.Enqueue(source);

        return source;
    }

    private AudioSource GetSource()
    {
        if (audioPool.Count == 0)
            return CreateAudioSource();

        return audioPool.Dequeue();
    }
    public void Play(string soundName, Vector3 position)
    {
        if (Instance != null)
            Instance.PlayInternal(soundName, position);
    }

    private void PlayInternal(string soundName, Vector3 position)
    {
        if (!clipDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"Sound {soundName} not found");
            return;
        }

        AudioClipInfo clipInfo = clipDictionary[soundName];
        AudioSource source = GetSource();

        source.clip = clipInfo.clip;
        source.loop = clipInfo.loop;

        source.transform.position = position;

        source.gameObject.SetActive(true);
        source.Play();

        if (!clipInfo.loop)
            StartCoroutine(ReturnToPool(source, clipInfo.clip.length));
    }

    public static void Stop(string soundName)
    {
        if (Instance != null)
            Instance.StopInternal(soundName);
    }

    private void StopInternal(string soundName)
    {
        if (!clipDictionary.ContainsKey(soundName))
            return;

        AudioSource[] sources = audioSourceParent.GetComponentsInChildren<AudioSource>();

        foreach (var s in sources)
        {
            if (s.clip == clipDictionary[soundName].clip)
            {
                s.Stop();
                s.gameObject.SetActive(false);
                audioPool.Enqueue(s);
            }
        }
    }

    private IEnumerator ReturnToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (source != null)
        {
            source.Stop();
            source.gameObject.SetActive(false);
            audioPool.Enqueue(source);
        }
    }
}