using System;
using R3;
using Runtime.Extensions;
using UnityEngine;

public class SoundEffectSource : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private AudioClip audioClip;
    private float volumeMultiplier;
        
    public void PlaySound(AudioClip sound, Observable<Unit> onStop, float newVolumeMultiplier = 1, float pitch = 1)
    {
        volumeMultiplier = newVolumeMultiplier;
        AudioManager.GameVolume.Subscribe(SetSourceVolume).AddTo(this);
        AudioManager.SFXVolume.Subscribe(SetSourceVolume).AddTo(this);
        SetSourceVolume();
        audioSource.clip = sound;
        audioSource.pitch = pitch;
        audioSource.Play();
        Observable.Timer(TimeSpan.FromSeconds(sound.length + 0.1f)).Subscribe(_ => Destroy(gameObject)).AddTo(this);
        onStop.Subscribe(_ => Destroy(gameObject)).AddTo(this);
    }

    private void SetSourceVolume() => audioSource.volume = AudioManager.GameVolume.CurrentValue * 
                                                           AudioManager.SFXVolume.CurrentValue *
                                                           volumeMultiplier;
}