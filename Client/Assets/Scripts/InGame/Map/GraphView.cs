using System;
using System.Collections.Generic;
using Assets.Scripts.InGame;
using Assets.Scripts.InGame.Map.Entities;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.Models.Units;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GraphView : MonoBehaviour
{
    public LayerMask movementMask;

    [Header("Tiles")]
    public GameObject walkableTilePrefab;

    [Header("Heroes")]
    public GameObject entityViewPrefab;
    public GameObject gridContainer;
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
        //gridContainer = new GameObject("GridContainer");
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

        nodeViews = new NodeView[graph.graphSizeY, graph.graphSizeX];

        for (int x = 0; x < graph.graphSizeX; x++) // TODO: chage this to X and Y and probably swap them in graph.nodes[row, col] -> graph.nodes[x, y]
        {
            for (int y = 0; y < graph.graphSizeY; y++)
            {
                Node node = graph.nodes[y, x];

                GameObject instance = Instantiate(walkableTilePrefab, node.worldPosition, Quaternion.identity);
                Transform parent = ResolveParent(node.nodeType);
                instance.transform.SetParent(parent);
                NodeView nodeView = instance.GetComponent<NodeView>();

                if (nodeView != null)
                {
                    nodeView.Init(node);
                    nodeViews[node.gridY, node.gridX] = nodeView;
                }

                // InitLabel(node);
            }
        }
    }

    public NodeContent GetNodeContent(GameObject nodeGraphic)
    {
        GameObject node = nodeGraphic.transform.parent.gameObject;
        NodeView nodeView = node.GetComponent<NodeView>();
        return nodeView.Content ?? null;
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

    public void DisplayAvailibleDestinations(List<NodeView> availibleDestinations, Color displayColor)
    {

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

    public void AddEntities(IEnumerable<GridEntity> entities)
    {
        foreach (var entity in entities)
        {
            //if (army.IsNPC)
            //{
            //    continue;
            //}

            this.AddEntity(entity, new Coord(entity.X, entity.Y), false);
        }
    }

    public void AddEntity(GridEntity entity, Coord spawnCoordinates, bool playSpawnEffect)
    {
        var armyView = InitEntity(entity, spawnCoordinates, playSpawnEffect);

        if (EntitiesManager.Instance.Entities.ContainsKey(entity.Id))
        {
            // 1. If the heroView exist - we override
            EntitiesManager.Instance.Entities[entity.Id] = armyView;
        }
        else
        {
            // 2. Else - create new
            EntitiesManager.Instance.Entities.Add(entity.Id, armyView);
        }
    }

    public EntityView InitEntity(GridEntity entity, Coord spawnCoordinates, bool playSpawnEffect)
    {
        //Vector3 worldPosition = MapManager.Instance.GetNodeWorldPosition(spawnCoordinates.X, spawnCoordinates.Y);
        Vector3 worldPosition = this.graph.GetNodeWorldPosition(spawnCoordinates.X, spawnCoordinates.Y);
        Vector3 placementPosition = new Vector3(worldPosition.x, entityViewPrefab.transform.position.y, worldPosition.z);
        GameObject instance = Instantiate(entityViewPrefab, placementPosition, Quaternion.identity);
        var heroView = instance.GetComponent<EntityView>();

        if (playSpawnEffect)
        {
            heroView.PlayTeleportEffect(ArmyView.TeleportType.In);
        }

        if (heroView != null)
        {
            heroView.Init(entity, spawnCoordinates, worldPosition);
        }

        this.AddContentToNode(spawnCoordinates, heroView);

        return heroView;
    }

    //public NPCView AddNPC(Army npc)
    //{
    //    var npcView = InitNPC(npc);
    //    HeroesManager.Instance.NPCs.Add(npc.Id, npcView);

    //    return npcView;
    //}

    public void AddContentToNode(Coord coord, NodeContent content)
    {
        NodeView nodeView = nodeViews[coord.Y, coord.X]; // x = cols; y = rows

        if (nodeView != null)
        {
            content.transform.SetParent(nodeView.transform);
            nodeView.Content = content;
        }
    }

    //private NPCView InitNPC(Army army)
    //{
    //    //Vector3 worldPosition = MapManager.Instance.GetNodeWorldPosition(monster.X, monster.Y);
    //    Vector3 worldPosition = this.graph.GetNodeWorldPosition(army.X, army.Y);
    //    Vector3 placementPosition = new Vector3(worldPosition.x, monsterViewPrefab.transform.position.y, worldPosition.z);
    //    GameObject instance = Instantiate(monsterViewPrefab, placementPosition, Quaternion.identity);
    //    NPCView monsterView = instance.GetComponent<NPCView>();

    //    if (monsterView != null)
    //    {
    //        monsterView.Init(army, worldPosition);
    //    }

    //    return monsterView;
    //}

    //public void AddNPCArmies(IList<Army> npcArmies)
    //{
    //    foreach (Army npcArmy in npcArmies)
    //    {
    //        if (!npcArmy.IsNPC)
    //        {
    //            continue;
    //        }

    //        NPCView newNpc = this.AddNPC(npcArmy);

    //        this.AddContentToNode(new Coord(npcArmy.X, npcArmy.Y), newNpc);

    //        //NodeView nodeView = nodeViews[npcHero.X, npcHero.Y]; // x = cols; y = rows

    //        //if (nodeView != null)
    //        //{
    //        //    newNpc.transform.SetParent(nodeView.transform);
    //        //    nodeView.Content = newNpc;

    //        //    // GameObject instance = Instantiate(monsterViewPrefab, nodeView.node.worldPosition, Quaternion.identity);
    //        //    // instance.transform.SetParent(nodeView.transform);
    //        //    // MonsterView monsterView = instance.GetComponent<MonsterView>();

    //        //    // if(monsterView != null)
    //        //    // monsterView.Init()
    //        //    // monsterViewsList.Add(monsterView);

    //        //    // nodeView.slot = MonsterView -> MonsterView : SlotView
    //        //}
    //    }
    //}
}
