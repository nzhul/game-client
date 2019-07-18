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

            Net_ConfirmLoadingBattleScene msg = new Net_ConfirmLoadingBattleScene
            {
                BattleId = battleId,
                HeroId = heroId,
                IsReady = true
            };

            NetworkClient.Instance.SendServer(msg);
        }
    }
}