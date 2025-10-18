using System;
using System.Linq;
using UnityEngine;

[Serializable] 
public enum SoundEffectType
{
    None,
    GetHit,
    MenuHover,
    MenuSelect,
    PunchAttack,
    LowHealth,
    Destroyed,
    FlurryPunch,
    Stunned,
    PunchedAway,
    Good,
    Evil,
}

[Serializable]
public enum MusicTrackID
{
    None,
    Menu,
    Pause,
    Boss1,
    Death,
}
    
[Serializable] 
public struct MusicTrack
{
    public MusicTrackID trackIDName;
    public AudioClip clip;
    public float volumeMultiplier;
    public bool doesLoop;

}

[Serializable]
public struct SoundEffect
{
    public SoundEffectType soundEffectName;
    public AudioClip clip;
    public float volumeMultiplier;
}

    
[Serializable]
[CreateAssetMenu(menuName = "AudioData")]
public class AudioStorage : ScriptableObject
{
    [SerializeField] private MusicTrack[] tracks;
    [SerializeField] private SoundEffect[] sounds;

    public SoundEffect GetSoundEffect(SoundEffectType sfx) => 
        sounds.FirstOrDefault(x => x.soundEffectName == sfx);

    public MusicTrack GetMusicTrack(MusicTrackID track) =>
        tracks.FirstOrDefault(x => x.trackIDName == track);
}