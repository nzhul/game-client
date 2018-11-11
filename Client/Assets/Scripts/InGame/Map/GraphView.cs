﻿using Assets.Scripts.Data.Models;
using UnityEngine;

public class GraphView : MonoBehaviour
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

    public NodeView[,] nodeViews;

    private void Awake()
    {
        // TODO: init those based on NodeType numeration values. For each value create new gameObject and push them to array of containers
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

    public void Init(Graph graph)
    {
        nodeViews = new NodeView[graph.graphSizeX, graph.graphSizeY];

        for (int row = 0; row < graph.graphSizeX; row++)
        {
            for (int col = 0; col < graph.graphSizeY; col++)
            {
                Node node = graph.nodes[row, col];

                GameObject instance = Instantiate(walkableTilePrefab, node.worldPosition, Quaternion.identity);
                Transform parent = this.ResolveParent(node.nodeType);
                instance.transform.SetParent(parent);
                NodeView nodeView = instance.GetComponent<NodeView>();

                if (nodeView != null)
                {
                    nodeView.Init(node);
                    nodeViews[node.gridX, node.gridY] = nodeView;
                }
            }
        }
    }

    private Transform ResolveParent(NodeType nodeType)
    {
        Transform parent = null;

        switch (nodeType)
        {
            case NodeType.Open:
                parent = openPointsContainer.transform;
                break;
            case NodeType.Wall:
                parent = wallPointsContainer.transform;
                break;
            case NodeType.ContactPoint:
                parent = contactPointsContainer.transform;
                break;
            case NodeType.Occupied:
                parent = occupiedPointsContainer.transform;
                break;
            default:
                break;
        }

        return parent;
    }

    public HeroView InitHero(Hero hero, Vector3 worldPosition)
    {
        Vector3 placementPosition = new Vector3(worldPosition.x, heroViewPrefab.transform.position.y, worldPosition.z);
        GameObject instance = Instantiate(heroViewPrefab, placementPosition, Quaternion.identity);
        HeroView heroView = instance.GetComponent<HeroView>();

        if (heroView != null)
        {
            heroView.Init(hero, worldPosition);
        }

        return heroView;
    }
}
