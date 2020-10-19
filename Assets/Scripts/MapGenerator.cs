using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    Transform[] pieces;
    public Transform player;
    public Transform floorPrefab;
    public Transform lavaPrefab;
    public Transform wallPrefab;
    public Room roomPrefab;
    public Room hallwayPrefab;
    public Pickup pickupPrefab;
    public Goal goalPrefab;
    public DungeonGenerator dungeonGenerator;

    public Goal goal;

    public System.Action OnReady;
    
    private static readonly Random random = new Random();
    public int[] floor;

    public void PopulateMap()
    {
        Game.singleton.totalCoins = 0;
        Game.singleton.coins = 0;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        dungeonGenerator.OnDungeonGenerated += OnDungeonGenerated;
        dungeonGenerator.Init();
    }

    void OnDungeonGenerated()
    {
        CreateRooms();
        FillRooms();
        OnReady?.Invoke();
    }

    public T RandomValue<T>(HashSet<T> bag) => bag.ElementAt(Random.Range(0, bag.Count));

    void FillRooms()
    {
        List<DungeonRoom> rooms = dungeonGenerator.Rooms;
        rooms.ForEach(room =>
        {
            if (room.IsVisible) FillRoom(room);
        });
    }

    void FillRoom(DungeonRoom room)
    {
        if (room.IsEndRoom)
        {
            Debug.Log("Found starting room");
            goal = Instantiate<Transform>(goalPrefab.transform, transform).GetComponent<Goal>();
            PlaceInRoomCenter(room, goal.transform);
        }
        else if (room.IsStartRoom)
        {

        }
        else
        {
            PlaceInRoomCenter(room, Instantiate<Transform>(pickupPrefab.transform, transform));
        }
    }

    public void PlaceInRoomCenter(DungeonRoom room, Transform t)
    {
        t.position = ToWorld(room.Center);
    }

    void CreateRooms()
    {
        int[,] grid = dungeonGenerator.Grid;
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Go through each room and place traps in some of them
        dungeonGenerator.Rooms.ForEach(room =>
        {
            if (room.IsVisible)
            {
                if (room.IsStartRoom)
                {

                }
                else if (room.IsEndRoom)
                {

                } else
                {
                    if (Random.value > 0.5f)
                    {
                        FillWithLava(room);
                    }
                }
            }
        });

        // build the floor
        MeshBuilder builder = new MeshBuilder();
        MeshBuilder lavaBuilder = new MeshBuilder();
        int x, y;
        for (x = 0; x < width; x++)
        {
            for (y=0; y<height; y++)
            {
                if (grid[x, y] > 0) {
                    if (grid[x, y] == (int)TileType.Lava)
                    {
                        AddFloorTile(lavaBuilder, x, y);
                    }
                    else
                    {
                        AddFloorTile(builder, x, y);
                    }
                }
            }
        }
        Transform t = Instantiate(floorPrefab);
        Mesh mesh = builder.Build();
        t.GetComponent<MeshFilter>().mesh = mesh;
        t.GetComponent<MeshCollider>().sharedMesh = mesh;

        t = Instantiate(lavaPrefab);
        mesh = lavaBuilder.Build();
        t.GetComponent<MeshFilter>().mesh = mesh;
        t.GetComponent<MeshCollider>().sharedMesh = mesh;

        // build the walls
        MeshBuilder wall = new MeshBuilder();
        for (x = -1; x < width; x++)
        {
            for (y = -1; y < height; y++)
            {
                bool v0 = false;
                if (x >= 0 && y >= 0)
                    v0 = grid[x, y] != 0;
                bool v1 = false;
                if (x+1 >= 0 && y >= 0 && x+1 < width)
                    v1 = grid[x + 1, y] != 0;
                bool v2 = false;
                if (x >= 0 && y+1 >= 0 && y + 1 < height)
                    v2 = grid[x, y + 1] != 0;
                if (v0 && !v1)
                    AddWall(wall, x + 1, y + 1, x + 1, y);
                if (!v0 && v1)
                    AddWall(wall, x + 1, y, x + 1, y + 1);
                if (v0 && !v2)
                    AddWall(wall, x, y + 1, x + 1, y + 1);
                if (!v0 && v2)
                    AddWall(wall, x+1, y + 1, x, y + 1);
            }
        }

        // lava walls
        for (x = -1; x < width; x++)
        {
            for (y = -1; y < height; y++)
            {
                bool v0 = false;
                if (x >= 0 && y >= 0)
                    v0 = grid[x, y] == (int)TileType.Lava;
                bool v1 = false;
                if (x + 1 >= 0 && y >= 0 && x + 1 < width)
                    v1 = grid[x + 1, y] == (int)TileType.Lava;
                bool v2 = false;
                if (x >= 0 && y + 1 >= 0 && y + 1 < height)
                    v2 = grid[x, y + 1] == (int)TileType.Lava;
                if (v0 && !v1)
                    AddWall(wall, x + 1, y + 1, x + 1, y, true);
                if (!v0 && v1)
                    AddWall(wall, x + 1, y, x + 1, y + 1, true);
                if (v0 && !v2)
                    AddWall(wall, x, y + 1, x + 1, y + 1, true);
                if (!v0 && v2)
                    AddWall(wall, x + 1, y + 1, x, y + 1, true);
            }
        }

        t = Instantiate(wallPrefab);
        mesh = wall.Build();
        t.GetComponent<MeshFilter>().mesh = mesh;
        t.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void FillWithLava(DungeonRoom room)
    {
        int[,] grid = dungeonGenerator.Grid;
        for (int x = (int)room.TopLeft.x + 2; x < (int)room.BottomRight.x - 2; x++)
        {
            for (int y = (int)room.BottomRight.y + 2; y < (int)room.TopLeft.y - 2; y++)
            {
                int a = x - (int)dungeonGenerator.RoomGenerator.XMin;
                int b = y - (int)dungeonGenerator.RoomGenerator.YMin;
                grid[a, b] = (int)TileType.Lava;
            }
        }
    }

    void AddFloorTile(MeshBuilder builder, int x, int y)
    {
        float x2 = x + 1.0f;
        float y2 = y + 1.0f;
        builder.AddQuad(
            new Vector3(x, 0.0f, y),
            new Vector3(x2, 0.0f, y),
            new Vector3(x2, 0.0f, y2),
            new Vector3(x, 0.0f, y2)
        );
    }

    void AddWall(MeshBuilder builder, int x0, int y0, int x1, int y1, bool lavaWall = false)
    {
        float height = lavaWall ? 0.25f : 20.0f;
        float yOffset = lavaWall ? -0.25f : 0.0f;
        builder.AddQuad(
            new Vector3(x0, yOffset, y0),
            new Vector3(x1, yOffset, y1),
            new Vector3(x1, yOffset+height, y1),
            new Vector3(x0, yOffset+height, y0)
        );
    }

    public Vector3 ToWorld(Vector2 pt) => dungeonGenerator.ToWorld(pt);
}
