#pragma warning disable CS0618 // Type or member is obsolete
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Shared.NetMessages;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkServer : MonoBehaviour
{
    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const int BYTE_SIZE = 1024;

    private byte reliableChannel;
    private int hostId;
    private int webHostId;
    private byte error;

    private bool isStarted;

    private Dictionary<int, ServerConnection> _connections = new Dictionary<int, ServerConnection>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
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

        int recievingHostId;
        int connectionId;
        int channelId;
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recievingHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);

        switch (type)
        {
            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMessage msg = (NetMessage)formatter.Deserialize(ms);

                OnData(connectionId, channelId, recievingHostId, msg);
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log(string.Format("User {0} has connected throught host {1}", connectionId, recievingHostId));
                break;
            case NetworkEventType.DisconnectEvent:
                if (_connections.ContainsKey(connectionId))
                {
                    // TODO: Call the API at -> "/api/auth/logout/{userId}" to logout the user.
                    Debug.Log(string.Format("{0} has disconnected from the server!", _connections[connectionId].Username));
                    _connections.Remove(connectionId);
                }
                else
                {
                    Debug.Log("Someone disconnected from the server!");
                }
                break;
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.BroadcastEvent:
                break;
            default:
                break;
        }
    }

    private void OnData(int connectionId, int channelId, int recievingHostId, NetMessage msg)
    {
        switch (msg.OperationCode)
        {
            case NetOperationCode.None:
                Debug.Log("Unexpected NET OperationCode");
                break;
            case NetOperationCode.AuthRequest:
                ConnectRequest(connectionId, channelId, recievingHostId, (Net_AuthRequest)msg);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// The user will authenticate directly throu the API.
    /// Then he will send his token and username to the dedicated server.
    /// This way we can track his connection and online status.
    /// </summary>
    private void ConnectRequest(int connectionId, int channelId, int recievingHostId, Net_AuthRequest msg)
    {
        Net_OnAuthRequest rmsg = new Net_OnAuthRequest();

        if (msg.IsValid())
        {
            rmsg.Success = 1;
            rmsg.ConnectionId = connectionId;
            this._connections.Add(connectionId, new ServerConnection
            {
                Id = msg.Id,
                Token = msg.Token,
                Username = msg.Username
            });

            Debug.Log(string.Format("{0} has connected to the server!", msg.Username));
        }
        else
        {
            rmsg.Success = 0;
            rmsg.ErrorMessage = "Invalid connection request!";
        }

        SendClient(recievingHostId, connectionId, rmsg);
    }

    private void SendClient(int recievingHostId, int connectionId, NetMessage msg)
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
}
#pragma warning restore CS0618 // Type or member is obsolete