using System;
using UnityEngine;

[Serializable]
public class StandardRoom : DungeonRoom
{
    public override RoomType RoomType => RoomType.StandardRoom;
}

[Serializable]
public class LockedRoom : DungeonRoom
{
    public override RoomType RoomType => RoomType.LockedRoom;

    [SerializeField] private bool isLocked;
    public bool IsLocked
    {
        get => isLocked;
        set => isLocked = value;
    }
}