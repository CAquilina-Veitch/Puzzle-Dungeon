using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLayoutManager : MonoBehaviour
{
    [SerializeField] private List<DungeonLayoutDefinition> standardRooms = new();
}

[CreateAssetMenu(menuName = "Dungeon/Dungeon Layout")]
public class DungeonLayoutDefinition : ScriptableObject
{
    [SerializeReference]
    public List<DungeonRoomDefinition> rooms = new();
}

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