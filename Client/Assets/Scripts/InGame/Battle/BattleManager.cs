using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Network.Services;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
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
    }
    #endregion

    public const int TURN_DURATION = 20;

    private IBattleService battleService;
    private BattleData bd;

    //bool _actionsEnabled = false;
    //public bool ActionsEnabled
    //{
    //    get
    //    {
    //        return _actionsEnabled;
    //    }
    //    set
    //    {
    //        _actionsEnabled = value;
    //        OnActionsEnabledChange?.Invoke(value);
    //    }
    //}

    public static event Action<int> OnRemainingTimeUpdate;

    public void EndTurn()
    {
        if (!this.CurrentPlayerIsMe(bd.CurrentPlayerId))
        {
            Debug.LogWarning("I am not the current player!");
            return;
        }

        this.battleService.SendEndTurnRequest(bd.BattleId, bd.CurrentPlayerId);
    }

    public bool CurrentPlayerIsMe(int currentPlayerId)
    {
        return DataManager.Instance.Avatar.Heroes.Any(h => h.Id == currentPlayerId);
    }

    //public static event Action<bool> OnActionsEnabledChange;

    private void Start()
    {

        // NOTE: consider the case when the ready message arrives before the battle is created in the server.
        this.bd = DataManager.Instance.BattleData;
        this.battleService = new BattleService();
        this.battleService.SendConfirmLoadingBattleSceneMessage(this.bd.BattleId, this.bd.AttackerId, true);

        StopCoroutine(UpdateRemainingTurnTime());
        StartCoroutine(UpdateRemainingTurnTime());
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
