using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Network.MessageHandlers;
using Assets.Scripts.Shared.NetMessages.Battle;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public Text Attacker;
    public Text Defender;
    public Text TimeLeft;
    public Text Turn;
    public Text Log;

    public string attackerTemplate = "ATTACKER: {0}";
    public string defenderTemplate = "DEFENDER: {0}";
    public string timeleftTemplate = "TIME LEFT: {0}";
    public string turnTemplate = "TURN: {0}";
    public string log = "";

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
        this.TimeLeft.text = string.Format(timeleftTemplate, remainingTime);
    }

    private void OnStartBattle()
    {
        var bd = DataManager.Instance.BattleData;
        var attackerName = bd.AttackerHero.Name;
        var defenderName = bd.DefenderHero.Name;

        this.Attacker.text = string.Format(attackerTemplate, attackerName);
        this.Defender.text = string.Format(defenderTemplate, defenderName);
        this.TimeLeft.text = string.Format(timeleftTemplate, bd.RemainingTimeForThisTurn);
        this.Turn.text = string.Format(turnTemplate, bd.Turn);
        this.Log.text = $"{Time.time}: Starting Battle between {attackerName}({bd.AttackerId}) and {defenderName}({bd.DefenderId})";
    }

    private void OnSwitchTurn(Net_SwitchTurnEvent @event)
    {
        var bd = DataManager.Instance.BattleData;
        this.TimeLeft.text = string.Format(timeleftTemplate, bd.RemainingTimeForThisTurn);
        this.Turn.text = string.Format(turnTemplate, bd.Turn);
        this.Log.text += $"\n{Time.time}: New Turn! Current player is the {bd.Turn}. ActionsEnabled: {bd.ActionsEnabled}";
    }
}
