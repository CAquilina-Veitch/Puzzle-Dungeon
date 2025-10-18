using UnityEngine;

namespace Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private MusicTrackID track = MusicTrackID.None;
        [SerializeField] private float volumeMultiplier = 1;
        [SerializeField] private bool playOnEnable = false;

        private void OnEnable()
        {
            if (playOnEnable)
                PlayTrack(track);
        }

        public void PlayTrack(MusicTrackID trackID = MusicTrackID.None)
        {
            trackID = trackID is MusicTrackID.None ? track : trackID;
            AudioManager.PlayMusic(trackID);
        }

        public void StopTrack() => AudioManager.StopMusic();
    }
}