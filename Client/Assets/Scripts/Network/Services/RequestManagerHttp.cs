﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Assets.Scripts.Network.Services.HTTP;
using Assets.Scripts.Network.Services.HTTP.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Network.Services
{
    public class RequestManagerHttp : MonoBehaviour
    {
        public static string SERVER_ROOT = "http://localhost:5000/api/";

        private static RequestManagerHttp _instance;

        public static RequestManagerHttp Instance
        {
            get
            {
                return _instance;
            }
        }

        public static HttpClient Client { get; private set; }

        public static IGameService GameService { get; private set; }

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

            if (Client == null)
            {
                Client = new HttpClient();
                Client.BaseAddress = new Uri(SERVER_ROOT);
                GameService = new GameService();
            }
        }

        public void SetToken(string token)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}