using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public AudioSource source;
    public bool loop;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;


    public float currentVolume { get; private set; }

    private int currZombiesAudioCount = 0;

    [SerializeField] private AudioSource[] zombiesAudio;
    [SerializeField] private Sound[] sounds;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        currentVolume = PlayerPrefs.GetFloat("Volume");
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = currentVolume;
            s.source.loop = s.loop;
        }

        UpdateVolume(currentVolume);
    }
    public void UpdateZombiesAudio(int audioToEnable)
    {
        audioToEnable = Mathf.Clamp(audioToEnable, 0, 9);
        if(currZombiesAudioCount > audioToEnable)
        {
            for(int i = audioToEnable;i<=currZombiesAudioCount;i++)
            {
                zombiesAudio[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = currZombiesAudioCount; i <= audioToEnable; i++)
            {
                zombiesAudio[i].gameObject.SetActive(true);
            }
        }
        currZombiesAudioCount = audioToEnable;
    }
    public void PlaySound(string name,bool playingCheck = false)
    {
        foreach (Sound s in sounds)
        {
            if(s.name == name)
            {
                if(playingCheck)
                {
                    if (!s.source.isPlaying)
                        s.source.Play();
                }
                else
                {
                    s.source.Play();

                }
            }
        }
    }

    public void StopSound(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.Stop();
            }
        }
    }
    public void UpdateVolume(float targetVolume)
    {
        PlayerPrefs.SetFloat("Volume",targetVolume);
        currentVolume = targetVolume;
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach(AudioSource a in sources)
        {
            a.volume = targetVolume;
        }
        foreach(AudioSource s in zombiesAudio)
        {
            s.volume = targetVolume;
        }
    }
}
