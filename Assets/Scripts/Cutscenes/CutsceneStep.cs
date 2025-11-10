using System;
using R3;
using UnityEngine;
using UnityEngine.Splines;

namespace Scripts.Cutscenes
{
    [Serializable]
    public abstract class CutsceneStep
    {
        [SerializeField] private string stepName = "Step";

        public abstract Observable<Unit> OnComplete { get; }
        public abstract void StartStep();
    }
    
    [Serializable]
    public class CutsceneStepCameraMove : CutsceneStep
    {
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private SpeedKeyframe[] speedKeyframes;

        public override Observable<Unit> OnComplete =>
            CutsceneManager.Instance.CutsceneCameraInstructions
                .RP
                .Skip(1)
                .Select(inst => inst == null)
                .Where(isNull => isNull)
                .Select(x => Unit.Default);

        public override void StartStep()
        {
            var instructions = new CutsceneCameraInstructions
            {
                splineContainer = splineContainer,
                speedKeyframes = speedKeyframes
            };
            var instructionRORP = CutsceneManager.Instance.CutsceneCameraInstructions;
            instructionRORP.Set(instructions);
        }
    }
    
    [Serializable]
    public class CutsceneStepWaitForUser : CutsceneStep
    {
        public override Observable<Unit> OnComplete { get; }

        public override void StartStep()
        {
            throw new NotImplementedException();
        }
    }
    
    [Serializable]
    public class CutsceneStepDialogue : CutsceneStep
    {
        public override Observable<Unit> OnComplete { get; }

        public override void StartStep()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class CutsceneStepLoadScene : CutsceneStep
    {
        [SerializeField] public SceneID id;
        public override Observable<Unit> OnComplete => SceneManager.Instance.CurrentScene.RP.Select(x => Unit.Default);

        public override void StartStep()
        {
            SceneManager.Instance.LoadScene(id);
        }
        
    }
}