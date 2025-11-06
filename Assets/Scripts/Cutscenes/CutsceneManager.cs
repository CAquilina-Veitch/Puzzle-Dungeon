using System;
using System.Linq;
using Runtime.Extensions;
using Scripts.Behaviours;
using UnityEngine;

namespace Scripts.Cutscenes
{
    public class CutsceneManager : SingletonBehaviour<CutsceneManager>
    {
        [SerializeField] private SO_CutsceneData[]  cutscenes;
        
        public readonly RORP<SO_CutsceneData> CurrentActiveCutscene = null;
        public readonly RORP<bool> IsCutscenePlaying;

        public void TryPlayCutscene(CutsceneID newCutscene)
        {
            if (IsCutscenePlaying)
            {
                Debug.LogWarning($"Cutscene {CurrentActiveCutscene} was already playing!!!!");
                IsCutscenePlaying.NewValue = false;
                //stop cutscene
            }

            var cutscene = cutscenes.FirstOrDefault(cs => cs.CutsceneID == newCutscene);
            if (cutscene != null)
            {
                CurrentActiveCutscene.NewValue = cutscene;
            }
            
        }

        public void ReportFinishedCutscene()
        {
            
        }
    }
}