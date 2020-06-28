using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Shared.NetMessages.Battle;
using System;

namespace Assets.Scripts.Network.Services.TCP
{
    public class BattleService : IBattleService
    {
        public void SendConfirmLoadingBattleSceneMessage(Guid battleId, int armyId, bool isReady)
        {
            if (NetworkClient.Instance == null || !NetworkClient.Instance.IsStarted)
            {
                return;
            }

            Net_ConfirmLoadingBattleSceneRequest msg = new Net_ConfirmLoadingBattleSceneRequest
            {
                BattleId = battleId,
                ArmyId = armyId,
                IsReady = true
            };

            NetworkClient.Instance.SendServer(msg);
        }

        public void SendEndTurnRequest(Guid battleId, int currentArmyId, int currentUnitId)
        {
            if (NetworkClient.Instance == null || !NetworkClient.Instance.IsStarted)
            {
                return;
            }

            Net_EndTurnRequest msg = new Net_EndTurnRequest
            {
                BattleId = battleId,
                RequesterArmyId = currentArmyId,
                RequesterUnitId = currentUnitId
            };

            NetworkClient.Instance.SendServer(msg);
        }
    }
}