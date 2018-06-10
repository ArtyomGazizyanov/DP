using NetMQ;
using System.Collections;
using System.Collections.Generic;
using Models;
using System;
using NetMQ.Sockets;

namespace Node
{
	public class NodeNetwork : IEnumerable<KeyValuePair<string, NodeClass>>
	{
		private readonly IDictionary<string, NodeClass> Nodes = new Dictionary<string, NodeClass>();

		public NodeNetwork()
		{
		}

		public IEnumerator<KeyValuePair<string, NodeClass>> GetEnumerator()
		{
			return Nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(NodeClass NodeClass)
		{
			Nodes.Add(NodeClass.Name, NodeClass);
		}

		public void SendMessageToAllNodes(string message)
		{
			foreach (var (_, NodeClass) in Nodes)
			{
				NodeClass.NodeSocket.SendFrame(message);
			}
		}

		public void Start(NodeClass instance)
		{
			Initialization(instance);
			Console.WriteLine("Creating connection between nodes...");
			foreach (var (_, NodeClass) in Nodes)
			{
				string logMessage = $"tcp socket: 127.0.0.1:{NodeClass.NodeConnectionPort }";
				try
				{
					NodeClass.NodeSocket = new PushSocket($">tcp://127.0.0.1:{NodeClass.NodeConnectionPort }");
				}
				catch (Exception)
				{
					logMessage += " - Fail";
					Console.WriteLine(logMessage);
					continue;
				}

				logMessage += " - Created";
				Console.WriteLine(logMessage);
			}
		}

		private void Initialization(NodeClass instance)
		{
			string logMessage = $"Managin port binding to tcp: 127.0.0.1:{instance.ManagingPort}";
			try
			{
				instance.ManagingSocket = new PairSocket($"@tcp://127.0.0.1:{instance.ManagingPort}");
			}
			catch (Exception)
			{
				logMessage += " - Fail";
                Console.WriteLine(logMessage);				
                return;
			}
			logMessage += " - Binded";
			Console.WriteLine(logMessage);
			logMessage = $"Node port binding to tcp: 127.0.0.1:{instance.NodeConnectionPort }";
			try
			{
				instance.NodeSocket = new RouterSocket($"@tcp://127.0.0.1:{instance.NodeConnectionPort }");
			}
			catch (Exception)
			{
				logMessage += " - Fail";
				Console.WriteLine(logMessage);
				return;
			}
			logMessage += " - Binded";
			Console.WriteLine(logMessage);
		}

        IEnumerator<KeyValuePair<string, NodeClass>> IEnumerable<KeyValuePair<string, NodeClass>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}