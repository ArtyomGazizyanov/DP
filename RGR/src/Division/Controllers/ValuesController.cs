using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Division.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }     

        [HttpPost]
        public string Divide([FromBody] Dto.MathModel data)
        {
            if(data.Value2 == 0)
            {
                return "Error <value2> can`t equals to 0";
            }

            return $"{data.Value1 / data.Value2}";         
        }      
    }
}
