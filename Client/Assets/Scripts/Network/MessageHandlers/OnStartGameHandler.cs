using Assets.Scripts.Data;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Shared.NetMessages.World.ServerClient;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class OnStartGameHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            var msg = (Net_OnStartGame)input;

            Debug.Log($"Starting game with Id {msg.GameId}");

            DataManager.Instance.ActiveGameId = msg.GameId;
            GameManager.Instance.LoadScene(LevelLoader.GAME_SCENE);

            // - var game = RequestManagerHttp.GameService.GetGame(msg.GameId);
            // Store game in DataManager.
            // Load Loading Scene
            // ....
            // Load World scene [consider using the same scene !]
        }
    }
}
