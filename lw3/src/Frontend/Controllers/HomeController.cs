using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Frontend.Dto;
using Microsoft.Extensions.Configuration;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
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

        [HttpPost]
        public IActionResult Upload(string data)
        {
            string id = null;
            HttpClient client = new HttpClient();

            DataTransferDto dataTransfer = new DataTransferDto {
                Data = data
            };
            
            string  backendUrl = _configuration["BackendUrl"];
            string  uploadApi = backendUrl + "api/values";
            var response = client.PostAsync(
                uploadApi,
                 new StringContent(JsonConvert.SerializeObject(dataTransfer), Encoding.UTF8, "application/json"));

            var contents =  response.Result.Content.ReadAsStringAsync();
            id = contents.Result;

            return Ok(id);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
