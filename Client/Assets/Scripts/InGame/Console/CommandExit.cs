using UnityEngine;

namespace Assets.Scripts.InGame.Console
{
    public class CommandExit : ConsoleCommand
    {
        public CommandExit()
        {
            Name = "Exit";
            Command = "exit";
            Description = "Exits the application";
            Help = "Use this command with no arguments to force Unity to quit!";

            AddCommandToConsole();
        }

        public override void RunCommand(string[] arguments)
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }

        public static CommandExit CreateCommand()
        {
            return new CommandExit();
        }
    }
}
