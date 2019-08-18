using Assets.Scripts.Services;
using Assets.Scripts.Shared.NetMessages.Battle;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MessageHandlers
{
    public class EndTurnRequestHandler : IMessageHandler
    {
        private IBattleService battleService;

        public EndTurnRequestHandler()
        {
            this.battleService = new BattleService();
        }

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_EndTurnRequest msg = (Net_EndTurnRequest)input;

            if (!msg.IsValid())
            {
                Debug.LogWarning($"Invalid {nameof(msg)}");
                return;
            }

            // 1. Find the battle
            var battle = NetworkServer.Instance.ActiveBattles.FirstOrDefault(b => b.Id == msg.BattleId);

            if (battle == null)
            {
                Debug.LogWarning($"Cannot find battle with Id {msg.BattleId}");
                return;
            }

            if (battle.CurrentHeroId != msg.RequesterHeroId)
            {
                Debug.LogWarning($"End turn requester is not currently active! Hacking ?");
                return;
            }

            // 2. Set movement and action points to zero
            if (msg.RequesterHeroId != 0)
            {
                this.battleService.NullifyHeroPoints(msg.RequesterHeroId, msg.IsDefend);
            }

            if (msg.RequesterUnitId != 0)
            {
                this.battleService.NullifyUnitPoints(msg.RequesterHeroId, msg.RequesterUnitId, msg.IsDefend);
            }


            // 2. Switch turns if the remaining time is more than 5 seconds
            this.battleService.SwitchTurn(battle);
        }
    }
}
