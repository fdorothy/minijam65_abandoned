using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public void Build(DungeonRoom room)
    {
        BuildCollider(room);

        // create the floor
        MeshBuilder meshBuilder = new MeshBuilder();
        Vector3 tl = room.TopLeft;
        Vector3 br = room.BottomRight;
        meshBuilder.AddQuad(
            new Vector3(tl.x, 0.0f, br.y),
            new Vector3(br.x, 0.0f, br.y),
            new Vector3(br.x, 0.0f, tl.y),
            new Vector3(tl.x, 0.0f, tl.y)
        );

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = meshBuilder.Build();
    }

    public void BuildCollider(DungeonRoom room)
    {
        // create the floor
        MeshBuilder meshBuilder = new MeshBuilder();
        Vector3 tl = room.TopLeft;
        Vector3 br = room.BottomRight;
        Debug.Log("coords");
        Debug.Log(tl);
        Debug.Log(br);
        meshBuilder.AddQuad(
            new Vector3(tl.x, 0.0f, br.y),
            new Vector3(br.x, 0.0f, br.y),
            new Vector3(br.x, 0.0f, tl.y),
            new Vector3(tl.x, 0.0f, tl.y)
        );
        Mesh mesh = meshBuilder.Build();

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}
