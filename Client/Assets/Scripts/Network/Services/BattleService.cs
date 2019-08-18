using Assets.Scripts.Shared.NetMessages.Battle;
using System;

namespace Assets.Scripts.Network.Services
{
    public class BattleService : IBattleService
    {
        public void SendConfirmLoadingBattleSceneMessage(Guid battleId, int heroId, bool isReady)
        {
            if (NetworkClient.Instance == null || !NetworkClient.Instance.IsStarted)
            {
                return;
            }

            Net_ConfirmLoadingBattleSceneRequest msg = new Net_ConfirmLoadingBattleSceneRequest
            {
                BattleId = battleId,
                HeroId = heroId,
                IsReady = true
            };

            NetworkClient.Instance.SendServer(msg);
        }

        public void SendEndTurnRequest(Guid battleId, int currentPlayerId, int currentUnitId)
        {
            if (NetworkClient.Instance == null || !NetworkClient.Instance.IsStarted)
            {
                return;
            }

            Net_EndTurnRequest msg = new Net_EndTurnRequest
            {
                BattleId = battleId,
                RequesterHeroId = currentPlayerId,
                RequesterUnitId = currentUnitId
            };

            NetworkClient.Instance.SendServer(msg);
        }
    }
}