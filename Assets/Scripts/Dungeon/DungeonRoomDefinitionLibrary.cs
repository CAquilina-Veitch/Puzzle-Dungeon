using System;

public static class DungeonRoomDefinitionLibrary
{
    public static Type GetDef(RoomType type)
    {
        switch (type)
        {
            case RoomType.None:
                return null;
            case RoomType.StandardRoom:
                return typeof(StandardRoom);
            case RoomType.LockedRoom:
                return typeof(LockedRoom);
            default:
                return null;
        }
    }
}