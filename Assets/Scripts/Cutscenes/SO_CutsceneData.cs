using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Cutscenes
{
    [CreateAssetMenu(fileName = "Cutscene", menuName = "ScriptableObjects/SO_CutsceneData", order = 1)]
    public class SO_CutsceneData : ScriptableObject
    {
        public CutsceneID CutsceneID => cutsceneID;
        [SerializeField] private CutsceneID cutsceneID;
        
        public IReadOnlyList<CutsceneStepDefiner> CutscenesSteps => cutscenesSteps;
        [SerializeField] private List<CutsceneStepDefiner> cutscenesSteps = new();

        private void OnValidate()
        {
            #if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying) return;

            foreach (var stepDefiner in cutscenesSteps)
                stepDefiner?.UpdateStepType();
            #endif
        }
    }
    
    [Serializable]
    public enum CutsceneID
    {
        None = 0,
        MainMenuTest1 = 001,
        Level1Test = 101,
    }
}