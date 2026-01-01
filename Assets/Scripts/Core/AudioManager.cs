using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.5f, 1.5f)]
        public float pitch = 1f;
        
        public bool loop = false;
        public AudioSource source;
    }
    
    [Header("Sound Settings")]
    public Sound[] sounds;
    
    [Header("Environmental Audio")]
    public float ambientVolume = 0.5f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        InitializeSounds();
    }
    
    void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    
    public void PlaySound(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }
    
    public void PlaySoundAtPosition(string name, Vector3 position)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            AudioSource.PlayClipAtPoint(s.clip, position, s.volume);
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }
    
    public void StopSound(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Stop();
        }
    }
    
    public void SetVolume(string name, float volume)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.volume = Mathf.Clamp01(volume);
        }
    }
    
    public void PlayAmbientSound(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.volume = ambientVolume;
            s.source.loop = true;
            s.source.Play();
        }
    }
    
    public void StopAmbientSound(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.loop = false;
        }
    }
}