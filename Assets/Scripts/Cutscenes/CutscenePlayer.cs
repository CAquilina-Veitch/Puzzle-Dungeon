using R3;
using UnityEngine;

namespace Scripts.Cutscenes
{
    public class CutscenePlayer : MonoBehaviour
    {
        private SO_CutsceneData cutscene;
        private int currentStep;
        
        private void Awake()
        {
            CutsceneManager.Instance.CurrentActiveCutscene.Subscribe(OnNewCutscene).AddTo(this);
            CutsceneManager.Instance.IsCutscenePlaying.Subscribe(OnCutscenePlaying).AddTo(this);
        }

        private void OnNewCutscene(SO_CutsceneData newCutscene)
        {
            currentStep = 0;
        }

        private void OnCutscenePlaying(bool newIsPlaying)
        {
            if (newIsPlaying)
            {
                if (currentStep != 0)
                {
                    Debug.LogWarning("Cutscene has already been played!");
                }
            }
            else
            {
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
    }
}