using System;
using Assets.Scripts.Data.Models;
using UnityEngine;

public class MapGridView : MonoBehaviour
{
    [Header("Tiles")]
    public GameObject wallTilePrefab;
    public GameObject walkableTilePrefab;
    public GameObject occupiedTilePrefab;
    public GameObject contactPointTilePrefab;

    [Header("Hero")]
    public GameObject heroViewPrefab;

    public void InitGrid(Node[,] grid)
    {
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                Node node = grid[row, col];

                if (node.nodeType == NodeType.Wall)
                {
                    // TODO: Instantiate nodeView instead of actual prefab;
                    GameObject newWallObj = Instantiate(wallTilePrefab, node.worldPosition, Quaternion.identity);
                }

                if (node.nodeType == NodeType.Open)
                {
                    GameObject newWallObj = Instantiate(walkableTilePrefab, node.worldPosition, Quaternion.identity);
                }

                if (node.nodeType == NodeType.Occupied)
                {
                    GameObject newWallObj = Instantiate(occupiedTilePrefab, node.worldPosition, Quaternion.identity);
                }

                if (node.nodeType == NodeType.ContactPoint)
                {
                    GameObject newWallObj = Instantiate(contactPointTilePrefab, node.worldPosition, Quaternion.identity);
                }
            }
        }
    }

    public void InitHero(Hero hero, Vector3 worldPosition)
    {
        GameObject heroObj = Instantiate(heroViewPrefab, worldPosition, Quaternion.identity);
    }
}
