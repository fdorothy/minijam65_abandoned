using UnityEngine;
using Delaunay.Geo;

public enum ConnectionType
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
}

public class RoomConnection
{
    public ConnectionType Direction
    {
        get;
        private set;
    }
    public DungeonRoom Room
    {
        get;
        private set;
    }

    public LineSegment Line1;
    public LineSegment Line2;

    public RoomConnection(DungeonRoom room, ConnectionType direction)
    {
        Room = room;
        Direction = direction;
    }
}
