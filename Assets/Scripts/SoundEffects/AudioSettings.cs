using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mainMixer;

    public float MasterVolume
    {
        get
        {
            float result;
            mainMixer.GetFloat("MasterVolume", out result);
            return result;
        }
        set { mainMixer.SetFloat("MasterVolume", value); }
    }
    public float EffectsVolume
    {
        get
        {
            float result;
            mainMixer.GetFloat("EffectsVolume", out result);
            return result;
        }
        set { mainMixer.SetFloat("EffectsVolume", value); }
    }
    public float MusicVolume
    {
        get
        {
            float result;
            mainMixer.GetFloat("MusicVolume",out result);
            return result;
        }
        set { mainMixer.SetFloat("MusicVolume", value); }
    }

    private void Start()
    {
        LoadSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("EffectsVolume", EffectsVolume);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
    }

    public void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0);
        EffectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 0);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0);
    }
}
