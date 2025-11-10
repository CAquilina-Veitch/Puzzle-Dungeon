using R3;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

namespace Scripts.Cutscenes
{
    public class CutsceneCameraPlayer : MonoBehaviour
    {
        [SerializeField] private CinemachineSplineDolly dolly;
        [SerializeField] private CinemachineCamera cmCamera; // The camera component
        [SerializeField] private SplineContainer testSplineContainer; // For testing

        private CutsceneCameraInstructions currentInstructions;
        private int currentKeyframeIndex = 0;
        private float targetVelocity;
        private float currentVelocity;
        private float velocityTransitionStartTime;
        private float velocityTransitionDuration;
        private float initialVelocity;

        private void Start()
        {
            CutsceneManager.Instance.CutsceneCameraInstructions.Subscribe(OnCameraInstructionsChanged).AddTo(this);
        }

        private void OnCameraInstructionsChanged(CutsceneCameraInstructions newInstructions)
        {
            currentInstructions = newInstructions;
            if (currentInstructions == null)
            {
                Debug.LogWarning("New camera instructions was null - ending cutscene camera");
                // Disable the camera when cutscene ends
                if (cmCamera != null)
                {
                    cmCamera.enabled = false;
                    Debug.Log("Disabled cutscene camera");
                }
                return;
            }
            Debug.Log("Starting Camera Instructions - TESTING WITHOUT SWITCHING SPLINE");

            // Just use the speed, don't switch spline at all
            currentKeyframeIndex = 0;

            // Set initial velocity from first keyframe if available
            if (currentInstructions.speedKeyframes != null && currentInstructions.speedKeyframes.Length > 0)
            {
                currentVelocity = currentInstructions.speedKeyframes[0].velocity;
                Debug.Log($"Setting speed from keyframe: {currentVelocity}");
                SetDollySpeed(currentVelocity);
            }
            else
            {
                Debug.Log("No keyframes, setting speed to 1");
                SetDollySpeed(1);
            }

            // IMPORTANT: Set up dolly BEFORE enabling camera
            // Otherwise Cinemachine will update the dolly position immediately when camera is enabled
            dolly.AutomaticDolly.Enabled = false;
            dolly.CameraPosition = 0;
            Debug.Log($"Reset dolly position to: {dolly.CameraPosition}");

            // Activate the camera so it's being used by Cinemachine
            if (cmCamera != null)
            {
                cmCamera.enabled = true;
                cmCamera.Priority = 100; // High priority to ensure it's active
                Debug.Log($"Activated camera with priority {cmCamera.Priority}");
            }
            else
            {
                Debug.LogError("cmCamera is null! Assign it in the inspector!");
            }

            // Now enable AutomaticDolly
            dolly.AutomaticDolly.Enabled = true;
            Debug.Log($"Dolly position after enabling AutoDolly: {dolly.CameraPosition}");

            Debug.Log($"Dolly enabled: {dolly.AutomaticDolly.Enabled}, speed method: {dolly.AutomaticDolly.Method}");
            Debug.Log($"Dolly spline: {dolly.Spline}");
            Debug.Log($"Spline knot count: {dolly.Spline?.Spline?.Count ?? -1}");
            Debug.Log($"This GameObject active: {gameObject.activeInHierarchy}");
        }

        private void Update()
        {
            if (currentInstructions == null)
                return;

            float currentPosition = dolly.CameraPosition;

            // Debug every 60 frames
            if (Time.frameCount % 60 == 0)
            {
                var fixedSpeed = dolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
                Debug.Log($"Dolly position: {currentPosition}, AutoDolly enabled: {dolly.AutomaticDolly.Enabled}, Speed: {fixedSpeed?.Speed}");
            }

            // Check if dolly has reached the end of the spline
            float splineLength = dolly.Spline.Spline.GetLength();
            if (currentPosition >= splineLength)
            {
                Debug.Log($"Dolly reached end at position {currentPosition} / {splineLength}");
                CutsceneManager.Instance.CutsceneCameraInstructions.Set(null);
                return;
            }

            if (currentInstructions.speedKeyframes == null || currentInstructions.speedKeyframes.Length == 0)
                return;

            // Check if we've reached the next keyframe
            if (currentKeyframeIndex < currentInstructions.speedKeyframes.Length - 1)
            {
                var nextKeyframe = currentInstructions.speedKeyframes[currentKeyframeIndex + 1];

                if (currentPosition >= nextKeyframe.positionOnTrack)
                {
                    currentKeyframeIndex++;
                    StartVelocityTransition(nextKeyframe.velocity, nextKeyframe.velocityAccelerationDuration);
                }
            }

            // Handle velocity transition
            if (velocityTransitionDuration > 0)
            {
                float elapsed = Time.time - velocityTransitionStartTime;
                float t = Mathf.Clamp01(elapsed / velocityTransitionDuration);

                currentVelocity = Mathf.Lerp(initialVelocity, targetVelocity, t);
                SetDollySpeed(currentVelocity);

                if (t >= 1f)
                    velocityTransitionDuration = 0;
            }
        }

        private void StartVelocityTransition(float newVelocity, float duration)
        {
            targetVelocity = newVelocity;
            initialVelocity = currentVelocity;
            velocityTransitionStartTime = Time.time;
            velocityTransitionDuration = duration;
        }

        private void SetDollySpeed(float speed)
        {
            if (dolly.AutomaticDolly.Method is SplineAutoDolly.FixedSpeed fixedSpeed)
            {
                fixedSpeed.Speed = speed;
                Debug.Log($"Set dolly speed to: {speed}");
            }
            else
            {
                Debug.LogError($"AutomaticDolly.Method is not FixedSpeed, it's: {dolly.AutomaticDolly.Method?.GetType().Name}");
            }
        }
    }
}