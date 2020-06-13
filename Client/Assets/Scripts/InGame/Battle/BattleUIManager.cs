using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Network.MessageHandlers;
using Assets.Scripts.Shared.NetMessages.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI
        attacker = default,
        defender = default,
        log = default,
        timeLeft = default;

    public Button EndTurnButton;
    public Text EndTurnButtonText;
    private const string END_TURN_BTN_DEFAULT_TEXT = "END TURN";
    private const string END_TURN_BTN_INACTIVE_TEXT = "ENEMY TURN";

    private void Start()
    {
        this.EndTurnButtonText = this.EndTurnButton.GetComponentInChildren<Text>();
        this.OnStartBattle();
        OnSwitchTurnEventHandler.OnSwitchTurn += OnSwitchTurn;
        BattleManager.OnRemainingTimeUpdate += OnRemainingTimeUpdate;
        BattleData.OnActionsEnabledChange += OnActionsEnabledChange;
    }

    public void OnEndTurnBtnClick()
    {
        BattleManager.Instance.EndTurn();
    }

    private void OnActionsEnabledChange(bool enabled)
    {
        this.EndTurnButton.interactable = enabled;
        if (enabled)
        {
            this.EndTurnButtonText.text = END_TURN_BTN_DEFAULT_TEXT;
        }
        else
        {
            this.EndTurnButtonText.text = END_TURN_BTN_INACTIVE_TEXT;
        }
    }

    private void OnRemainingTimeUpdate(int remainingTime)
    {
        this.timeLeft.text = remainingTime.ToString();
    }

    private void OnStartBattle()
    {
        var bd = DataManager.Instance.BattleData;
        var attackerName = bd.AttackerHero.Class.ToString();
        var defenderName = bd.DefenderHero.Class.ToString();

        this.attacker.text = attackerName;
        this.defender.text = defenderName;
        this.timeLeft.text = bd.RemainingTimeForThisTurn.ToString();
        this.log.text = $"{Time.time}: Starting Battle between {attackerName}({bd.AttackerId}) and {defenderName}({bd.DefenderId})";
    }

    private void OnSwitchTurn(Net_SwitchTurnEvent @event)
    {
        var bd = DataManager.Instance.BattleData;
        this.timeLeft.text = bd.RemainingTimeForThisTurn.ToString();
        this.log.text += $"\n{Time.time}: New Turn! Current player is the {bd.Turn}. ActionsEnabled: {bd.ActionsEnabled}";
    }
}
