using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

namespace Scripts.Dungeon
{
    public class DungeonLayoutDefinition : MonoBehaviour
    {
        [SerializeReference]
        public DungeonRoomDefiner[] rooms = {};
        [SerializeField,HideInInspector] private List<DungeonRoomDefiner> lastRooms = new List<DungeonRoomDefiner>();

        private int numRooms;
        private void OnValidate()
        {
            if (numRooms == rooms.Length) return;
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
                    Observable.TimerFrame(1).Subscribe(_=>DestroyImmediate(room.gameObject)).AddTo(this);
            }
            
            lastRooms = rooms.ToList();
            numRooms = rooms.Length;
        }
    }
}