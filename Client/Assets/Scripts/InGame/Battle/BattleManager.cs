using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Shared.NetMessages.World.Models;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(GraphView))]
public class BattleManager : MonoBehaviour
{
    [SerializeField]
    GameObject nodeHoverPrefab;

    

    [SerializeField]
    LayerMask interactableMask;

    public Graph graph;
    public GraphView graphView;

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
        this.cam = Camera.main;
        // NOTE: consider the case when the ready message arrives before the battle is created in the server.
        this.bd = DataManager.Instance.BattleData;
        this.battleService = new BattleService();
        this.battleService.SendConfirmLoadingBattleSceneMessage(this.bd.BattleId, this.bd.AttackerId, true);

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
            if (this.hoveredNode)
            {
                Debug.Log("Node exist!");
            }
            else
            {
                Debug.Log("Node do not exist!");
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit, 1000f, interactableMask))
        //    {
        //        GameObject graphic = hit.collider.gameObject;
        //        GameObject parent = graphic.transform.parent.gameObject;
        //        NodeView nodeView = parent.GetComponent<NodeView>();

        //        var unit = nodeView.Content;

        //        EnableHover(nodeView.transform.position);
        //    }
        //    else
        //    {
        //        DisableHover();
        //    }
        //}
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
        if (!this.CurrentPlayerIsMe(bd.CurrentHeroId))
        {
            Debug.LogWarning("I am not the current player!");
            return;
        }

        this.battleService.SendEndTurnRequest(bd.BattleId, bd.CurrentHeroId, bd.SelectedUnit.Id);
    }

    public bool CurrentPlayerIsMe(int currentPlayerId)
    {
        return DataManager.Instance.Avatar.Heroes.Any(h => h.Id == currentPlayerId);
    }

    private void RenderScene()
    {
        this.InitNodes();
        this.InitHeroes();
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

    private void InitHeroes()
    {
        this.UpdateCoordinates();

        var attackerCoords = new Coord(this.bd.AttackerHero.BattleX, this.bd.AttackerHero.BattleY);
        var defenderCoords = new Coord(this.bd.DefenderHero.BattleX, this.bd.DefenderHero.BattleY);

        graphView.AddHero(this.bd.AttackerHero, attackerCoords, false);
        graphView.AddHero(this.bd.DefenderHero, defenderCoords, false);
    }

    private void UpdateCoordinates()
    {
        this.bd.AttackerHero.BattleX = this.bd.AttackerHero.StartX;
        this.bd.AttackerHero.BattleY = this.bd.AttackerHero.StartY;

        this.bd.DefenderHero.BattleX = (graph.graphSizeX - 1) - this.bd.DefenderHero.StartX;
        this.bd.DefenderHero.BattleY = this.bd.DefenderHero.StartY;
    }

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
}
