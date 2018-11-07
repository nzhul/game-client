using Assets.Scripts.Data.Models;
using UnityEngine;

public class MapGridView : MonoBehaviour
{
    public LayerMask movementMask;

    [Header("Tiles")]
    public GameObject wallTilePrefab;
    public GameObject walkableTilePrefab;
    public GameObject occupiedTilePrefab;
    public GameObject contactPointTilePrefab;

    [Header("Hero")]
    public GameObject heroViewPrefab;

    GameObject gridContainer;
    GameObject contactPointsContainer;
    GameObject occupiedPointsContainer;
    GameObject openPointsContainer;
    GameObject wallPointsContainer;

    private void Awake()
    {
        gridContainer = new GameObject("GridContainer");
        contactPointsContainer = new GameObject("ContactPointsContainer");
        occupiedPointsContainer = new GameObject("OccupiedPointsContainer");
        openPointsContainer = new GameObject("OpenPointsContainer");
        wallPointsContainer = new GameObject("wallPointsContainer");

        contactPointsContainer.transform.SetParent(gridContainer.transform);
        occupiedPointsContainer.transform.SetParent(gridContainer.transform);
        openPointsContainer.transform.SetParent(gridContainer.transform);
        wallPointsContainer.transform.SetParent(gridContainer.transform);
    }

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
                    // TODO: Handle code duplication
                    GameObject newObj = Instantiate(wallTilePrefab, node.worldPosition, Quaternion.identity);
                    newObj.transform.SetParent(wallPointsContainer.transform);
                }

                if (node.nodeType == NodeType.Open)
                {
                    GameObject newObj = Instantiate(walkableTilePrefab, node.worldPosition, Quaternion.identity);
                    newObj.transform.SetParent(openPointsContainer.transform);
                    newObj.layer = LayerMask.NameToLayer("Interactable");
                }

                if (node.nodeType == NodeType.Occupied)
                {
                    GameObject newObj = Instantiate(occupiedTilePrefab, node.worldPosition, Quaternion.identity);
                    newObj.transform.SetParent(occupiedPointsContainer.transform);
                }

                if (node.nodeType == NodeType.ContactPoint)
                {
                    GameObject newObj = Instantiate(contactPointTilePrefab, node.worldPosition, Quaternion.identity);
                    newObj.transform.SetParent(contactPointsContainer.transform);
                    newObj.layer = LayerMask.NameToLayer("Interactable");
                }
            }
        }
    }

    public void InitHero(Hero hero, Vector3 worldPosition)
    {
        Vector3 placementPosition = new Vector3(worldPosition.x, heroViewPrefab.transform.position.y, worldPosition.z);
        GameObject heroObj = Instantiate(heroViewPrefab, placementPosition, Quaternion.identity);
    }
}
