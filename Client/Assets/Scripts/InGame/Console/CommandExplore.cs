using System;
using Assets.Scripts.Data;
using Assets.Scripts.Network.Shared.DataModels;
using Assets.Scripts.Network.Shared.Http;
using BestHTTP;
using UnityEngine;

namespace Assets.Scripts.InGame.Console
{
    public class CommandExplore : ConsoleCommand
    {
        public CommandExplore()
        {
            Name = "Explore";
            Command = "explore";
            Description = "Explore specific or all region/s";
            Help = "Arguments: all, {regionId}";

            AddCommandToConsole();
        }

        public override void RunCommand(string[] arguments)
        {
            string msg = string.Empty;

            string endpoint = $"avatars/{DataManager.Instance.Avatar.Id}/explore";

            var exploreParams = new ExploreParams
            {
                Type = DwellingType.Waypoint
            };

            RequestManager.Instance.Put<ExploreParams>(endpoint, exploreParams, DataManager.Instance.Token, OnExploreRequestFinished);
        }

        private void OnExploreRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            if (NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
            {
                DeveloperConsole.Instance.AddMessageToConsole("Explore request successful!");
                Debug.Log("Explore request successful!");
            }
            else
            {
                Debug.LogWarning("Explore request failed");
            }
        }

        public static CommandExplore CreateCommand()
        {
            return new CommandExplore();
        }
    }

    public class ExploreParams
    {
        public DwellingType Type { get; set; }

        public int[] RegionIds { get; set; }
    }
}
