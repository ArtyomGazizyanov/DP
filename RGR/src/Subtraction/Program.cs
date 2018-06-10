using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Subtraction
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
			{
				Console.WriteLine("Error: port isn't specified");
				return;
			}
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder()
                    .UseStartup<Startup>()
                    .UseUrls($"http://localhost:{args.ElementAt(0)}")
                    .Build(); 
    }
}
