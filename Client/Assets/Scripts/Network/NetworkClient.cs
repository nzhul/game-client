#pragma warning disable CS0618 // Type or member is obsolete

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Network.MessageHandlers;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Network
{
    public class NetworkClient : MonoBehaviour
    {
        public static NetworkClient Instance { get; private set; }

        private const int MAX_USER = 100;
        private const int PORT = 26000;
        private const int WEB_PORT = 26001;
        private const int BYTE_SIZE = 1024; // 1402

        private const string SERVER_IP = "127.0.0.1";

        private byte reliableChannel;
        private int connectionId;
        private int hostId;
        private byte error;

        public bool IsStarted;

        #region Server Events

        private DisconnectEventHandler _disconnectEventHandler;
        private Dictionary<int, IMessageHandler> _messageHandlers;

        #endregion

        private void Start()
        {
            Instance = this;
            Init();
            RegisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            _disconnectEventHandler = new DisconnectEventHandler();
            _messageHandlers = new Dictionary<int, IMessageHandler>
        {
            { NetOperationCode.OnAuthRequest, new OnAuthRequestHandler() },
            //{ NetOperationCode.OnWorldEnter, new OnWorldEnterRequestHandler() },
            { NetOperationCode.OnMapMovement, new OnMapMovementRequestHandler() },
            { NetOperationCode.OnTeleport, new OnTeleportRequestRequestHandler() },
            { NetOperationCode.OnStartBattle, new OnStartBattleRequestHandler() },
            { NetOperationCode.OnSwitchTurn, new OnSwitchTurnEventHandler() },
            {NetOperationCode.OnStartGame, new OnStartGameHandler() }
        };
        }

        private void Update()
        {
            UpdateMessagePump();
        }

        private void Init()
        {
            NetworkTransport.Init();

            ConnectionConfig connectionConfig = new ConnectionConfig();
            reliableChannel = connectionConfig.AddChannel(QosType.Reliable);

            HostTopology topo = new HostTopology(connectionConfig, MAX_USER);

            // Client only code
            hostId = NetworkTransport.AddHost(topo, 0);

#if UNITY_WEBGL && !UNITY_EDITOR
        // Web Client
        NetworkTransport.Connect(hostId, SERVER_IP, WEB_PORT, 0, out error);
        Debug.Log("Connecting from Web");
#else
            // Standalone Client
            connectionId = NetworkTransport.Connect(hostId, SERVER_IP, PORT, 0, out error);
            Debug.Log("Connecting from standalone");
#endif

            Debug.Log(string.Format("Attempting to connect on {0} ...", SERVER_IP));

            IsStarted = true;
        }

        public void ShutDown()
        {
            IsStarted = false;
            NetworkTransport.Shutdown();
        }

        public void UpdateMessagePump()
        {
            if (!IsStarted)
            {
                return;
            }

            int recievingHostId; // Is this from Web ? Or standalone
            int connectionId; // Which user is sending me this ?
            int channelId; // Which lane is he sending that message from

            byte[] recBuffer = new byte[BYTE_SIZE];
            int dataSize;

            NetworkEventType type = NetworkTransport.Receive(out recievingHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);

            switch (type)
            {
                case NetworkEventType.DataEvent:
                    OnData(connectionId, channelId, recievingHostId, recBuffer);
                    break;
                case NetworkEventType.ConnectEvent:
                    Debug.Log("We have connected to the server");
                    break;
                case NetworkEventType.DisconnectEvent:
                    _disconnectEventHandler.Handle(connectionId);
                    break;
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.BroadcastEvent:
                    Debug.Log("Unexpected network event type!");
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

            //switch (msg.OperationCode)
            //{
            //    case NetOperationCode.None:
            //        Debug.Log("Unexpected NETOperationCode");
            //        break;
            //    case NetOperationCode.OnAuthRequest:
            //        Debug.Log("Client has authenticated to dedicated server!");
            //        break;
            //    case NetOperationCode.OnWorldEnter:
            //        if (OnWorldEnter != null)
            //        {
            //            OnWorldEnter((Net_OnWorldEnter)msg);
            //        }
            //        // TODO: Raise static event. Map manager should wait for this event to occur before rendering the map.
            //        // All client server interaction should work in a similar way:
            //        // 1. Client send request to the server and enters "waiting state"
            //        // 2. Server sends back result message
            //        // 3. Client consumes the message by listening to the event and is no longer in waiting state
            //        break;
            //    case NetOperationCode.OnMapMovement:
            //        if (OnMapMovement != null)
            //        {
            //            OnMapMovement((Net_OnMapMovement)msg);
            //        }
            //        break;
            //    default:
            //        break;
            //}
        }

        public void SendServer(NetMessage msg)
        {
            byte[] buffer = new byte[BYTE_SIZE];

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(buffer);
            formatter.Serialize(ms, msg);

            NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete