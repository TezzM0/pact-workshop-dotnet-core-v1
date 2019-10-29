using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using provider;

namespace Provider.Controllers
{
    [Route("api/[controller]")]
    public class ProviderController : Controller
    {
        private readonly Data _data;
        private IConfiguration _Configuration { get; }

        public ProviderController(IConfiguration configuration)
        {
            this._Configuration = configuration;
            _data = new Data();
        }

        // GET api/provider?validDateTime=[DateTime String]
        [HttpGet]
        public IActionResult Get(string validDateTime)
        {
            if(String.IsNullOrEmpty(validDateTime))
            {
                return BadRequest(new { message = "validDateTime is required" });
            }

            if(_data.DataIsMissing())
            {
                return NotFound();
            }

            DateTime parsedDateTime;

            try
            {
                parsedDateTime = DateTime.Parse(validDateTime);
            }
            catch(Exception)
            {
                return BadRequest(new { message = "validDateTime is not a date or time" });
            }

            return new JsonResult(new {
                test = "NO",
                validDateTime = parsedDateTime.ToString("dd-MM-yyyy HH:mm:ss")
            });
        }
    }
}
