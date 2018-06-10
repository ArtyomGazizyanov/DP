using System;
using Models;
using System.Linq;
using NetMQ;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Node
{
    class Program
    {
        private const string ConfigFileName = "../NodeConfig.json";
        private const string Ok = "OK";
		private const string Bad = "Bad";
		private static readonly ISet<string> serviceMap = new HashSet<string> {"Division", "Subtraction"};
        private static NodeClass Instance = new NodeClass();
        private static readonly IDictionary<string, ISet<KeyValuePair<int, Process>>> Services = new Dictionary<string, ISet<KeyValuePair<int, Process>>>();
        private static NodeNetwork NodeNetwork;

        static void Main(string[] args)
        {
            if (!GetNodeName(args))
			{
				System.Console.WriteLine(" [X] Node name not specified");  
				return;				
			}              
                NodeNetwork = new NodeNetwork();
                ReadConfig();
				Console.WriteLine($"ManagingPort = |{Instance.ManagingPort}| NodeConnectionPort  = |{Instance.NodeConnectionPort }|");
                NodeNetwork.Start(Instance);
                Task.Factory.StartNew(state => ServerActivity(), string.Format($"Server {Instance.Name}"), TaskCreationOptions.LongRunning);
                Task.Factory.StartNew(state => NodeActivity(), string.Format($"Node {Instance.Name}"), TaskCreationOptions.LongRunning);

			    System.Console.ReadKey();
				return;
        }

        private static bool GetNodeName(string[] args)
        {
            if (args.Length < 1)
            {
                return false;
            }
            Instance.Name = args.ElementAt(0);
            return true;
        }

        private static void ReadConfig()
		{
			JObject config = JObject.Parse(System.IO.File.ReadAllText(ConfigFileName));
			foreach (var (name, portsToken) in config)
			{
				string managingPort = portsToken.SelectToken("ManagerPort").Value<string>();
				string nodePort = portsToken.SelectToken("NodeConnectionPort ").Value<string>();
				
				if (name == Instance.Name)
				{
					Instance.ManagingPort = managingPort;
					Instance.NodeConnectionPort  = nodePort;
					
					Console.WriteLine($"{Instance.Name} == {name}, {managingPort}, {nodePort}");
				}
				else
				{
					NodeNetwork.Add(new NodeClass(name, managingPort, nodePort));
					Console.WriteLine($"{name}, {managingPort}, {nodePort}");
				}
			}
		}
        
        private static IEnumerable<int> GetPorts(ISet<KeyValuePair<int, Process>> portsWithProcesses)
		{
			return portsWithProcesses.Select(portWithProcess => portWithProcess.Key);
		}

		private static void ServerActivity()
		{
			while (true)
			{
				string message = Instance.ManagingSocket.ReceiveFrameString();
				Console.WriteLine($"Received message from Manager: {message}");
				string[] command = message.Split(' ');
				switch (command[0])
				{
					case "CHECK":
						Instance.ManagingSocket.SendFrameEmpty();
						break;
					case "KILL":
						Instance.ManagingSocket.SendFrameEmpty();
						break;
					case "GET":
						Services.TryGetValue(command[1], out var servicePortsWithProcesses);
						Instance.ManagingSocket.SendFrame(servicePortsWithProcesses == null ? "||" : string.Join(", ", GetPorts(servicePortsWithProcesses)));
						break;
					case "GETALL":
						Instance.ManagingSocket.SendFrame(ServicesToJson(Services));
						break;
					case "START":
						if (!serviceMap.Contains(command[1]))
						{
							Instance.ManagingSocket.SendFrame(Bad);
							break;
						}

						NodeNetwork.SendMessageToAllNodes(message);
						Services.TryAdd(command[1], new HashSet<KeyValuePair<int, Process>>());
						if (int.TryParse(command[2], out var port) && Services[command[1]].Add(new KeyValuePair<int, Process>(port, StartService(command[1], command[2]))))
						{
							Instance.ManagingSocket.SendFrame(Ok);
						}
						else
						{
							Instance.ManagingSocket.SendFrame(Bad);
						}
						break;
					case "STOP":
						NodeNetwork.SendMessageToAllNodes(message);
						if (!Services.ContainsKey(command[1]))
						{
                            break;
                        }
                        var portToRemove = int.Parse(command[2]);
                        foreach (var portToProcess in Services[command[1]])
                        {
                            if (portToProcess.Key == portToRemove)
                            {
                                portToProcess.Value?.Kill();
                                Instance.ManagingSocket.SendFrame(Services[command[1]].Remove(portToProcess) ? Ok : Bad);
                                break;
                            }
                        }
                        Instance.ManagingSocket.SendFrame(Bad);		
						break;
				}
			}
		}

		private static string ServicesToJson(IDictionary<string, ISet<KeyValuePair<int, Process>>> dictionary)
		{
			var entries = dictionary.Select(d => $"\"{d.Key}\": [{string.Join(", ", d.Value.Select(s => s.Key))}]");
			return $"{{{string.Join(",", entries)}}}";
		}
		private static void NodeActivity()
		{
			while (true)
			{
				string message = Instance.NodeSocket.ReceiveMultipartStrings().ElementAt(1);
				Console.WriteLine($"Received message from Node: {message}");
				string[] command = message.Split(' ');
				switch (command[0])
				{
					case "START":
						Services.TryAdd(command[1], new HashSet<KeyValuePair<int, Process>>());
						int.TryParse(command[2], out var port);
						Services[command[1]].Add(new KeyValuePair<int, Process>(port, null));
						break;
					case "STOP":
						if (Services.ContainsKey(command[1]))
						{
							var portToRemove = int.Parse(command[2]);
							foreach (var portToProcess in Services[command[1]])
							{
								if (portToProcess.Key == portToRemove)
								{
									portToProcess.Value?.Kill();
									Services[command[1]].Remove(portToProcess);
									break;
								}
							}
						}
						break;
				}
			}
		}

		private static Process StartService(string name, string port)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "dotnet",
					Arguments = $"../{name}/{name}.dll {port}",
					UseShellExecute = true,
					RedirectStandardOutput = false,
					RedirectStandardError = false,
					CreateNoWindow = true
				}
			};

			process.Start();
			return process;
		}
    }    
}
