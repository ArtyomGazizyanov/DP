using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Models;
using System.Linq;
using NetMQ;
using NetMQ.Sockets;

namespace Client
{
    class Program
    {
        private const string Config = "../NodeConfig.json";

		private static readonly List<NodeClass> Nodes = new List<NodeClass>();
		private static IDictionary<string, ISet<int>> Services = new Dictionary<string, ISet<int>>();

        static void Main(string[] args)
        {
            InitWithConfig();
            
            NodeClass node;
            int currentNode = Nodes.Count;
            do
            {
                currentNode--;
                node = Nodes.ElementAt(0);
                InitManagingSocket(ref node);

                Console.Write("Try to handshake");
                
            } while(!HandshakeSucceeded(node) && currentNode >= 0);
			
            if(currentNode < 0)
            {
                Console.WriteLine("[X] Error: Can`t connect to nodes");
                return;
            }

            Console.WriteLine(" [OK]");
			
            GetArguements(node);
			GetAllServices(node);			
        }

        private static void GetArguements(NodeClass node)
        {
            while (true)
			{
				Console.WriteLine("Enter a <http port> <1st Number> <2nd Number> Or to get all services write `GETALL` command");
				string commandString = Console.ReadLine();

				if (commandString == Command.EXIT)
				{
					break;
				}

                if (commandString == Command.GETALL)
				{
                    GetAllServices(node);
					continue;
				}

				string[] command = commandString.Split(" ");

                if(command.Length != 3)
                {
                    continue;
                }

				string communicatePort = GetPortOfService(node, command[0]);
				Console.WriteLine($"#{communicatePort}#");
				if(String.IsNullOrEmpty(communicatePort))
				{
					Console.WriteLine($"Not found communicate port for {command[0]}: ({communicatePort})");
					continue;
				}

                MathModel mathModel = new MathModel
                {
                    Value1 = Int32.Parse(command[1]),
                    Value2 = Int32.Parse(command[2])
                };

                string serializedMathModel = JsonConvert.SerializeObject(mathModel);

				string url = $"http://127.0.0.1:{communicatePort}/api/values";
				StringContent stringContent = CreateStringContent(serializedMathModel);

				using (HttpClient httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

					using (HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result)
					using (HttpContent content = response.Content)
					{
						Console.WriteLine(content.ReadAsStringAsync().Result);
					}
				}
			}
        }        
		private static string GetPortOfService(NodeClass node, string serviceName)
		{
            node.ManagingSocket.SendFrame($"{Command.GET} {serviceName}");
			string[] ports = node.ManagingSocket.ReceiveFrameString().Split(", ");
			
			if(ports == null && ports?.Length == 0)
			{
				return String.Empty;
			}

			return  ports.FirstOrDefault();
		} 

        private static void GetAllServices(NodeClass node)
        {
            node.ManagingSocket.SendFrame(Command.GETALL);
			Services = JsonConvert.DeserializeObject<IDictionary<string, ISet<int>>>(node.ManagingSocket.ReceiveFrameString());

			Console.WriteLine("Services: ");

			foreach (KeyValuePair<string, ISet<int>> service in Services)
			{
				string serviceName = service.Key;
				ISet<int> servicePorts = service.Value;
				Console.WriteLine($"{serviceName}: {string.Join(", ", servicePorts)}");
			}
        }

        private static void InitManagingSocket(ref NodeClass node)
        {
            if (node.ManagingSocket == null)
			{
				Console.Write($"Creating connection to tcp socket: 127.0.0.1:{node.ManagingPort}");
				try
				{
					node.ManagingSocket = new PairSocket($">tcp://127.0.0.1:{node.ManagingPort}");
				}
				catch (Exception e)
				{
					Console.WriteLine(" - Fail");
					Console.WriteLine(e.Message);
					return;
				}

				Console.WriteLine(" - Created");
			}
        }

        private static void InitWithConfig()
		{
			JObject config = JObject.Parse(System.IO.File.ReadAllText(Config));

			foreach (var (name, portsToken) in config)
			{
				string managingPort = portsToken.SelectToken("ManagerPort").Value<string>();
				string nodePort = portsToken.SelectToken("NodeConnectionPort ").Value<string>();
				Nodes.Add(new NodeClass(name, managingPort, nodePort));
			}
		}

        private static bool HandshakeSucceeded(NodeClass node)
		{
			return node.ManagingSocket.TrySendFrame(TimeSpan.FromSeconds(3), Command.CHECK)
					&& node.ManagingSocket.TryReceiveFrameString(TimeSpan.FromSeconds(3), out _);
		}
		
		private static StringContent CreateStringContent(string serializedMathModel)
		{
			return new StringContent(serializedMathModel, Encoding.UTF8, "application/json");
		} 
    }
}
