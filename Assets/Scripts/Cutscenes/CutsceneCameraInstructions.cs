using System;
using UnityEngine.Splines;

namespace Scripts.Cutscenes
{
    [Serializable]
    public class SpeedKeyframe
    {
        public float positionOnTrack;
        public float velocity;
        public float velocityAccelerationDuration; // Time in seconds to transition to the new speed
    }

    [Serializable]
    public class CutsceneCameraInstructions
    {
        public SplineContainer splineContainer; // Changed from Spline to SplineContainer
        public SpeedKeyframe[] speedKeyframes;
    }
}