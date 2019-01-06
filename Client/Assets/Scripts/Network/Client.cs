using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Network
{
    public class Client : MonoBehaviour
    {
        public static Client Instance { get; private set; }

        private const int MAX_USER = 100;
        private const int PORT = 26000;
        private const int WEB_PORT = 26001;
        private const int BYTE_SIZE = 1024;

        private const string SERVER_IP = "127.0.0.1";

        private byte reliableChannel;
        private int connectionId;
        private int hostId;
        private byte error;

        private bool isStarted;

        private void Start()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }

        private void Init()
        {
            NetworkTransport.Init();

            ConnectionConfig connectionConfig = new ConnectionConfig();
            this.reliableChannel = connectionConfig.AddChannel(QosType.Reliable);

            HostTopology topo = new HostTopology(connectionConfig, MAX_USER);

            // Client only code
            this.hostId = NetworkTransport.AddHost(topo, 0);


        }
    }
}
