using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider vfxSlider;

    private void Start()
    {  
        if (!PlayerPrefs.HasKey("BGMVolume"))
        {
            PlayerPrefs.SetFloat("BGMVolume", 0.5f);
        }
        if (!PlayerPrefs.HasKey("VFXVolume"))
        {
            PlayerPrefs.SetFloat("VFXVolume", 0.5f);
        }

        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        vfxSlider.value = PlayerPrefs.GetFloat("VFXVolume");

        SoundManager.Instance.SetBGMVolume(bgmSlider.value);
        SoundManager.Instance.SetVFXVolume(vfxSlider.value);

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        vfxSlider.onValueChanged.AddListener(SetVFXVolume);
    }

    public void SetBGMVolume(float volume)
    {
        SoundManager.Instance.SetBGMVolume(volume);
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetVFXVolume(float volume)
    {
        SoundManager.Instance.SetVFXVolume(volume);
        PlayerPrefs.SetFloat("VFXVolume", volume);
    }
}
