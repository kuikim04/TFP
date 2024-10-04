using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource vfxSource;

    #region Audio

    [SerializeField] private AudioClip bgmClipMain;

    [SerializeField] private AudioClip bgmClipPlayR1;
    [SerializeField] private AudioClip bgmClipPlayR2;
    [SerializeField] private AudioClip bgmClipPlayR3;
    [SerializeField] private AudioClip bgmClipPlayR4;

    [SerializeField] private AudioClip bgmClipPlayTap;
    [SerializeField] private AudioClip bgmClipPlayArrow;
    [SerializeField] private AudioClip bgmClipPlayRps;

    [SerializeField] private AudioClip clickSoundClip;

    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip defeatSound;

    [SerializeField] private AudioClip buySound;
    [SerializeField] private AudioClip cantBuySound;

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            
            bgmSource.loop = true;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
    }

    public void SetVFXVolume(float volume)
    {
        audioMixer.SetFloat("VFXVolume", Mathf.Log10(volume) * 20);
    }

    public void PlayMusicMainMenu()
    {
        PlayBGM(bgmClipMain);
    }

    public void PlayMusicRegion1()
    {
        PlayBGM(bgmClipPlayR1);
    }
    public void PlayMusicRegion2()
    {
        PlayBGM(bgmClipPlayR2);
    }
    public void PlayMusicRegion3()
    {
        PlayBGM(bgmClipPlayR3);
    }
    public void PlayMusicRegion4()
    {
        PlayBGM(bgmClipPlayR4);
    }

    public void PlayMusicMiniGameTap()
    {
        PlayBGM(bgmClipPlayTap);
    }
    public void PlayMusicMiniGameArrow()
    {
        PlayBGM(bgmClipPlayArrow);
    }

    public void PlayMusicMiniGameRPS()
    {
        PlayBGM(bgmClipPlayRps);
    }

    public void ClickSound()
    {
        if (clickSoundClip != null)
        {
            PlayVFX(clickSoundClip);
        }
    }
    public void VictoryVFX()
    {
        if (victorySound != null)
        {
            PlayVFX(victorySound);
        }
    }

    public void DefeatVFX()
    {
        if (defeatSound != null)
        {
            PlayVFX(defeatSound);
        }
    }
    public void BuyFX()
    {
        if (buySound != null)
        {
            PlayVFX(buySound);
        }
    }
    public void CantBuyVFX()
    {
        if (cantBuySound != null)
        {
            PlayVFX(cantBuySound);
        }
    }

    public void PlayBGM(AudioClip audio)
    {
        if (audio != null)
        {
            bgmSource.clip = audio;
            bgmSource.Play();
        }
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlayVFX(AudioClip audio)
    {
        if (audio != null)
        {
            vfxSource.PlayOneShot(audio);
        }
    }
    public void PlayRandomVFX(AudioClip[] audioClips)
    {
        if (audioClips != null && audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, audioClips.Length);
            AudioClip randomClip = audioClips[randomIndex];
            vfxSource.PlayOneShot(randomClip);
        }
    }
}
