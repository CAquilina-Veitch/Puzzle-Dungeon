using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLayoutManager : MonoBehaviour
{
    [SerializeField] private List<DungeonLayoutDefinition> standardRooms = new();
}
[CreateAssetMenu][Serializable]
public class DungeonLayoutDefinition : ScriptableObject
{
    public List<DungeonRoomDefinition> rooms;
}

[Serializable]
public class DungeonRoomDefinition : MonoBehaviour
{
    public RoomType RoomType;
    public DungeonRoom DungeonRoomComponent;

    private RoomType currentRoomTypeComponent;
    
    
    private void OnValidate()
    {
        if (RoomType == currentRoomTypeComponent) return;
        
        if (DungeonRoomComponent != null)
            Destroy(DungeonRoomComponent);

        var componentType = DungeonRoomDefinitionLibrary.GetDef(RoomType);
        if (componentType != null)
        {
            DungeonRoomComponent = (DungeonRoom)gameObject.AddComponent(componentType);
            currentRoomTypeComponent = RoomType;
        }
    }
}


public enum RoomType
{
    None = 0,
    StandardRoom,
    LockedRoom,
}