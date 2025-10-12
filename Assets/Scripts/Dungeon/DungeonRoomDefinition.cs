using System;
using System.Linq;
using UnityEngine;

namespace Scripts.Dungeon
{
    [Serializable]
    public abstract class DungeonRoomDefinition
    {
        public void SetStartPosition(Vector2Int newStartPosition) => startPosition = newStartPosition;
        public Vector2Int StartPosition => startPosition;
        [SerializeField] private Vector2Int startPosition;
        public void SetShape(TileShape newShape) => shape = newShape;
        public void UpdateShapePosition() => Shape.SetPosition(StartPosition);
        public void SetPosition(Vector2Int newPosition) => Shape.SetPosition(newPosition);
        
        public TileShape Shape => shape;
        [SerializeField] private TileShape shape = new();
    }

    [Serializable]public class TileShape
    {
        public void SetPosition(Vector2Int newPosition)
        {
            Debug.LogWarning($"new pos {newPosition}");
            position = newPosition;
        }

        public Vector2Int Position => position;
        [SerializeField] private Vector2Int position;
        public Vector2Int[] ShapeCoordinates => shapeCoordinates;
        [SerializeField] private Vector2Int[] shapeCoordinates = { new(0, 0) };

        public TileShape()
        {
            
        }

        public TileShape(Vector2Int position, Vector2Int[] shapeCoordinates)
        {
            this.position = position;
            this.shapeCoordinates = shapeCoordinates;
        }

        public bool IsInCoordinates(Vector2Int posCheck) => CoordinatesAsPosition.Contains(posCheck);

        public Vector2Int[] CoordinatesAsPosition => ShapeCoordinates.Select(shapeCoordinate => shapeCoordinate + Position ).ToArray();
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