using UnityEngine;

public enum MazeDirection
{
    North,
    East,
    South,
    West
}

public static class MazeDirections
{
    /// <summary>
    /// The number of directions.
    /// </summary>
    public const int Count = 4;

    /// <summary>
    /// A random direction.
    /// </summary>
    public static MazeDirection RandomValue
    {
        get
        {
            return (MazeDirection)Random.Range(0, Count);
        }
    }

    /// <summary>
    /// A random direction.
    /// </summary>
    public static MazeDirection RandomValueFrom(System.Random generator)
    {
        return (MazeDirection)generator.Next(0, Count);
    }

    /// <summary>
    /// Convert a direction to an IntVector2.
    /// </summary>
    /// <param name="direction">The direction to convert.</param>
    /// <returns>The direction as an IntVector2</returns>
    public static IntVector2 ToIntVector2(this MazeDirection direction)
    {
        return vectors[(int)direction];
    }

    /// <summary>
    /// Return the opposite direction of the given direction.
    /// </summary>
    /// <param name="direction">The direction we want the opposite.</param>
    /// <returns></returns>
    public static MazeDirection GetOpposite(this MazeDirection direction)
    {
        return opposites[(int)direction];
    }

    /// <summary>
    /// Return the rotation to put a wall on the given direction.
    /// </summary>
    /// <param name="direction">The direction we want to add the wall.</param>
    /// <returns></returns>
    public static Quaternion ToRotation(this MazeDirection direction)
    {
        return rotations[(int)direction];
    }

    /// <summary>
    /// Directions as vectors.
    /// </summary>
    private static IntVector2[] vectors = {
		new IntVector2(0, 1), // North
		new IntVector2(1, 0), // East
		new IntVector2(0, -1), // South
		new IntVector2(-1, 0) // West
	};

    /// <summary>
    /// The opposites of directions.
    /// </summary>
    private static MazeDirection[] opposites = {
        MazeDirection.South, // North
        MazeDirection.West, // East
        MazeDirection.North, // South
        MazeDirection.East // West
    };

    private static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };
}