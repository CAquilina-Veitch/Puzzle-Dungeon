using System;
using UnityEngine;

namespace Scripts.Cutscenes
{
    [Serializable]
    public class CutsceneStepDefiner
    {
        [Serializable]public enum StepType
        {
            None,
            CameraMove,
            WaitForUser,
        }
        public CutsceneStep Step => step;
        private CutsceneStep step;

        [SerializeField] private StepType stepType;
        private StepType cachedStepType;
        
        
        private void OnValidate()
        {
            if (stepType != cachedStepType)
            {
                cachedStepType = stepType;
                switch (stepType)
                {
                    case StepType.None:
                        step = null;
                        break;
                    case StepType.CameraMove:
                        step = new CameraMoveCutsceneStep();
                        break;
                    case StepType.WaitForUser:
                        step = new WaitForUserCutsceneStep();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    [Serializable]
    public class CutsceneStep
    {
        
    }
    
    [Serializable]
    public class CameraMoveCutsceneStep : CutsceneStep
    {
        [SerializeField] private string CameraTest;
    }
    
    [Serializable]
    public class WaitForUserCutsceneStep : CutsceneStep
    {
        [SerializeField] private string WaitTest;

    }
    
}