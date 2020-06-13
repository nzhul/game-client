using System.Linq;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.NetMessages.Battle;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class ConfirmLoadingBattleSceneHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_ConfirmLoadingBattleSceneRequest msg = (Net_ConfirmLoadingBattleSceneRequest)input;

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