using System;
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

        public GameObject roomObject;
    }

    public class DungeonRoom : MonoBehaviour
    {
        [SerializeField] private DungeonRoomDefinition roomDefinition;
        
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