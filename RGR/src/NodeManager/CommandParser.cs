using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace NodeManager {

    public class CommandParser {
        private readonly IDictionary<string, int> СommandArgumentsCount = new Dictionary<string, int> ();
        private readonly IDictionary<string, string> CommandHelper = new Dictionary<string, string> ();

        public void Init () 
        {
            AddCommand (Command.GET, 1, $"Command signature: {Command.GET} <service_name>");
            AddCommand (Command.GETALL, 0, $"Command signature: {Command.GETALL}");
            AddCommand (Command.START, 2, $"Command signature: {Command.START} <service_name> <port>");
            AddCommand (Command.STOP, 2, $"Command signature: {Command.STOP} <service_name> <port>");
            AddCommand (Command.KILL, 0, $"Command signature: {Command.KILL}");
        }

        public void AddCommand (string name, int commandArgumentCount, string help) 
        {
            СommandArgumentsCount.Add (name, commandArgumentCount);
            CommandHelper.Add (name, help);
        }

        public bool IsValidString (string commandString) 
        {
            string[] commandArguments = commandString.Split (' ');
            if (commandArguments.Length == 0) 
            {
                return false;
            }
            string commandName = commandArguments.ElementAt (0);
            if (!СommandArgumentsCount.ContainsKey (commandName) || commandArguments.Length - 1 != СommandArgumentsCount[commandName]) 
            {
                return false;
            }

            return true;
        }

        public void WriteHelpForCommand (string commandString) 
        {
            string[] commandArguments = commandString.Split (' ');
            if (commandArguments.Length == 0) 
            {
                Console.WriteLine ($"Error: because of emptiness: \"{commandString}\"");
                return;
            }
            string commandName = commandArguments.ElementAt (0);
            if (!CommandHelper.ContainsKey (commandName)) 
            {
                Console.WriteLine ($"Unknown command: \"{commandString}\"");
                return;
            }
            Console.WriteLine (CommandHelper[commandName]);
        }
    }
}