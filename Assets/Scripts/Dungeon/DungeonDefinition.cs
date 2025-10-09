using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

namespace Scripts.Dungeon
{
    public class DungeonDefinition : MonoBehaviour
    {
        public DungeonID DungeonID => dungeonID;
        private DungeonID dungeonID;
        
        [SerializeField] public DungeonRoomDefiner[] rooms = {};
        [SerializeField, HideInInspector] private List<DungeonRoomDefiner> lastRooms = new();

        private int numRooms;
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (numRooms != rooms.Length) 
                HandleRoomDifferences();
        }

        private void HandleRoomDifferences()
        {
            if (rooms.Length > numRooms)
            {
                var seen = new HashSet<DungeonRoomDefiner>();
                
                for (var i = 0; i < rooms.Length; i++)
                {
                    var room = rooms[i];
                    if (room == null || !seen.Add(room))
                    {
                        var obj = Instantiate(new GameObject(), transform);
                        var comp = obj.AddComponent<DungeonRoomDefiner>();
                        rooms[i] = comp;
                    }
                }
            }
            else
            {
                var removedRooms = lastRooms.Except(rooms).ToList();
                foreach (var room in removedRooms)
                    DestroyImmediate(room.gameObject);
            }
            
            lastRooms = rooms.ToList();
            numRooms = rooms.Length;
        }
        
        
        #endif
    }
}