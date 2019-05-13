using Assets.Scripts.Shared.NetMessages.Battle.Models;
using Assets.Scripts.Shared.NetMessages.World;
using System;

namespace Assets.Scripts.MessageHandlers
{
    public class StartBattleRequestHandler : IMessageHandler
    {
        public static event Action<Battle> OnBattleStarted;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_StartBattleRequest msg = (Net_StartBattleRequest)input;
            Net_OnStartBattle rmsg = new Net_OnStartBattle();

            if (msg.IsValid())
            {
                rmsg.Success = 1;
                rmsg.HeroId = msg.HeroId;
                rmsg.MonsterId = msg.MonsterId;

                // 1. Add new record into NetworkServer.Instance.ActiveBattles
                // 2. Send OnStartBattle back to the client.

                Battle newBattle = new Battle()
                {
                    AttackerId = msg.HeroId,
                    DefenderId = msg.MonsterId,
                    AttackerIsAI = false,
                    DefenderIsAI = true
                };

                NetworkServer.Instance.ActiveBattles.Add(newBattle);
                NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);
                OnBattleStarted?.Invoke(newBattle);
            }
        }
    }
}
