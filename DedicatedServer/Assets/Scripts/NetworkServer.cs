#pragma warning disable CS0618 // Type or member is obsolete
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts;
using Assets.Scripts.Configuration;
using Assets.Scripts.MessageHandlers;
using Assets.Scripts.Network.Shared.Http;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.Battle.Models;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkServer : MonoBehaviour
{
    public static NetworkServer Instance { get; private set; }

    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const int BYTE_SIZE = 1024;

    private byte reliableChannel;
    private int hostId;
    private int webHostId;
    private byte error;

    private bool isStarted;

    private Dictionary<int, IMessageHandler> _messageHandlers;

    private DisconnectEventHandler _disconnectEventHandler;

    public ServerConfiguration ServerConfiguration;

    public Dictionary<CreatureType, UnitConfiguration> UnitConfigurations = new Dictionary<CreatureType, UnitConfiguration>();

    public LoginResponse Admin;

    public static event Action AdminAuthenticated;

    public Dictionary<int, ServerConnection> Connections = new Dictionary<int, ServerConnection>();

    public Dictionary<int, Region> Regions = new Dictionary<int, Region>();

    public List<Battle> ActiveBattles = new List<Battle>();

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        this.ServerConfiguration = this.LoadServerConfiguration();
        this.LogInAdmin();
        NetworkServer.AdminAuthenticated += OnAdminAuthenticated;
        Init();
    }

    private void OnAdminAuthenticated()
    {
        this.LoadUnitConfigurations();
    }

    private void LogInAdmin()
    {
        var loginInput = new LoginInput
        {
            username = this.ServerConfiguration.AName,
            password = this.ServerConfiguration.AKey
        };

        RequestManager.Instance.Post<LoginInput>("auth/login", loginInput, OnLoginRequestFinished);
    }

    private void OnLoginRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        if (response == null || response.StatusCode != 200)
        {
            Debug.LogError("Admin login request failed!");
            return;
        }

        string json = response.DataAsText;
        //LoginResponse loginInfo = JsonUtility.FromJson<LoginResponse>(json);
        LoginResponse loginInfo = JsonConvert.DeserializeObject<LoginResponse>(json);

        if (loginInfo == null)
        {
            Debug.LogError("Cannot parse admin login information!");
            return;
        }

        Debug.Log("Admin authenticated successfully!");
        this.Admin = loginInfo;
        NetworkServer.AdminAuthenticated?.Invoke();
    }

    private void LoadUnitConfigurations()
    {
        RequestManager.Instance.Get("unit-configurations", new string[] { }, this.Admin.tokenString, OnLoadUnitConfigurationsFinished);
    }

    private void OnLoadUnitConfigurationsFinished(HTTPRequest request, HTTPResponse response)
    {
        if (NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
        {
            string json = response.DataAsText;
            this.UnitConfigurations = JsonConvert.DeserializeObject<Dictionary<CreatureType, UnitConfiguration>>(json);
            Debug.Log("Unit configurations loaded successfully!");
        }
        else
        {
            Debug.LogError("Cannot load unit configurations!");
        }
    }

    private ServerConfiguration LoadServerConfiguration()
    {
        TextAsset json = Resources.Load("Config") as TextAsset;
        //return JsonUtility.FromJson<ServerConfiguration>(json.text);
        return JsonConvert.DeserializeObject<ServerConfiguration>(json.text);
    }

    private void Init()
    {
        NetworkTransport.Init();

        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannel = connectionConfig.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(connectionConfig, MAX_USER);

        // SERVER only code
        hostId = NetworkTransport.AddHost(topo, PORT, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);

        Debug.Log(string.Format("Opening connection on port {0} and webport {1}", PORT, WEB_PORT));

        isStarted = true;

        RegisterMessageHandlers();
    }

    private void RegisterMessageHandlers()
    {
        _disconnectEventHandler = new DisconnectEventHandler();
        _messageHandlers = new Dictionary<int, IMessageHandler>
        {
            { NetOperationCode.AuthRequest, new AuthRequestHandler() },
            { NetOperationCode.WorldEnterRequest, new WorldEnterRequestHandler() },
            { NetOperationCode.MapMovementRequest, new MapMovementRequestHandler() },
            { NetOperationCode.TeleportRequest, new TeleportRequestHandler() },
            { NetOperationCode.StartBattleRequest, new StartBattleRequestHandler() },
            { NetOperationCode.ConfirmLoadingBattleScene, new ConfirmLoadingBattleSceneHandler() },
            { NetOperationCode.EndTurnRequest, new EndTurnRequestHandler() }
        };
    }

    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    private void Update()
    {
        UpdateMessagePump();
    }

    private void UpdateMessagePump()
    {
        if (!isStarted)
        {
            return;
        }

        byte[] recBuffer = new byte[BYTE_SIZE];

        NetworkEventType type = NetworkTransport.Receive(out int recievingHostId, out int connectionId, out int channelId, recBuffer, BYTE_SIZE, out int dataSize, out error);

        switch (type)
        {
            case NetworkEventType.DataEvent:
                OnData(connectionId, channelId, recievingHostId, recBuffer);
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log(string.Format("User {0} has connected throught host {1}", connectionId, recievingHostId));
                break;
            case NetworkEventType.DisconnectEvent:
                _disconnectEventHandler.Handle(connectionId);
                break;
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.BroadcastEvent:
                break;
            default:
                break;
        }
    }

    private void OnData(int connectionId, int channelId, int recievingHostId, byte[] recBuffer)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(recBuffer);
        NetMessage msg = (NetMessage)formatter.Deserialize(ms);

        _messageHandlers[msg.OperationCode].Handle(connectionId, channelId, recievingHostId, msg);
    }

    public void SendClient(int recievingHostId, int connectionId, NetMessage msg)
    {
        byte[] buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        if (recievingHostId == 0)
        {
            NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);
        }
        else
        {
            NetworkTransport.Send(webHostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);
        }
    }

    public int GetConnectionIdByHeroId(int heroId)
    {
        var pair = this.Connections.FirstOrDefault(c => c.Value.Avatar.Heroes.Any(h => h.Id == heroId));

        if (!pair.Equals(default(KeyValuePair<int, ServerConnection>))) // null check for keyValuePair
        {
            return pair.Value.ConnectionId;
        }
        else
        {
            return 0;
        }
    }

    public Hero GetHeroById(int heroId)
    {
        var pair = this.Regions.FirstOrDefault(c => c.Value.Heroes.Any(h => h.Id == heroId));

        if (!pair.Equals(default(KeyValuePair<int, Region>))) // null check for keyValuePair
        {
            return pair.Value.Heroes.FirstOrDefault(h => h.Id == heroId);
        }
        else
        {
            return null;
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete