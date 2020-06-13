using Assets.Scripts.Network.Services.TCP;
using Assets.Scripts.Network.Services.TCP.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Network.Services
{
    public class RequestManagerTcp : MonoBehaviour
    {
        private static RequestManagerTcp _instance;

        public static RequestManagerTcp Instance
        {
            get
            {
                return _instance;
            }
        }

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

            GameService = new GameService();
        }

        public static IGameService GameService { get; private set; }
    }
}
