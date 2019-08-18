using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
//using Assets.Scripts.InGame.Battle;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World.Models;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(GraphView))]
public class BattleManager : MonoBehaviour
{
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

    private void Start()
    {

        // NOTE: consider the case when the ready message arrives before the battle is created in the server.
        this.bd = DataManager.Instance.BattleData;
        this.battleService = new BattleService();
        this.battleService.SendConfirmLoadingBattleSceneMessage(this.bd.BattleId, this.bd.AttackerId, true);

        StopCoroutine(UpdateRemainingTurnTime());
        StartCoroutine(UpdateRemainingTurnTime());

        this.RenderScene();
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

        graphView.AddHero(this.bd.AttackerHero, new Coord(this.bd.AttackerHero.BattleX, this.bd.AttackerHero.BattleY), false);
        graphView.AddHero(this.bd.DefenderHero, new Coord(this.bd.DefenderHero.BattleX, this.bd.DefenderHero.BattleY), false);
    }

    private void UpdateCoordinates()
    {
        this.bd.AttackerHero.BattleX = this.bd.AttackerHero.StartX;
        this.bd.AttackerHero.BattleY = this.bd.AttackerHero.StartY;

        this.bd.DefenderHero.BattleX = (graph.graphSizeX - 1) - this.bd.DefenderHero.StartX;
        this.bd.DefenderHero.BattleY = this.bd.DefenderHero.StartY;
    }

    private Coord InverseColumn(Coord battleCurrentPosition, int graphSizeX)
    {
        return new Coord
        {
            X = graphSizeX - battleCurrentPosition.X,
            Y = battleCurrentPosition.Y
        };
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

    //public static event Action<bool> OnActionsEnabledChange;

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
