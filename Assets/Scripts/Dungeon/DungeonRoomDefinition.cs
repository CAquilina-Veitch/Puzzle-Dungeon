using System;
using UnityEngine;

namespace Scripts.Dungeon
{
    [Serializable]
    public abstract class DungeonRoomDefinition
    {
        public Vector2Int StartPosition;
        public Vector2Int[] ShapeCoordinates;
    }
    [Serializable]
    public class StandardRoomDefinition : DungeonRoomDefinition
    {
        //default
    }

    [Serializable]
    public class LockedRoomDefinition : DungeonRoomDefinition
    {
        public bool StartsLocked = true;
        public int RequiredKeyCount = 1;
    }
    public enum RoomType
    {
        None = 0,
        StandardRoom,
        LockedRoom,
    }
}