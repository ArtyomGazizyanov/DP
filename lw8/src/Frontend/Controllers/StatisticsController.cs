using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Frontend.Dto;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System;

namespace Frontend.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IConfiguration _configuration;

        public StatisticsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Statistics(string id)
        {
            HttpClient client = new HttpClient();
            string backendUrl = _configuration["BackendUrl"];
            string getRankApi = backendUrl + $"api/values/statistics";

            var response = client.GetAsync(getRankApi);
            Console.WriteLine($" [x] Responce: {response}");    
            var contents = response.Result.Content.ReadAsStringAsync();
            StatisticsDto staticsDto = JsonConvert.DeserializeObject<StatisticsDto>(contents.Result);
            TempData["avgRank"] = staticsDto.AvgRank;
            TempData["highRankPart"] = staticsDto.HighRankPart;
            TempData["textNum"] = staticsDto.TextNum;
            Thread.Sleep(1000);
            return View();
        }

        

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
