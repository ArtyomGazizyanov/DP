using System.Linq;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Models;
using System;

namespace NodeManager
{
    internal static class Program
	{
		private const string ConfigFileName = "../NodeConfig.json";

		private static readonly List<NodeClass> Nodes = new List<NodeClass>();

		private static void Main()
		{
			ReadConfig();
			WriteNodes();
			while (true)
			{
				Console.Write("Type node number to connect: ");
				string command = Console.ReadLine();
				if (command == "EXIT")
				{
					break;
				}
				if (!int.TryParse(command, out var nodeSerialNumber) || nodeSerialNumber < 1 || nodeSerialNumber > Nodes.Count)
				{
					continue;
				}

				NodeClass nodeModel = Nodes.ElementAt(nodeSerialNumber - 1);
				if (nodeModel.ManagingSocket == null)
				{
					Console.Write($"Creating connection to tcp socket: 127.0.0.1:{nodeModel.ManagingPort}");
					try
					{
						nodeModel.ManagingSocket = new PairSocket($">tcp://127.0.0.1:{nodeModel.ManagingPort}");
					}
					catch (Exception e)
					{
						Console.WriteLine(" - Fail");
						Console.WriteLine(e.Message);
						continue;
					}
					Console.WriteLine(" - Created");
				}

				Console.Write("Handshake");
				if (!DoesHandshakeSucceeded(nodeModel))
				{
					Console.WriteLine(" - Fail");
					continue;
				}

				Console.WriteLine(" - Said Hello");

				CommandParser commandParser  = new CommandParser();
				commandParser.Init();

				CommunicateWithNode(nodeModel, commandParser);
			}
		}

		private static void ReadConfig()
		{
			JObject config = JObject.Parse(System.IO.File.ReadAllText(ConfigFileName));
			foreach (var (name, portsToken) in config)
			{
				string managingPort = portsToken.SelectToken("ManagerPort").Value<string>();
				string nodePort = portsToken.SelectToken("NodeConnectionPort ").Value<string>();
				Nodes.Add(new NodeClass(name, managingPort, nodePort));
			}
		}

		private static void WriteNodes()
		{
			Console.WriteLine("Nodes:");
			for (var i = 0; i < Nodes.Count; ++i)
			{
				NodeClass nodeModel = Nodes.ElementAt(i);
				Console.WriteLine($"{(i + 1).ToString()}. {nodeModel.Name}({nodeModel.ManagingPort})");
			}
		}

		private static bool DoesHandshakeSucceeded(NodeClass nodeModel)
		{
			return nodeModel.ManagingSocket.TrySendFrame(TimeSpan.FromSeconds(5), Command.CHECK) && nodeModel.ManagingSocket.TryReceiveFrameString(TimeSpan.FromSeconds(3), out _);
		}
		
		private static void CommunicateWithNode(NodeClass nodeModel, CommandParser commandParser)
		{
			string commandString = "";
			while (commandString != Command.KILL)
			{
				Console.Write("#  ");
				commandString = Console.ReadLine();
				if (!commandParser.IsValidString(commandString))
				{
					commandParser.WriteHelpForCommand(commandString);
					continue;
				}

				nodeModel.ManagingSocket.SendFrame(commandString);
				Console.WriteLine(nodeModel.ManagingSocket.ReceiveFrameString());
			}
		}
    }
}