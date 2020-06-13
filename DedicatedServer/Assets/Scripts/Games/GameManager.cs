using System.Collections.Generic;
using Assets.Scripts.Shared.Models;
using UnityEngine;

namespace Assets.Scripts.Games
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private List<Game> Games;

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

            this.Games = new List<Game>();
        }

        public void RegisterGame(Game game)
        {
            this.Games.Add(game);
        }

    }

}