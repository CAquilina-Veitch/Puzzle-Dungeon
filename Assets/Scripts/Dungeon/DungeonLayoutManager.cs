using System.Collections.Generic;
using System.Linq;
using R3;
using Scripts.Behaviours;
using UnityEngine;

namespace Scripts.Dungeon
{
    public class DungeonLayoutManager : SingletonBehaviour<DungeonLayoutManager>
    {
        [SerializeField] private List<DungeonDefinition> dungeonDefinitions = new();

        private DungeonDefinition currentDungeonDefinition;
        
        public ReadOnlyReactiveProperty<DungeonDefinition> CurrentDungeon => currentDungeon;
        private readonly ReactiveProperty<DungeonDefinition> currentDungeon = new();
        
        public void LoadDungeon(DungeonID newID)
        {
            if (currentDungeon.CurrentValue.DungeonID != DungeonID.None)
            {
                Debug.LogWarning($"Tried to load new dungeon {newID} while {currentDungeon.CurrentValue} is still loaded. ");
                return;
            }
            
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
            Destroy(currentDungeonDefinition.gameObject);
        }
    }



    public enum DungeonID
    {
        None = 0,
        Dungeon1 = 1,
        Dungeon2 = 2,
        Dungeon3 = 3,
        Dungeon4 = 4,
    }
}