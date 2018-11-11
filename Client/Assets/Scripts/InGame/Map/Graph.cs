using System;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public float nodeRadius;
    public Node[,] nodes;

    Vector2 gridWorldSize;
    float nodeDiameter;
    public int graphSizeX, graphSizeY;


    public int MaxSize
    {
        get
        {
            return graphSizeX * graphSizeY;
        }
    }

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
    }

    public void Init(string matrixString)
    {
        int[,] matrix = this.ParseMatrix(matrixString);

        graphSizeX = matrix.GetLength(0);
        graphSizeY = matrix.GetLength(1);

        gridWorldSize = new Vector2(Mathf.RoundToInt(graphSizeX * nodeDiameter), Mathf.RoundToInt(graphSizeY * nodeDiameter)); // NOT sure about this line ...


        nodes = new Node[graphSizeX, graphSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < graphSizeX; x++)
        {
            for (int y = 0; y < graphSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                NodeType nodeType = (NodeType)matrix[x, y];
                nodes[x, y] = new Node(nodeType, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < graphSizeX && checkY >= 0 && checkY < graphSizeY)
                {
                    neighbours.Add(nodes[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((graphSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((graphSizeY - 1) * percentY);
        return nodes[x, y];
    }

    private int[,] ParseMatrix(string matrixString)
    {
        string[] lines = matrixString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        int[,] parsedMatrix = new int[lines[0].Length, lines.Length];
        for (int row = 0; row < lines.Length; row++)
        {
            string line = lines[row];
            for (int col = 0; col < line.Length; col++)
            {
                parsedMatrix[col, row] = (int)char.GetNumericValue(line[col]);
            }
        }

        return parsedMatrix;
    }
}