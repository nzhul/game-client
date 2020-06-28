using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.InGame.Console
{
    public abstract class ConsoleCommand
    {
        public string Name { get; protected set; }
        public string Command { get; protected set; }
        public string Description { get; protected set; }
        public string Help { get; protected set; }

        public void AddCommandToConsole()
        {
            DeveloperConsole.AddCommandsToConsole(Command, this);
        }

        public abstract void RunCommand(string[] arguments);
    }

    public class DeveloperConsole : MonoBehaviour
    {
        public static DeveloperConsole Instance { get; private set; }
        public static bool ConsoleIsOpen { get; private set; }
        public static Dictionary<string, ConsoleCommand> Commands { get; private set; }

        [Header("UI Components")]
        public GameObject consolePanel;
        public Text consoleText;
        public Text inputText;
        public InputField consoleInput;

        private void Awake()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;
            Commands = new Dictionary<string, ConsoleCommand>();
        }

        private void Start()
        {
            consolePanel.SetActive(false);
            CreateCommands();
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        public static void AddCommandsToConsole(string _name, ConsoleCommand _command)
        {
            if (!Commands.ContainsKey(_name))
            {
                Commands.Add(_name, _command);
            }
        }

        public void ClearConsole()
        {
            consoleText.text = "";
        }

        private void HandleLog(string logMessage, string stackTrace, LogType type)
        {
            string _message = "[" + type.ToString() + "] " + logMessage;
            AddMessageToConsole(_message);
        }

        private void CreateCommands()
        {
            CommandExit.CreateCommand();
            CommandShow.CreateCommand();
            CommandClear.CreateCommand();
            //CommandExplore.CreateCommand();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                consolePanel.SetActive(!consolePanel.activeInHierarchy);
                if (consolePanel.activeInHierarchy)
                {
                    ConsoleIsOpen = true;
                    FocusAndClear();
                }
                else
                {
                    ConsoleIsOpen = false;
                }
            }

            if (consolePanel.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (inputText.text != "")
                    {
                        AddMessageToConsole(inputText.text);
                        ParseInput(inputText.text);
                        FocusAndClear();
                    }
                }
            }
        }

        private void FocusAndClear()
        {
            consoleInput.text = "";
            EventSystem.current.SetSelectedGameObject(consoleInput.gameObject, null);
            consoleInput.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        public void AddMessageToConsole(string msg)
        {
            consoleText.text += msg + "\n";
        }

        private void ParseInput(string input)
        {
            string[] _input = input.Split(null);

            if (_input.Length == 0 || _input == null)
            {
                Debug.LogWarning("Command not recognized.");
                return;
            }

            if (!Commands.ContainsKey(_input[0]))
            {
                Debug.LogWarning("Command not recognized.");
            }
            else
            {
                Commands[_input[0]].RunCommand(_input);
            }
        }
    }
}
