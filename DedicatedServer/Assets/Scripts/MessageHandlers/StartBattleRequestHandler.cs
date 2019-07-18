using Assets.Scripts.Shared.NetMessages.Battle.Models;
using Assets.Scripts.Shared.NetMessages.World;
using Assets.Scripts.Shared.NetMessages.World.Models;
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
                BattleScenario scenario = this.ResolveBattleScenario(msg.AttackerType, msg.DefenderType);
                rmsg.Success = 1;
                rmsg.AttackerId = msg.AttackerId;
                rmsg.DefenderId = msg.DefenderId;
                rmsg.BattleScenario = scenario;

                // 1. Add new record into NetworkServer.Instance.ActiveBattles
                // 2. Send OnStartBattle back to the client.

                Battle newBattle = new Battle()
                {
                    Id = Guid.NewGuid(),
                    AttackerId = msg.AttackerId,
                    DefenderId = msg.DefenderId,
                    AttackerType = msg.AttackerType,
                    DefenderType = msg.DefenderType,
                    BattleScenario = scenario
                };

                rmsg.BattleId = newBattle.Id;

                this.ConfigurePlayerReady(newBattle, scenario);

                NetworkServer.Instance.ActiveBattles.Add(newBattle);
                NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);
                OnBattleStarted?.Invoke(newBattle);
            }
        }

        private void ConfigurePlayerReady(Battle newBattle, BattleScenario scenario)
        {
            switch (scenario)
            {
                case BattleScenario.HUvsMonsterAI:
                    newBattle.AttackerReady = false;
                    newBattle.DefenderReady = true;
                    break;
                case BattleScenario.HUAIvsMonsterAI:
                    newBattle.AttackerReady = true;
                    newBattle.DefenderReady = true;
                    break;
                case BattleScenario.HUvsHU:
                    newBattle.AttackerReady = false;
                    newBattle.DefenderReady = false;
                    break;
                case BattleScenario.MonsterAIvsHU:
                    newBattle.AttackerReady = true;
                    newBattle.DefenderReady = false;
                    break;
                case BattleScenario.MonsterAIvsHUAI:
                    newBattle.AttackerReady = true;
                    newBattle.DefenderReady = true;
                    break;
                case BattleScenario.Unknown:
                    break;
                default:
                    break;
            }
        }

        private BattleScenario ResolveBattleScenario(PlayerType attackerType, PlayerType defenderType)
        {
            if (attackerType == PlayerType.Human && defenderType == PlayerType.MonsterAI)
            {
                return BattleScenario.HUvsMonsterAI;
            }
            else if (attackerType == PlayerType.HumanAI && defenderType == PlayerType.MonsterAI)
            {
                return BattleScenario.HUAIvsMonsterAI;
            }
            else if (attackerType == PlayerType.Human && defenderType == PlayerType.Human)
            {
                return BattleScenario.HUvsHU;
            }
            else if (attackerType == PlayerType.MonsterAI && defenderType == PlayerType.Human)
            {
                return BattleScenario.MonsterAIvsHU;
            }
            else if (attackerType == PlayerType.MonsterAI && defenderType == PlayerType.HumanAI)
            {
                return BattleScenario.MonsterAIvsHUAI;
            }

            return BattleScenario.Unknown;
        }
    }

    // # Battle Scenarios
    // 1. HU vs AI Monster
    // 2. AI Human vs AI Monster (AutoCombat)
    // 3. Human vs Human (PvP)

    // # Not supported scenarios
    // 1. AI HU vs AI HU (Autocombat)
    // 2. HU vs AI HU (one of the players is Autocombat)
}
