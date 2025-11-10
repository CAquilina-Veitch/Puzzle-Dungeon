using R3;
using UnityEngine;

namespace Scripts.Cutscenes
{
    public class CutscenePlayer : MonoBehaviour
    {
        [SerializeField] private SO_CutsceneData currentCutscene;
        [SerializeField] private int currentStep;
        
        private void Start()
        {
            CutsceneManager.Instance.CurrentActiveCutscene.Subscribe(OnNewCutscene).AddTo(this);
        }

        private void OnNewCutscene(SO_CutsceneData newCutscene)
        {
            currentStep = 0;
            currentCutscene = newCutscene;

            var newIsPlaying = newCutscene != null;
            
            if (newIsPlaying)
            {
                Debug.Log($"CutscenePlayer - Playing new {newCutscene.CutsceneID}");

                ExecuteCutsceneStep();
            }
            else
            {
                Debug.LogWarning("Clearing cutscene.");
                CurrentCutsceneStepDisposable.Clear();
                if (currentStep == 0)
                {
                    
                }
                else
                {
                    currentStep = 0;
                    //stop all disposables.
                }
            }
        }

        private void ExecuteCutsceneStep()
        {
            if (currentCutscene == null)
            {
                Debug.LogError("CurrentCutscene is null");
                return;
            }
            
            if (currentStep >= currentCutscene.CutscenesSteps.Count)
            {
                Debug.LogWarning($"Reached end of cutscene steps. {currentStep} / {currentCutscene.CutscenesSteps.Count}");
                return;
            }
            var nextStep = currentCutscene.CutscenesSteps[currentStep];
            Debug.Log($"playing Step {currentStep}, && {nextStep.Type}");

            nextStep.TriggerNextStep(this);
        }

        public void OnStepFinished()
        {
            Debug.Log("StepOncompleteCalled");
            CurrentCutsceneStepDisposable.Clear();
            currentStep++;
            ExecuteCutsceneStep();
        }

        public CompositeDisposable CurrentCutsceneStepDisposable { get; } = new();
    }
}