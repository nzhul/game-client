﻿using Assets.Scripts.InGame;
using Assets.Scripts.Shared.DataModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphView : MonoBehaviour
{
    public LayerMask movementMask;

    [Header("Tiles")]
    public GameObject walkableTilePrefab;

    [Header("Heroes")]
    public GameObject heroViewPrefab;
    private GameObject gridContainer;
    private GameObject contactPointsContainer;
    private GameObject occupiedPointsContainer;
    private GameObject openPointsContainer;
    private GameObject wallPointsContainer; 

    [Header("Monsters")]
    public GameObject monsterViewPrefab;

    public NodeView[,] nodeViews;

    [SerializeField]
    public Canvas labelsCanvas;

    [SerializeField]
    public Text labelPrefab;

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
                Transform parent = ResolveParent(node.nodeType);
                instance.transform.SetParent(parent);
                NodeView nodeView = instance.GetComponent<NodeView>();

                if (nodeView != null)
                {
                    nodeView.Init(node);
                    nodeViews[node.gridX, node.gridY] = nodeView;
                }

                // InitLabel(node);
            }
        }
    }

    private void InitLabel(Node node)
    {
        Color labelColor = Color.black;
        float yShift = 0.2f;
        switch (node.nodeType)
        {
            case NodeType.Open:
                break;
            case NodeType.Wall:
                labelColor = Color.white;
                break;
            case NodeType.ContactPoint:
                yShift = 0.5f;
                break;
            case NodeType.Occupied:
                yShift = 1.5f;
                break;
            default:
                break;
        }
        Text label = Instantiate(labelPrefab, new Vector3(node.worldPosition.x, node.worldPosition.y + yShift, node.worldPosition.z), labelsCanvas.transform.rotation);
        label.color = labelColor;
        label.text = "X:" + node.gridX + Environment.NewLine + "Y:" + node.gridY;
        label.transform.SetParent(labelsCanvas.transform);
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

    public void AddHeroes(IList<Hero> heroes)
    {
        foreach (var hero in heroes)
        {
            this.AddHero(hero, false);
        }
    }

    public void AddHero(Hero hero, bool playSpawnEffect)
    {
        var heroView = InitHero(hero, playSpawnEffect);
        HeroesManager.Instance.Heroes.Add(hero.id, heroView);
    }

    public HeroView InitHero(Hero hero, bool playSpawnEffect)
    {
        Vector3 worldPosition = MapManager.Instance.GetNodeWorldPosition(hero.x, hero.y);
        Vector3 placementPosition = new Vector3(worldPosition.x, heroViewPrefab.transform.position.y, worldPosition.z);
        GameObject instance = Instantiate(heroViewPrefab, placementPosition, Quaternion.identity);
        HeroView heroView = instance.GetComponent<HeroView>();

        if (playSpawnEffect)
        {
            heroView.PlayTeleportEffect(HeroView.TeleportType.In);
        }

        if (heroView != null)
        {
            heroView.Init(hero, worldPosition);
        }

        return heroView;
    }

    public MonsterView AddMonster(MonsterPack monster)
    {
        var monsterView = InitMonster(monster);
        MonstersManager.Instance.Monsters.Add(monster.id, monsterView);

        return monsterView;
    }

    private MonsterView InitMonster(MonsterPack monster)
    {
        Vector3 worldPosition = MapManager.Instance.GetNodeWorldPosition(monster.x, monster.y);
        Vector3 placementPosition = new Vector3(worldPosition.x, monsterViewPrefab.transform.position.y, worldPosition.z);
        GameObject instance = Instantiate(monsterViewPrefab, placementPosition, Quaternion.identity);
        MonsterView monsterView = instance.GetComponent<MonsterView>();

        if (monsterView != null)
        {
            monsterView.Init(monster, worldPosition);
        }

        return monsterView;
    }

    public void AddMonsters(IList<MonsterPack> monsterPacks)
    {
        foreach (MonsterPack monster in monsterPacks)
        {
            MonsterView newMonster = this.AddMonster(monster);

            NodeView nodeView = nodeViews[monster.x, monster.y]; // x = cols; y = rows

            if (nodeView != null)
            {
                newMonster.transform.SetParent(nodeView.transform);
                nodeView.Slot = newMonster;

                // GameObject instance = Instantiate(monsterViewPrefab, nodeView.node.worldPosition, Quaternion.identity);
                // instance.transform.SetParent(nodeView.transform);
                // MonsterView monsterView = instance.GetComponent<MonsterView>();

                // if(monsterView != null)
                // monsterView.Init()
                // monsterViewsList.Add(monsterView);

                // nodeView.slot = MonsterView -> MonsterView : SlotView
            }
        }
    }
}
