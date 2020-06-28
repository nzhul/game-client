using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Network.Services.TCP;
using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.Models.Units;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(GraphView))]
public class BattleManager : MonoBehaviour
{
    //TODO: Move highlight logic in other class
    [SerializeField]
    GameObject nodeHoverPrefab = default;

    [SerializeField]
    GameObject nodeAvailiblePathPrefab = default;

    [SerializeField]
    LayerMask interactableMask = default;

    public Graph graph;
    public GraphView graphView;

    private List<GameObject> highlights;

    #region Singleton
    private static BattleManager _instance;

    public static BattleManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        graph = GetComponent<Graph>();
        graphView = GetComponent<GraphView>();
    }
    #endregion

    public const int TURN_DURATION = 20;

    private IBattleService battleService;
    private BattleData bd;

    public static event Action<int> OnRemainingTimeUpdate;

    private Camera cam;

    GameObject nodeHover;

    GameObject hoveredNode;

    private void Start()
    {
        this.highlights = new List<GameObject>();
        this.cam = Camera.main;
        // NOTE: consider the case when the ready message arrives before the battle is created in the server.
        this.bd = DataManager.Instance.BattleData;
        this.battleService = new BattleService();
        this.battleService.SendConfirmLoadingBattleSceneMessage(this.bd.BattleId, this.bd.AttackerArmyId, true);

        StopCoroutine(UpdateRemainingTurnTime());
        StartCoroutine(UpdateRemainingTurnTime());

        this.RenderScene();
        this.InitHoverHighlight();
    }

    private void InitHoverHighlight()
    {
        if (nodeHoverPrefab != null)
        {
            nodeHover = Instantiate(nodeHoverPrefab);
            nodeHover.SetActive(false);
        }
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        this.UpdateSelectedNode();

        if (Input.GetMouseButtonDown(0))
        {
            if (!this.hoveredNode)
            {
                return;
            }

            var content = graphView.GetNodeContent(this.hoveredNode);
            if (content != null)
            {
                this.HandleTouch(content);
            }
        }
    }

    private void HandleTouch(NodeContent content)
    {
        string displayName = string.Empty;

        if (content.Type == NodeContentType.Unit)
        {
            var unit = content as UnitView;
            displayName = (unit.rawEntity as Unit).Type.ToString();

            graphView.DisplayAvailibleDestinations(unit.AvailibleDestinations, Color.green);
        }



        // 1. Display:
        // 1.1. Name or Type
        // 1.2 Portrait
        // 1.3 Abilities
        // 2. Mark as selected in the UI
        // 3. Display availible destinations
    }

    private void UpdateSelectedNode()
    {
        Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, interactableMask))
        {
            if (this.hoveredNode != null && this.hoveredNode == hit.collider.gameObject)
            {
                return;
            }

            EnableHover(hit.collider.gameObject);
        }
        else
        {

            DisableHover();
        }
    }

    public void EnableHover(GameObject node)
    {
        this.hoveredNode = node;
        nodeHover.SetActive(true);
        Vector3 pos = this.hoveredNode.transform.position;
        nodeHover.transform.position = new Vector3(pos.x, pos.y + 0.11f, pos.z);
    }

    public void DisableHover()
    {
        this.hoveredNode = null;
        nodeHover.transform.position = Vector3.zero;
        nodeHover.SetActive(false);
    }

    public void EndTurn()
    {
        if (!this.CurrentPlayerIsMe(bd.CurrentArmyId))
        {
            Debug.LogWarning("I am not the current player!");
            return;
        }

        this.battleService.SendEndTurnRequest(bd.BattleId, bd.CurrentArmyId, bd.SelectedUnit.Id);
    }

    public bool CurrentPlayerIsMe(int currentArmyId)
    {
        return DataManager.Instance.ActiveGame.GetArmy(currentArmyId).UserId == DataManager.Instance.Id;
    }

    private void RenderScene()
    {
        this.InitNodes();
        this.InitUnits();
        //this.InitUnits();
    }

    private void InitNodes()
    {
        if (graph != null && graphView != null)
        {
            var matrixString =
@"000000000000000
000000000000000
000000000000000
000000000000000
000000000000000
000000000000000
000000000000000
000000000000000
000000000000000
000000000000000
000000000000000";

            graph.Init(matrixString);
            graphView.Init(graph);
        }
    }

    private void InitUnits()
    {
        //this.UpdateCoordinates();

        //var attackerCoords = new Coord(this.bd.AttackerArmy.BattleX, this.bd.AttackerArmy.BattleY);
        //var defenderCoords = new Coord(this.bd.DefenderArmy.BattleX, this.bd.DefenderArmy.BattleY);

        //graphView.AddHero(this.bd.AttackerArmy, attackerCoords, false);
        //graphView.AddHero(this.bd.DefenderArmy, defenderCoords, false);
    }

    //private void UpdateCoordinates()
    //{
    //    this.bd.AttackerArmy.BattleX = this.bd.AttackerArmy.StartX;
    //    this.bd.AttackerArmy.BattleY = this.bd.AttackerArmy.StartY;

    //    this.bd.DefenderArmy.BattleX = (graph.graphSizeX - 1) - this.bd.DefenderArmy.StartX;
    //    this.bd.DefenderArmy.BattleY = this.bd.DefenderArmy.StartY;
    //}

    IEnumerator UpdateRemainingTurnTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            bd.RemainingTimeForThisTurn--;
            OnRemainingTimeUpdate?.Invoke(bd.RemainingTimeForThisTurn);
            yield return null;
        }
    }

    public void HideHighlights()
    {
        foreach (GameObject go in this.highlights)
        {
            go.GetComponent<MeshRenderer>().material.color = Color.white;
            go.SetActive(false);
        }
    }

    private GameObject GetHighlightObject()
    {
        GameObject instance = highlights.Find(x => !x.activeSelf);
        if (instance == null)
        {
            instance = Instantiate(nodeAvailiblePathPrefab);
            this.highlights.Add(instance);
        }

        return instance;
    }
}
