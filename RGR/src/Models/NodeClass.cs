using System;
using NetMQ;
using NetMQ.Sockets;

namespace Models
{
    public class NodeClass
    {
		public NodeClass(string name, string managingPort, string port)
		{
			Name = name;
			ManagingPort = managingPort;
			NodeConnectionPort  = port;
		}
		
        public NodeClass()
		{
		}		

		public string Name { get; set; }
		public string ManagingPort { get; set; }
		public string NodeConnectionPort  { get; set; }
		public NetMQSocket ManagingSocket { get; set; }
		public NetMQSocket NodeSocket { get; set; }
    }
}
