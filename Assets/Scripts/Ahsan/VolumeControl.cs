using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] float minVolume = -80;
    [SerializeField] float maxVolume = 0;
    [SerializeField] private string songVolumeParam;
    [SerializeField] private string sfxVolumeParam;

    public void SetParam(float value, string param)
    {
        audioMixer.SetFloat(param, Mathf.Lerp(minVolume, maxVolume, value));
    }

    public void SetSongVolume(float value)
    {
        SetParam(value, songVolumeParam);
    }
    
    
    public void SetSFXVolume(float value)
    {
        SetParam(value, sfxVolumeParam);
    }
}
