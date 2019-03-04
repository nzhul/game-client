using System;
using System.Collections.Generic;
using Assets.Scripts.Data;
using Newtonsoft.Json;

namespace Assets.Scripts.InGame.Console
{
    public class CommandShow : ConsoleCommand
    {
        private List<string> _validArguments;

        public CommandShow()
        {
            Name = "Show";
            Command = "show";
            Description = "Show cached data for specified entity";
            Help = "Arguments: 'avatar' 'dwellings' 'waypoints'";

            AddCommandToConsole();
        }

        public override void RunCommand(string[] arguments)
        {
            string msg = string.Empty;

            if (arguments.Length > 1)
            {
                string argument = arguments[1];

                switch (argument)
                {
                    case "avatar":
                        msg = JsonConvert.SerializeObject(DataManager.Instance.Avatar, Formatting.Indented);
                        break;
                    case "dwellings":
                        msg = JsonConvert.SerializeObject(DataManager.Instance.Avatar.dwellings, Formatting.Indented);
                        break;
                    case "waypoints":
                        msg = JsonConvert.SerializeObject(DataManager.Instance.Avatar.waypoints, Formatting.Indented);
                        break;
                    case "regions":
                        msg = JsonConvert.SerializeObject(DataManager.Instance.Regions, Formatting.Indented);
                        break;
                    case "-help":
                        msg = this.Description + Environment.NewLine + this.Help;
                        break;
                    default:
                        msg = "Unknown agument. Use show -help for list of available arguments";
                        break;
                }
            }
            else
            {
                msg = "This command require atleast one argument. Use show -help for more information.";
            }

            DeveloperConsole.Instance.AddMessageToConsole(msg);
        }

        public static CommandShow CreateCommand()
        {
            return new CommandShow();
        }
    }
}
