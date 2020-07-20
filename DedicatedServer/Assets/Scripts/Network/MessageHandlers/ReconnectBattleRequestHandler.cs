using System.Linq;
using Assets.Scripts.Shared.NetMessages.World.ClientServer;
using Assets.Scripts.Shared.NetMessages.World.ServerClient;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class ReconnectBattleRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_ReconnectBattleRequest msg = (Net_ReconnectBattleRequest)input;

            // TODO: User should not be able to just reconnect to any Game.
            // Server should do API call to check what is the gameId of the user.

            var connection = NetworkServer.Instance.Connections[connectionId];
            connection.GameId = msg.GameId;

            var battle = NetworkServer.Instance.ActiveBattles.FirstOrDefault(x => x.Id == msg.BattleId);

            var isAttacker = battle.AttackerArmy.UserId == connection.UserId;
            if (isAttacker)
            {
                battle.AttackerDisconnected = false;
                battle.AttackerConnectionId = connectionId;
            }
            else
            {
                battle.DefenderDisconnected = false;
                battle.DefenderConnectionId = connectionId;
            }


            Net_OnStartBattle rmsg = new Net_OnStartBattle
            {
                BattleId = battle.Id,
                AttackerArmyId = battle.AttackerArmyId,
                DefenderArmyId = battle.DefenderArmyId,
                BattleScenario = battle.BattleScenario,
                SelectedUnitId = battle.SelectedUnit.Id,
                AttackerType = battle.AttackerType,
                DefenderType = battle.DefenderType,
                Turn = battle.Turn
            };

            NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);
        }
    }
}
