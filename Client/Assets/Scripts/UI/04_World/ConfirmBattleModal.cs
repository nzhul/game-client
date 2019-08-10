using Assets.Scripts.Data;
using Assets.Scripts.Network;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World;
using Assets.Scripts.Shared.NetMessages.World.Models;
using Assets.Scripts.UI.Modals;
using UnityEngine.UI;

public class ConfirmBattleModal : Modal<ConfirmBattleModal>
{
    private DataManager _dm;
    private BattleScenario scenario;
    private Hero attackingHero;
    private Hero defendingHero;

    public Toggle autoCombatToggle;

    public void OnEngageBtnPressed()
    {
        Net_StartBattleRequest msg = BuildRequest();

        NetworkClient.Instance.SendServer(msg);
    }

    private Net_StartBattleRequest BuildRequest()
    {
        var msg = new Net_StartBattleRequest();
        msg.AttackerId = this.attackingHero.Id;
        msg.DefenderId = this.defendingHero.Id;

        switch (this.scenario)
        {
            case BattleScenario.HUvsMonsterAI:
                msg.AttackerType = PlayerType.Human;
                msg.DefenderType = PlayerType.MonsterAI;
                break;
            case BattleScenario.HUAIvsMonsterAI:

                msg.AttackerType = PlayerType.HumanAI;
                msg.DefenderType = PlayerType.MonsterAI;
                break;
            case BattleScenario.HUvsHU:
                msg.AttackerType = PlayerType.Human;
                msg.DefenderType = PlayerType.Human;
                break;
            case BattleScenario.MonsterAIvsHU:
                // Not implemented
                break;
            case BattleScenario.MonsterAIvsHUAI:
                // Not implemented
                break;
            default:
                break;
        }

        return msg;
    }

    public void Open(BattleScenario scenario,
        Hero attackingHero = null,
        Hero defendingHero = null)
    {
        this._dm = DataManager.Instance;
        this.scenario = scenario;
        this.attackingHero = attackingHero;
        this.defendingHero = defendingHero;
        this.autoCombatToggle.isOn = this._dm.BattleData.UseAutoBattles;
        this.autoCombatToggle.onValueChanged.AddListener(delegate
        {
            this.AutoCombatValueChange(this.autoCombatToggle);
        });

        base.Open();
    }

    public void OnFleeBtnPressed()
    {
        base.Close();
    }

    public void OnOutsideClick()
    {
        base.Close();
    }

    private void AutoCombatValueChange(Toggle change)
    {
        if (autoCombatToggle != null)
        {
            _dm.BattleData.UseAutoBattles = this.autoCombatToggle.isOn;
            _dm.Save();
        }
    }
}
