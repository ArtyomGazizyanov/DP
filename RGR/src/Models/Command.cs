using System;
using NetMQ;
using NetMQ.Sockets;

namespace Models
{
    public static class Command
	{
		public const string GET = "GET";
		public const string GETALL = "GETALL";
		public const string START = "START";
		public const string STOP = "STOP";
		public const string KILL = "KILL";
		public const string EXIT = "EXIT";
		public static string CHECK = "CHECK";
	}
}