using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDoor : MazePassage {

    public Transform Model;
    public Transform Hinge;

    private MazeDoor OtherSideOfDoor
    {
        get
        {
            return otherCell.GetEdge(direction.GetOpposite()) as MazeDoor;
        }
    }
}
