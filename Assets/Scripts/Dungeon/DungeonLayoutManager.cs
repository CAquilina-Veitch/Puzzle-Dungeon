using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using Scripts.Behaviours;
using UnityEditor;
using UnityEngine;

namespace Scripts.Dungeon
{
    public class DungeonLayoutManager : SingletonBehaviour<DungeonLayoutManager>
    {
        [SerializeField] private DungeonID testingDungeonID;
        [SerializeField, HideInInspector] private DungeonID currentTestingDungeonID;
        
        [SerializeField] private List<DungeonDefinition> dungeonDefinitions = new();

        private DungeonDefinition currentDungeonDefinition;
        
        public ReadOnlyReactiveProperty<DungeonDefinition> CurrentDungeon => currentDungeon;
        private readonly ReactiveProperty<DungeonDefinition> currentDungeon = new();

        private void OnValidate()
        {
            if (testingDungeonID != currentTestingDungeonID) 
                TestingLoadDungeon(testingDungeonID);
        }

        private void TestingLoadDungeon(DungeonID id)
        {
            if (currentTestingDungeonID != DungeonID.None && id != DungeonID.None)
                Debug.LogWarning($"Tried to load new dungeon {id} while {currentTestingDungeonID} is still loaded. ");

            if (id is DungeonID.None || dungeonDefinitions.Any(def => def.DungeonID == id))
            {
                if (currentDungeonDefinition != null)
                    UnloadDungeon();

                var definition = dungeonDefinitions.FirstOrDefault(def => def.DungeonID == id);

                if(definition != null)
                    currentDungeonDefinition = (DungeonDefinition)PrefabUtility.InstantiatePrefab(definition, transform);
            }

            currentTestingDungeonID = testingDungeonID = id;
        }

        public void LoadDungeon(DungeonID newID)
        {
            if (currentDungeon.CurrentValue.DungeonID != DungeonID.None && newID != DungeonID.None) 
                Debug.LogWarning($"Tried to load new dungeon {newID} while {currentDungeon.CurrentValue} is still loaded. ");
            
            if (newID is DungeonID.None || dungeonDefinitions.Any(def => def.DungeonID == newID))
            {
                if (currentDungeonDefinition != null) 
                    UnloadDungeon();
                
                var definition = dungeonDefinitions.FirstOrDefault(def => def.DungeonID == newID);
                
                if(definition != null)
                    currentDungeonDefinition = Instantiate(definition, transform);
            } 
            currentDungeon.Value = currentDungeonDefinition;
        }
        
        private void UnloadDungeon()
        {
            Debug.Log($"Unloading dungeon {currentDungeonDefinition.DungeonID}");

            var toDestroy = currentDungeonDefinition.gameObject;
            currentDungeonDefinition = null;

            if (!EditorApplication.isPlaying)
                Observable.TimerFrame(1).Subscribe(_ => DestroyImmediate(toDestroy));
            else
                Destroy(toDestroy);
        }
    }



    [Serializable]
    public enum DungeonID
    {
        None = 0,
        Dungeon1 = 1,
        Dungeon2 = 2,
        Dungeon3 = 3,
        Dungeon4 = 4,
    }
}