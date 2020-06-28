//using System;
//using System.Collections.Generic;
//using Assets.Scripts.Data;
//using Assets.Scripts.Network.Services;
//using Assets.Scripts.Shared.Models;
//using Assets.Scripts.Shared.NetMessages.World.ServerClient;
//using BestHTTP;
//using Newtonsoft.Json;
//using UnityEngine;

//namespace Assets.Scripts.Network.MessageHandlers
//{
//    public class OnWorldEnterRequestHandler : IMessageHandler
//    {
//        public static event Action<Net_OnWorldEnter> OnWorldEnter;

//        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
//        {
//            //1. Raise event
//            var msg = (Net_OnWorldEnter)input;
//            OnWorldEnter?.Invoke(msg);

//            //2. Enable player inputs

//            if (msg.Success == 1)
//            {
//                PlayerController.Instance.EnableInputs();
//                this.LoadUnitConfigurations();
//            }
//        }

//        private void LoadUnitConfigurations()
//        {
//            RequestManager.Instance.Get("unit-configurations", new string[] { }, DataManager.Instance.Token, OnLoadUnitConfigurationsFinished);
//        }

//        private void OnLoadUnitConfigurationsFinished(HTTPRequest request, HTTPResponse response)
//        {
//            if (NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
//            {
//                string json = response.DataAsText;
//                DataManager.Instance.UnitConfigurations = JsonConvert.DeserializeObject<Dictionary<CreatureType, UnitConfiguration>>(json);
//                Debug.Log("Unit configurations loaded successfully!");
//            }
//            else
//            {
//                Debug.LogError("Cannot load unit configurations!");
//            }
//        }
//    }
//}