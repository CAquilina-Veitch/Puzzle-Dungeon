using System;
using R3;
using Runtime.Extensions;
using UnityEngine;

namespace Scripts.Cutscenes
{
    [Serializable]
    public class CutsceneStepDefiner
    {
        public CutsceneStepType Type => cutsceneStepType;
        [SerializeField] private CutsceneStepType cutsceneStepType;
        [SerializeField] private CutsceneStepType cachedCutsceneStepType;
        
        [SerializeReference] public CutsceneStep step;
        
        public void UpdateStepType()
        {
            if (cutsceneStepType == cachedCutsceneStepType) 
                return;
            cachedCutsceneStepType = cutsceneStepType;
            
            switch (cutsceneStepType)
            {
                case CutsceneStepType.None:
                    step = null;
                    break;
                case CutsceneStepType.CameraMove:
                    step = new CutsceneStepCameraMove();
                    break;
                case CutsceneStepType.WaitForUser:
                    step = new CutsceneStepWaitForUser();
                    break;
                case CutsceneStepType.Dialogue:
                    step = new CutsceneStepDialogue();
                    break;
                case CutsceneStepType.LoadScene:
                    step = new CutsceneStepLoadScene();
                    break;
                default:
                    Debug.LogError($"CUTSCENE STEP {step} NOT IMPLEMENTED!!");
                    return;
            }
        }
        
        public void TriggerNextStep(CutscenePlayer player)
        {
            step.OnComplete.Subscribe(player.OnStepFinished).AddTo(player.CurrentCutsceneStepDisposable);
            step.StartStep();
        }
    }
}