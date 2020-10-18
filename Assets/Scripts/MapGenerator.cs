using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    Transform[] pieces;
    public Transform player;
    public Transform floorPrefab;
    public Transform wallPrefab;
    public Room roomPrefab;
    public Room hallwayPrefab;
    public Pickup pickupPrefab;
    public Goal goalPrefab;
    public DungeonGenerator dungeonGenerator;

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
            PlaceInRoomCenter(room, Instantiate<Transform>(goalPrefab.transform, transform));
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

        // build the floor
        MeshBuilder builder = new MeshBuilder();
        int x, y;
        for (x = 0; x < width; x++)
        {
            for (y=0; y<height; y++)
            {
                if (grid[x, y] > 0) {
                    AddFloorTile(builder, x, y);
                }
            }
        }
        Transform t = Instantiate(floorPrefab);
        Mesh mesh = builder.Build();
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
        t = Instantiate(wallPrefab);
        mesh = wall.Build();
        t.GetComponent<MeshFilter>().mesh = mesh;
        t.GetComponent<MeshCollider>().sharedMesh = mesh;
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

    void AddWall(MeshBuilder builder, int x0, int y0, int x1, int y1)
    {
        const float height = 20.0f;
        builder.AddQuad(
            new Vector3(x0, 0.0f, y0),
            new Vector3(x1, 0.0f, y1),
            new Vector3(x1, height, y1),
            new Vector3(x0, height, y0)
        );
    }

    public Vector3 ToWorld(Vector2 pt) => dungeonGenerator.ToWorld(pt);
}
