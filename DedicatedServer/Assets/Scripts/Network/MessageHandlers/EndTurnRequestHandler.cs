using System.Linq;
using Assets.Scripts.Games;
using Assets.Scripts.Network.Services.TCP;
using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Shared.NetMessages.Battle;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
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
            var game = GameManager.Instance.GetGameByConnectionId(connectionId);

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

            //if (battle.CurrentUnitId != msg.RequesterUnitId)
            //{
            //    Debug.LogWarning($"End turn requester is not currently active! Hacking ?");
            //    return;
            //}

            // 2. Update last activity
            battle.UpdateLastActivity(msg.RequesterArmyId);

            // 3. Set movement and action points to zero

            if (msg.RequesterUnitId != 0)
            {
                this.battleService.NullifyUnitPoints(game.Id, msg.RequesterArmyId, msg.RequesterUnitId, msg.IsDefend);
            }


            // 4. Switch turns if the remaining time is more than 5 seconds
            this.battleService.SwitchTurn(battle);
        }
    }
}
