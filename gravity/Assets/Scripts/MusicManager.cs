// using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    private float clipVolume = 1.0f;
    
    [SerializeField] float fadeTime = 2f;
    [SerializeField] AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("References")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] AudioListener audioListener;
    [SerializeField] List<AudioClip> music; // could not figure out AssetBundles or Addressables. ugh
    [SerializeField] Image pauseImage;
    [SerializeField] Sprite pause;
    [SerializeField] Sprite play;
    [SerializeField] TMP_Text songName;

    private int trackIndex = 0;
    
    void Awake()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            volumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume"));
            AudioListener.volume = volumeSlider.value;
        } else
        {
            PlayerPrefs.SetFloat("MusicVolume", volumeSlider.value);
            UpdatePlayerVolume();
        }

        // pick random track index to start playing
        trackIndex = Random.Range(0, music.Count);
        PlayIndex(trackIndex);
        
        musicSource.Play();
    }
    
    public void UpdatePlayerVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", volumeSlider.value);
        AudioListener.volume = volumeSlider.value; // actually i remembered why i don't do this. this is master volume only
    }

    public void TogglePause()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            pauseImage.sprite = play;
        } else
        {
            musicSource.UnPause();
            pauseImage.sprite = pause;
        }
    }

    void Update()
    {
        if (musicSource.isPlaying)
        {
            pauseImage.sprite = pause;
        } else
        {
            pauseImage.sprite = play;
        }
    }

    void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            musicSource.DOFade(volume, fadeTime).SetEase(fadeCurve).SetUpdate(true);
        } else
        {
            // fade current music source
            Sequence fadeSequence = DOTween.Sequence();
            fadeSequence.Append(musicSource.DOFade(0f, fadeTime/2)
                                    .SetEase(fadeCurve).SetUpdate(true)
                                    .OnComplete(() => {musicSource.clip = clip; musicSource.volume = 0f; musicSource.Play();})); // fade out and set next clip
            fadeSequence.Append(musicSource.DOFade(volume, fadeTime/2).SetEase(fadeCurve).SetUpdate(true)); // fade in
            fadeSequence.Play();
        }

    }

    public void PlayIncrement(int delta)
    {
        trackIndex += delta;
        trackIndex += music.Count;
        trackIndex %= music.Count; // remainder above zero
        PlayIndex(trackIndex);
    }

    void PlayIndex(int index)
    {
        Play(music[index]);
        songName.text = music[index].name;
    }
}