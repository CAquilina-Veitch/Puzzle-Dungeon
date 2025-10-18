using System;
using R3;
using Scripts.Behaviours;
using UnityEngine;
using Runtime.Extensions;

public class AudioManager : SingletonBehaviour<AudioManager>
    {
        [SerializeField] private SoundEffectSource soundPlayerPrefab;
        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioStorage audioStorage;
        private MusicTrack currentMusicTrack;
        private float currentMusicVolumeMultiplier = 1;
        
        private readonly CompositeDisposable musicPlayingDisposable = new();
        
        #region Singleton
        
        public static AudioManager Instance;

        #endregion
        
        #region Observables
        
        public static ReadOnlyReactiveProperty<float> MusicVolume => Instance.musicVolume;
        private readonly ReactiveProperty<float> musicVolume = new(1);
        public static ReadOnlyReactiveProperty<float> SFXVolume => Instance.sfxVolume;
        private readonly ReactiveProperty<float> sfxVolume = new(1);
        public static ReadOnlyReactiveProperty<float> GameVolume => Instance.gameVolume;
        private readonly ReactiveProperty<float> gameVolume = new(1);
        public Observable<Unit> StopAllSFX => stopAllSFX;
        private readonly Subject<Unit> stopAllSFX = new();
        public Observable<Unit> StopAllMusic => stopAllMusic;
        private readonly Subject<Unit> stopAllMusic = new();
        
        #endregion
        
        private void Awake()
        {
            Instance = this;
            GameVolume.Subscribe(UpdateMusicPlayerVolume).AddTo(this);
        }

        #region Sound Effects

        public static void PlaySFX(SoundEffectType sound, float volumeMultiplier = 1, float pitchMult = 1)
        {
            if (Instance == null) return;
            
            Instance.PlaySoundEffect(sound, volumeMultiplier, pitchMult);
        }

        private void PlaySoundEffect(SoundEffectType clip, float volumeMultiplier = 1, float pitchMult = 1)
        {
            var SFX = audioStorage.GetSoundEffect(clip);
            var sfxPlayer = Instantiate(soundPlayerPrefab, transform);
            sfxPlayer.PlaySound(SFX.clip, stopAllSFX,SFX.volumeMultiplier * volumeMultiplier, pitchMult);
        }

        #endregion

        #region Music

        public static void PlayMusic(MusicTrackID track)
        {
            if (Instance == null) return;
            
            var newTrack = Instance.audioStorage.GetMusicTrack(track);
            
            if (Instance.currentMusicTrack.trackIDName == track)
            {
                Debug.LogError("Already playing track: " + track);
                return;
            }
            StopMusic();
            Instance.musicAudioSource.clip = newTrack.clip;
            Instance.currentMusicVolumeMultiplier = newTrack.volumeMultiplier;
            Instance.UpdateMusicPlayerVolume();
            Instance.musicAudioSource.loop = newTrack.doesLoop;

            Instance.musicPlayingDisposable.Clear();
            Instance.musicAudioSource.Play();
            Instance.StopAllMusic.Subscribe(Instance.StopCurrentSong).AddTo(Instance.musicPlayingDisposable);
            if (!newTrack.doesLoop) 
                Observable.Timer(TimeSpan.FromSeconds(newTrack.clip.length)).Subscribe(OnMusicEnded).AddTo(Instance.musicPlayingDisposable);

        }
        

        private void StopCurrentSong()
        {
            Instance.musicAudioSource.Stop();
            musicPlayingDisposable.Clear();
        }
        
        public static void StopMusic()
        {
            if (Instance == null) return;

            Instance.stopAllMusic.OnNext();
        }

        private static void OnMusicEnded() => Debug.LogWarning("On Music Ended.");

        #endregion

        #region Volumes

        public void NewMusicVolume(float newVolume) => musicVolume.Value = newVolume;
        public void NewSFXVolume(float newVolume) => sfxVolume.Value = newVolume;
        public void NewGameVolume(float newVolume) => gameVolume.Value = newVolume;
        private void UpdateMusicPlayerVolume() => 
            musicAudioSource.volume = 
                GameVolume.CurrentValue * 
                MusicVolume.CurrentValue * 
                currentMusicVolumeMultiplier;

        #endregion
        
        
    }