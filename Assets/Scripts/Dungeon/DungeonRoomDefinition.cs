using System;
using System.Linq;
using UnityEngine;

namespace Scripts.Dungeon
{
    [Serializable]
    public abstract class DungeonRoomDefinition
    {
        public void UpdateShapePosition() => Shape.Position = StartPosition;
        public Vector2Int StartPosition;
        public TileShape Shape = new();
    }

    [Serializable]public class TileShape
    {
        public Vector2Int Position;
        public Vector2Int[] ShapeCoordinates = { new(0, 0) };

        public bool IsInCoordinates(Vector2Int posCheck) => CoordinatesAsPosition.Contains(posCheck);

        public Vector2Int[] CoordinatesAsPosition => ShapeCoordinates.Select(shapeCoordinate => shapeCoordinate + Position).ToArray();
    }
    
    [Serializable]
    public class StandardRoomDefinition : DungeonRoomDefinition
    {
        //default
    }
    [Serializable]
    public class EmptyRoomDefinition : DungeonRoomDefinition
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