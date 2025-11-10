using System.Linq;
using Runtime.Extensions;
using Scripts.Behaviours;
using UnityEngine;

namespace Scripts.Cutscenes
{
    public class CutsceneManager : SingletonBehaviour<CutsceneManager>
    {
        [SerializeField] private SO_CutsceneData[]  cutscenes;
        
        public readonly RORP<SO_CutsceneData> CurrentActiveCutscene = new(null);

        public readonly RORP<CutsceneCameraInstructions> CutsceneCameraInstructions = new(null);

        public void TryPlayCutscene(CutsceneID newCutscene)
        {
            Debug.Log($"Playing Cutscene {newCutscene}");
            if (CurrentActiveCutscene.Get != null)
            {
                Debug.LogWarning($"Cutscene {CurrentActiveCutscene} was already playing!!!!");
                CurrentActiveCutscene.Set(null);
                //stop cutscene
            }

            var cutscene = cutscenes.FirstOrDefault(cs => cs.CutsceneID == newCutscene);
            if (cutscene != null) 
                CurrentActiveCutscene.NewValue = cutscene;
            else
                Debug.LogWarning($"Couldn't find cutscene {newCutscene}");
        }

        public void ReportFinishedCutscene()
        {
            
        }
    }
}