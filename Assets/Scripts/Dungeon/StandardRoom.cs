using System;

[Serializable]
public class StandardRoom : DungeonRoom
{
    public override RoomType RoomType => RoomType.StandardRoom;
}

public class LockedRoom : DungeonRoom
{
    public override RoomType RoomType => RoomType.LockedRoom;
}