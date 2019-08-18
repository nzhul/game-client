using Assets.Scripts.InGame;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World.Models;
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

    private Graph graph;

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
        this.graph = graph;

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
            if (hero.IsNPC)
            {
                continue;
            }

            this.AddHero(hero, new Coord(hero.X, hero.Y), false);
        }
    }

    public void AddHero(Hero hero, Coord spawnCoordinates, bool playSpawnEffect)
    {
        var heroView = InitHero(hero, spawnCoordinates, playSpawnEffect);

        if (HeroesManager.Instance.Heroes.ContainsKey(hero.Id))
        {
            // 1. If the heroView exist - we override
            HeroesManager.Instance.Heroes[hero.Id] = heroView;
        }
        else
        {
            // 2. Else - create new
            HeroesManager.Instance.Heroes.Add(hero.Id, heroView);
        }
    }

    public HeroView InitHero(Hero hero, Coord spawnCoordinates, bool playSpawnEffect)
    {
        //Vector3 worldPosition = MapManager.Instance.GetNodeWorldPosition(spawnCoordinates.X, spawnCoordinates.Y);
        Vector3 worldPosition = this.graph.GetNodeWorldPosition(spawnCoordinates.X, spawnCoordinates.Y);
        Vector3 placementPosition = new Vector3(worldPosition.x, heroViewPrefab.transform.position.y, worldPosition.z);
        GameObject instance = Instantiate(heroViewPrefab, placementPosition, Quaternion.identity);
        HeroView heroView = instance.GetComponent<HeroView>();

        if (playSpawnEffect)
        {
            heroView.PlayTeleportEffect(HeroView.TeleportType.In);
        }

        if (heroView != null)
        {
            heroView.Init(hero, spawnCoordinates, worldPosition);
        }

        return heroView;
    }

    public NPCView AddNPC(Hero npc)
    {
        var npcView = InitNPC(npc);
        HeroesManager.Instance.NPCs.Add(npc.Id, npcView);

        return npcView;
    }

    private NPCView InitNPC(Hero monster)
    {
        //Vector3 worldPosition = MapManager.Instance.GetNodeWorldPosition(monster.X, monster.Y);
        Vector3 worldPosition = this.graph.GetNodeWorldPosition(monster.X, monster.Y);
        Vector3 placementPosition = new Vector3(worldPosition.x, monsterViewPrefab.transform.position.y, worldPosition.z);
        GameObject instance = Instantiate(monsterViewPrefab, placementPosition, Quaternion.identity);
        NPCView monsterView = instance.GetComponent<NPCView>();

        if (monsterView != null)
        {
            monsterView.Init(monster, worldPosition);
        }

        return monsterView;
    }

    public void AddNPCs(IList<Hero> npcHeroes)
    {
        foreach (Hero npcHero in npcHeroes)
        {
            if (!npcHero.IsNPC)
            {
                continue;
            }

            NPCView newNpc = this.AddNPC(npcHero);

            NodeView nodeView = nodeViews[npcHero.X, npcHero.Y]; // x = cols; y = rows

            if (nodeView != null)
            {
                newNpc.transform.SetParent(nodeView.transform);
                nodeView.Slot = newNpc;

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
