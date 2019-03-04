namespace Assets.Scripts.InGame.Console
{
    public class CommandClear : ConsoleCommand
    {
        public CommandClear()
        {
            Name = "Clear";
            Command = "clear";
            Description = "Clears the developer console";
            Help = "Use this command with no arguments clear the developer console!";

            AddCommandToConsole();
        }

        public override void RunCommand(string[] arguments)
        {
            DeveloperConsole.Instance.ClearConsole();
        }

        public static CommandClear CreateCommand()
        {
            return new CommandClear();
        }
    }
}
