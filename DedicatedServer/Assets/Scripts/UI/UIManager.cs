using System;
using System.Collections.Generic;
using Assets.Scripts.Network.MessageHandlers;
using Assets.Scripts.Shared.Models;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform activePlayersContainer;

    [SerializeField]
    private Button activeEntityBtn;

    [SerializeField]
    private RectTransform activeRegionsContainer;

    [SerializeField]
    private RectTransform activeBattlesContainer;

    private static UIManager _instance;

    public static UIManager Instance => _instance;

    private Dictionary<int, GameObject> PlayerButtons = new Dictionary<int, GameObject>();

    private Dictionary<int, GameObject> RegionButtons = new Dictionary<int, GameObject>();

    private Dictionary<Guid, GameObject> BattleButtons = new Dictionary<Guid, GameObject>();

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        AuthRequestHandler.OnAuth += OnAuth;
        DisconnectEventHandler.OnDisconnect += OnDisconnect;
        InnerWERHandler.OnRegionLoaded += OnRegionLoaded;
        DisconnectEventHandler.OnRegionUnload += OnRegionUnload;
        StartBattleRequestHandler.OnBattleStarted += OnBattleStarted;
    }

    public void OnBattleEnded(Guid battleId)
    {
        if (this.BattleButtons.ContainsKey(battleId))
        {
            GameObject btnToRemove = this.BattleButtons[battleId];
            Destroy(btnToRemove);
            this.BattleButtons.Remove(battleId);
        }
    }

    private void OnBattleStarted(Battle battle)
    {
        Button btn = Instantiate<Button>(activeEntityBtn, activeBattlesContainer.transform);
        btn.name = battle.AttackerId + "_" + battle.DefenderId;

        var btnText = btn.GetComponentInChildren<Text>();
        btnText.text = btn.name;
        this.BattleButtons.Add(battle.Id, btn.gameObject);
    }

    private void OnRegionUnload(int regionId)
    {
        if (this.RegionButtons.ContainsKey(regionId))
        {
            GameObject btnToRemove = this.RegionButtons[regionId];
            Destroy(btnToRemove);
            this.RegionButtons.Remove(regionId);
        }
    }

    private void OnRegionLoaded(Game region)
    {
        Button btn = Instantiate<Button>(activeEntityBtn, activeRegionsContainer.transform);
        //btn.name = region.Id + "_" + region.Name;

        var btnText = btn.GetComponentInChildren<Text>();
        //btnText.text = btn.name = region.Id + "_" + region.Name;

        this.RegionButtons.Add(region.Id, btn.gameObject);
    }

    private void OnDisconnect(ServerConnection connection)
    {
        if (this.PlayerButtons.ContainsKey(connection.ConnectionId))
        {
            GameObject btnToRemove = this.PlayerButtons[connection.ConnectionId];
            Destroy(btnToRemove);
            this.PlayerButtons.Remove(connection.ConnectionId);
        }
    }

    private void OnAuth(ServerConnection connection)
    {
        Button btn = Instantiate<Button>(activeEntityBtn, activePlayersContainer.transform);
        btn.name = connection.ConnectionId + "_" + connection.Username;

        var btnText = btn.GetComponentInChildren<Text>();
        btnText.text = connection.ConnectionId + "_" + connection.Username;

        this.PlayerButtons.Add(connection.ConnectionId, btn.gameObject);
    }
}
