using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;


}

public class PathNode
{
    int _x, _y;

    public int gCost, hCost, fCost;

    public PathNode prevNode;
}
