using Assets.Scripts.Shared.NetMessages.Battle;
using Assets.Scripts.Shared.NetMessages.Battle.Models;
using System.Linq;

namespace Assets.Scripts.MessageHandlers
{
    public class ConfirmLoadingBattleSceneHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_ConfirmLoadingBattleScene msg = (Net_ConfirmLoadingBattleScene)input;

            if (msg.IsValid())
            {
                var battle = NetworkServer.Instance.ActiveBattles.FirstOrDefault(b => b.Id == msg.BattleId);

                if (battle != null && battle.AttackerId == msg.HeroId)
                {
                    battle.AttackerReady = true;
                }

                if (battle != null && battle.DefenderId == msg.HeroId)
                {
                    battle.DefenderReady = true;
                }

                if (battle.AttackerReady && battle.DefenderReady)
                {
                    battle.State = BattleState.Fight;
                }
            }
        }
    }
}