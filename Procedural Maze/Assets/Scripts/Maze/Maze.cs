using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Maze : MonoBehaviour {
    
    [Header("Generation attributes")]
    [Tooltip("The size of the maze grid.")]
    public IntVector2 Size;   
    [Tooltip("Probility of creating a light on a cell.")]
    [Range(0.0f, 1.0f)]
    public float LightProbability;  
    [Tooltip("Probability of creating a door.")]
    [Range(0.0f, 1.0f)]
    public float DoorProbability;
    public int NumberFractals;

    [Header("Generation constraints")]
    public int DistanceBetweenLights;
    //public int MinNumberOfCellsInRoom; TODO

    [Header("Prefabs")]
    [Tooltip("Prefab to apply on a basic cell.")]
    public MazeCell CellPrefab;
    public MazeCell[] FractalPrefabs;
    [Tooltip("Prefabs for walls")]
    public MazeWall[] WallPrefabs;
    [Tooltip("Prefab to use on a basic passage.")]
    public MazePassage PassagePrefab;
    [Tooltip("Prefab to use when creating a door.")]
    public MazeDoor DoorPrefab;
    [Tooltip("Room settings to use when creating rooms.")]
    public MazeRoomSettings[] RoomSettings;

    [Header("Generation method")]
    [Tooltip("Change how we choose the next cell from the active cells to initialize.")]
    public IndexSelectionMethod CurrentIndexSelectionMethod;
    [Tooltip("If false, it won't create walls within the same room.")]
    public bool UsingWallInSameRoom;
    [Tooltip("If true, it will merge two rooms sided with the same settings.")]
    public bool MergeSameSettingRooms;

    [Header("Generation utils")]
    [Tooltip("Time between two step of generation.")]
    public float GenerationStepDelay;

    public enum IndexSelectionMethod
    {
        First,
        Middle,
        Last,
        Random
    }

    /// <summary>
    /// Generate the maze by randomly creating paths with cells.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Generate(int seed)
    {
        randomGenerator = new System.Random(seed);
        cells = new MazeCell[Size.x, Size.z];
        // Will contain the added cells in order
        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells);
        while (activeCells.Count > 0)
        {
            if (GenerationStepDelay > 0) yield return new WaitForSeconds(GenerationStepDelay);  
            DoNextGenerationStep(activeCells);
        }
        // Add fractals
        int count = randomGenerator.Next(0, NumberFractals);
        List<MazeCell> freeEdgesCells = new List<MazeCell>();
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.z; y++)
            {
                IntVector2 coordinates = new IntVector2(x, y);
                MazeCell cell = GetCell(coordinates);
                if (cell != null && cell.HasFreeCellEdges())
                {
                    freeEdgesCells.Add(cell);
                }
            }
        }
        for (int i = 0; i < count; i++)
        {

            if (freeEdgesCells.Count == 0) break;
            int index = randomGenerator.Next(0, freeEdgesCells.Count);
            MazeCell newCell = Instantiate(FractalPrefabs[randomGenerator.Next(0, FractalPrefabs.Length)]) as MazeCell;
            newCell.transform.position = freeEdgesCells[index].transform.position;
            newCell.transform.parent = transform;
            newCell.transform.localRotation = Quaternion.identity;
            freeEdgesCells.RemoveAt(index);
        }
    }

    public IntVector2 RandomCoordinates
    {
        get
        {
            return new IntVector2(randomGenerator.Next(0, Size.x), randomGenerator.Next(0, Size.z));
        }
    }

    public MazeCell GetCell(IntVector2 coordinates)
    {
        return cells[coordinates.x, coordinates.z];
    }

    public bool ContainsCoordinates(IntVector2 coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < Size.x && coordinate.z >= 0 && coordinate.z < Size.z;
    }

    private MazeCell[,] cells;
    private System.Random randomGenerator;

    private MazeCell CreateCell(IntVector2 coordinates)
    {
        MazeCell newCell = Instantiate(CellPrefab) as MazeCell;
        cells[coordinates.x, coordinates.z] = newCell;
        newCell.Coordinates = coordinates;
        newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
        newCell.transform.parent = transform;
        newCell.transform.localPosition = 
            new Vector3(coordinates.x - Size.x * 0.5f + 0.5f, 0f, coordinates.z - Size.z * 0.5f + 0.5f);
        newCell.transform.localRotation = Quaternion.identity;
        if (randomGenerator.NextDouble() < LightProbability)
        {
            newCell.HasLight = CanPutLightOnCell(newCell);
        }
        return newCell;
    }

    /// <summary>
    /// We can put a light on a cell, if the cell is far enough from another light **of the same room**
    /// TODO is concerned by cells from other room, should note, but the room is not set when this method is called.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool CanPutLightOnCell(MazeCell cell)
    {
        for (int x = Mathf.Max(0, cell.Coordinates.x - DistanceBetweenLights); x < Mathf.Min(Size.x, cell.Coordinates.x + DistanceBetweenLights); x++)
        {
            for (int y = Mathf.Max(0, cell.Coordinates.z - DistanceBetweenLights); y < Mathf.Min(Size.x, cell.Coordinates.z + DistanceBetweenLights); y++)
            {
                if (cells[x, y] != null  && cells[x, y].HasLight)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Create the first cell (randomly) f the generation.
    /// </summary>
    /// <param name="activeCells">The list where to add the new cell.</param>
    private void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
        MazeCell newCell = CreateCell(RandomCoordinates);
        newCell.Initialize(CreateRoom(-1));
        activeCells.Add(newCell);
    }

    /// <summary>
    /// Initialize one of the active cells by creating wall or other cells.
    /// The chosen active cell to be initialized depends of the IndexSelectionMethod.
    /// </summary>
    /// <param name="activeCells"></param>
    private void DoNextGenerationStep(List<MazeCell> activeCells)
    {
        int currentIndex = GetIndexBySelectionMethod(activeCells);
        MazeCell currentCell = activeCells[currentIndex];

        // if we initialized all edges, remove this cell from active cell
        if (currentCell.IsFullyInitialized)
        {
            activeCells.RemoveAt(currentIndex);
            return;
        }

        // Choose a random edge not initialized
        MazeDirection direction = currentCell.RandomUninitializedDirectionFrom(randomGenerator);
        IntVector2 coordinates = currentCell.Coordinates + direction.ToIntVector2();

        if (ContainsCoordinates(coordinates))
        {
            MazeCell neighbor = GetCell(coordinates);

            // If the choosen edge has'nt cell attached create a cell with a passage
            if (neighbor == null)
            {
                neighbor = CreateCell(coordinates);
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor);
            }
            else if ((!UsingWallInSameRoom && currentCell.Room == neighbor.Room) || 
                (MergeSameSettingRooms && currentCell.Room.settings == neighbor.Room.settings))
            {
                CreatePassageInSameRoom(currentCell, neighbor, direction);
            }
            // Else we create a wall with the existing cell
            else
            {
                CreateWall(currentCell, neighbor, direction);
            }
        }
        else // The edge is on the limit of the maze
        {
            CreateWall(currentCell, null, direction);
        }
    }

    private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        bool createDoor = randomGenerator.NextDouble() < DoorProbability;
        if (createDoor)
        {
            MazePassage door = Instantiate(DoorPrefab) as MazePassage;
            door.Initialize(cell, otherCell, direction);
            MazePassage passage = Instantiate(PassagePrefab);
            passage.Initialize(otherCell, cell, direction.GetOpposite());
            otherCell.Initialize(CreateRoom(cell.Room.settingsIndex));
        }
        else
        {
            MazePassage passage = Instantiate(PassagePrefab) as MazePassage;
            passage.Initialize(cell, otherCell, direction);
            passage = Instantiate(PassagePrefab) as MazePassage;
            otherCell.Initialize(cell.Room);
            passage.Initialize(otherCell, cell, direction.GetOpposite());
        }

    }

    private void CreatePassageInSameRoom(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage passage = Instantiate(PassagePrefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = Instantiate(PassagePrefab) as MazePassage;
        passage.Initialize(otherCell, cell, direction.GetOpposite());
        if (cell.Room != otherCell.Room)
        {
            MazeRoom roomToAssimilate = otherCell.Room;
            cell.Room.Assimilate(roomToAssimilate);
            rooms.Remove(roomToAssimilate);
            Destroy(roomToAssimilate);
        }
    }

    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazeWall wall = Instantiate(WallPrefabs[randomGenerator.Next(0, WallPrefabs.Length)]) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
        if (otherCell != null)
        {
            wall = Instantiate(WallPrefabs[randomGenerator.Next(0, WallPrefabs.Length)]) as MazeWall;
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }

    private int GetIndexBySelectionMethod(List<MazeCell> activeCells)
    {
        switch (CurrentIndexSelectionMethod)
        {
            case IndexSelectionMethod.First:
                return 0;
            case IndexSelectionMethod.Middle:
                return activeCells.Count / 2;
            case IndexSelectionMethod.Last:
                return activeCells.Count - 1;
            default:
                return randomGenerator.Next(0, activeCells.Count);
        }
    }

    private List<MazeRoom> rooms = new List<MazeRoom>();

    /// <summary>
    /// Avoid creating two times the same room by selecting the one after the current.
    /// Should be called with a negative value to create the first room.
    /// </summary>
    /// <param name="indexToExclude"></param>
    /// <returns></returns>
    private MazeRoom CreateRoom(int indexToExclude)
    {
        MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
        newRoom.settingsIndex = randomGenerator.Next(0, RoomSettings.Length);
        if (newRoom.settingsIndex == indexToExclude)
        {
            newRoom.settingsIndex = (newRoom.settingsIndex + 1) % RoomSettings.Length;
        }
        newRoom.settings = RoomSettings[newRoom.settingsIndex];
        rooms.Add(newRoom);
        return newRoom;
    }
}
