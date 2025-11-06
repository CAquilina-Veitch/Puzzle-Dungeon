using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Cutscenes
{
    [CreateAssetMenu(fileName = "Cutscene", menuName = "ScriptableObjects/SO_CutsceneData", order = 1)]
    public class SO_CutsceneData : ScriptableObject
    {
        public CutsceneID CutsceneID => cutsceneID;
        [SerializeField] private CutsceneID cutsceneID;
        
        public IReadOnlyList<CutsceneStep> CutscenesSteps => cutscenesSteps;
        [SerializeField] private List<CutsceneStep> cutscenesSteps = new();
    }
    public enum CutsceneID
    {
        None,
        TestCutscene1,
        TestCutscene2,
    }
}