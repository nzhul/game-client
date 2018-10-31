using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Node[,] nodes;

    public List<Node> walls = new List<Node>();

    public List<Node> contactPoints = new List<Node>();

    public List<Node> occupied = new List<Node>();

    int[,] _matrix;

    private int _width;

    public int Width
    {
        get { return _width; }
    }

    private int _height;

    public int Height
    {
        get { return _height; }
        set { _height = value; }
    }

    public static readonly Vector2[] allDirections =
    {
        new Vector2(0,1),
        new Vector2(1,1),
        new Vector2(1,0),
        new Vector2(1,-1),
        new Vector2(0,-1),
        new Vector2(-1,-1),
        new Vector2(-1,0),
        new Vector2(-1,1)
    };

    public void Init(int[,] matrix)
    {
        _matrix = matrix;
        _width = matrix.GetLength(0);
        _height = matrix.GetLength(1);

        nodes = new Node[_width, _height];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                NodeType type = (NodeType)_matrix[x, y];
                Node newNode = new Node(x, y, type);
                nodes[x, y] = newNode;

                newNode.position = new Vector3(x, 0, y);

                if (type == NodeType.Wall)
                {
                    walls.Add(newNode);
                }

                if (type == NodeType.Occupied)
                {
                    occupied.Add(newNode);
                }

                if (type == NodeType.ContactPoint)
                {
                    contactPoints.Add(newNode);
                }
            }
        }


        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (nodes[x, y].nodeType == NodeType.Open)
                {
                    nodes[x, y].neighbors = GetNeighbors(x, y);
                }
            }
        }
    }

    public float GetNodeDistance(Node source, Node target)
    {
        int dx = Mathf.Abs(source.xIndex - target.xIndex);
        int dy = Mathf.Abs(source.yIndex - target.yIndex);

        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);

        int diagonalSteps = min;
        int straightSteps = max - min;

        return (1.4f * diagonalSteps + straightSteps);
    }

    // Manhatan distance ignores the diagonal path and has better performance compared to GetNodeDistance
    // "Manhatan" comes from the comparison with zig-zag taxi driver that drives through Manhatan city.
    // Use this instread of GetNodeDistance for slight performance boost.
    public int GetManhattanDistance(Node source, Node target)
    {
        int dx = Mathf.Abs(source.xIndex - target.xIndex);
        int dy = Mathf.Abs(source.yIndex - target.yIndex);

        return (dx + dy);
    }

    private List<Node> GetNeighbors(int x, int y)
    {
        return this.GetNeighbors(x, y, nodes, allDirections);
    }

    List<Node> GetNeighbors(int x, int y, Node[,] nodeArray, Vector2[] directions)
    {
        List<Node> neighborNodes = new List<Node>();

        foreach (Vector2 dir in directions)
        {
            int newX = x + (int)dir.x;
            int newY = y + (int)dir.y;

            if (IsWithinBounds(newX, newY)
                && nodeArray[newX, newY] != null
                && nodeArray[newX, newY].nodeType == NodeType.Open)
            {
                neighborNodes.Add(nodeArray[newX, newY]);
            }
        }

        return neighborNodes;
    }

    private bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < _width && y >= 0 && y < _height);
    }
}
