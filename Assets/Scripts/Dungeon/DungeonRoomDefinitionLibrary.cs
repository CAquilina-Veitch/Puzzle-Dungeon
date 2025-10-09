using System;
using Scripts.Dungeon;

public static class DungeonRoomDefinitionLibrary
{
    public static DungeonRoomDefinition GetDef(RoomType type)
    {
        switch (type)
        {
            case RoomType.None:
                return null;
            case RoomType.StandardRoom:
                return new StandardRoomDefinition();
            case RoomType.LockedRoom:
                return new LockedRoomDefinition();
            default:
                return null;
        }
    }
}