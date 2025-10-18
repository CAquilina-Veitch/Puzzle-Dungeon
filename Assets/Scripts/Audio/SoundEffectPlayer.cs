using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    [SerializeField] private SoundEffectType soundEffect = SoundEffectType.None;
    [SerializeField] private float volumeMultiplier = 1;
    [SerializeField] private float pitchMult = 1;
    [SerializeField] private bool playOnEnable = false;
    private void OnEnable()
    {
        if (playOnEnable)
            PlaySoundEffect(soundEffect, volumeMultiplier, pitchMult);
    }

    public void PlaySoundEffect(SoundEffectType sfx = SoundEffectType.None, float volume = -1f, float pitch = -100)
    {
        sfx = sfx is SoundEffectType.None ? soundEffect : sfx;
        volume = volume < 0 ? volumeMultiplier : volume;
        pitch = pitch < -50 ? pitchMult : pitch;
        AudioManager.PlaySFX(sfx, volume, pitch);
    }
}