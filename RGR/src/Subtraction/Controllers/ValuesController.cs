using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Subtraction.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Check()
        {
            return new string[] { "Ok" };
        }

        [HttpPost]
        public string Subtraction([FromBody] Dto.MathModel data)
        {
             return $"{data.Value1 - data.Value2}";      
        }       
    }
}
