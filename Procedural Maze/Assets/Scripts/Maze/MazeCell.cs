using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{

    public IntVector2 Coordinates;
    public MazeRoom Room; //< The cell is in this room
    public Renderer Floor;
    public GameObject LightPrefab;
    public Transform LightParent;

    public bool HasLight = false;

    public MazeCellEdge GetEdge(MazeDirection direction)
    {
        return edges[(int)direction];
    }

    public bool IsFullyInitialized
    {
        get
        {
            return initializedEdgeCount == MazeDirections.Count;
        }
    }

    public MazeDirection RandomUninitializedDirection
    {
        get
        {
            int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
            for (int i = 0; i < MazeDirections.Count; i++)
            {
                if (edges[i] != null) continue;
                if (skips == 0)
                {
                    return (MazeDirection)i;
                }
                skips -= 1;
            }
            throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
        }
    }

    public MazeDirection RandomUninitializedDirectionFrom(System.Random generator)
    {
        int skips = generator.Next(0, MazeDirections.Count - initializedEdgeCount);
        for (int i = 0; i < MazeDirections.Count; i++)
        {
            if (edges[i] == null)
            {
                if (skips == 0)
                {
                    return (MazeDirection)i;
                }
                skips -= 1;
            }
        }
        throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
    }

    public void SetEdge(MazeDirection direction, MazeCellEdge edge)
    {
        edges[(int)direction] = edge;
        initializedEdgeCount += 1;
    }

    public void Initialize(MazeRoom room)
    {
        room.Add(this);
        Floor.material = room.settings.FloorMaterial;
        if (HasLight)
        {
            Instantiate(LightPrefab, LightParent);
        }
    }

    public bool HasFreeCellEdges()
    {
        foreach (MazeCellEdge edge in edges)
        {
            if (!(edge is MazePassage))
            {
                return false;
            }
        }
        return true;
    }

    private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];
    private int initializedEdgeCount;
}
