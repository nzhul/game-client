using Assets.Scripts.Data;
using Assets.Scripts.Network;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World;
using Assets.Scripts.UI.Modals;
using UnityEngine.UI;

public class ConfirmBattleModal : Modal<ConfirmBattleModal>
{
    private DataManager _dm;
    private HeroView heroView;
    private MonsterPack monsterPack;

    public Toggle autoCombatToggle;

    public void OnEngageBtnPressed()
    {
        Net_StartBattleRequest msg = new Net_StartBattleRequest
        {
            HeroId = this.heroView.hero.id,
            MonsterId = this.monsterPack.id
        };

        NetworkClient.Instance.SendServer(msg);
    }

    public void Open(HeroView heroView, MonsterPack monsterPack)
    {
        this._dm = DataManager.Instance;

        this.heroView = heroView;
        this.monsterPack = monsterPack;
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
