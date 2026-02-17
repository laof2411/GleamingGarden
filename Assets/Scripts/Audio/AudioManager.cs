using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private Dictionary<string, AudioSource> soundDictionary;
    [SerializeField] private AudioMixer audioMixer;

    public AudioPreset[] presets;

    public void Awake()
    {
        if (instance == null) 
        {
            transform.SetParent(null);
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            soundDictionary = new Dictionary<string, AudioSource>();

            foreach (AudioPreset preset in presets)
            {
                soundDictionary.Add(preset.audioName, gameObject.AddComponent<AudioSource>());
                soundDictionary[preset.audioName].playOnAwake = false;
                soundDictionary[preset.audioName].clip = preset.clip;
                soundDictionary[preset.audioName].volume = preset.volume;

                soundDictionary[preset.audioName].loop = preset.loop;
                soundDictionary[preset.audioName].outputAudioMixerGroup = audioMixer.FindMatchingGroups(preset.audioType.ToString())[0];
            }           
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void PlayAudio(string audioName)
    {
        //Debug.Log("PlayAudio " + audioName);
        soundDictionary[audioName].Play();
    }

    public void StopAudio(string audioName)
    {
        if (soundDictionary[audioName].isPlaying)
        {
            //Debug.Log("StopAudio " + audioName);

            soundDictionary[audioName].Stop();
        }
    }

    public void ChangeVolume(AudioType audioType, float settingsVolume) 
    {
        audioMixer.SetFloat(audioType.ToString(), Mathf.Log10(settingsVolume) * 20);
    }

    public AudioMixer GetAudioMixer()
    {
        return audioMixer;
    }
}

[System.Serializable]
public class AudioPreset
{
    public string audioName;
    public AudioClip clip;

    [Range(0, 1)]
    public float volume;

    public bool loop;

    public AudioType audioType;
}

public enum AudioType
{
    Master,
    Music,
    SoundEffect,
    UI,
}
