using System;
using UnityEngine;

namespace Scripts.Dungeon
{
    public class DungeonRoomDefiner : MonoBehaviour
    {
        [SerializeField] private RoomType roomType;
        private RoomType lastSelectedRoomType = RoomType.None;
        
        [SerializeReference] private DungeonRoomDefinition definition;

        private void Reset()
        {
            transform.name = roomType.ToString();
        }

        private void OnValidate()
        {
            if (roomType == lastSelectedRoomType) return;
            
            /*if(definition != null)
                Destroy(definition);*/
            definition = DungeonRoomDefinitionLibrary.GetDef(roomType);
            lastSelectedRoomType = roomType;
            transform.name = roomType.ToString();
        }
    }
}