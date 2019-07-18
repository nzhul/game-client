using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Network.Services;
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

    private IBattleService battleService;
    private BattleData bd;

    private void Start()
    {

        // NOTE: consider the case when the ready message arrives before the battle is created in the server.
        this.bd = DataManager.Instance.BattleData;
        this.battleService = new BattleService();
        this.battleService.SendConfirmLoadingBattleSceneMessage(this.bd.BattleId, this.bd.AttackerId, true);
    }
}
