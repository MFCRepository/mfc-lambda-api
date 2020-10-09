using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyHttpGatewayApi.Controllers.Misc
{
    [Route("")]
    [ApiController]
    public class healthController : ControllerBase
    {
        // GET: api/<healthController>
        [HttpGet]
        public Output Get()
        {
            return new Output() { Health = "ok", Environment = (GetSubDomain(HttpContext) == "api" ? "production" : GetSubDomain(HttpContext)) };
        }

        // GET api/<urlController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "";
        }

        // POST api/<urlController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<urlController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<urlController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public string GetSubDomain(HttpContext httpContext)
        {
            var subDomain = string.Empty;

            var host = httpContext.Request.Host.Host;

            if (!string.IsNullOrWhiteSpace(host))
            {
                subDomain = host.Split('.')[0];
            }

            return subDomain.Trim().ToLower();
        }
    }

    public  class Output
    {
        public string Health { get; set; }
        public string Environment { get; set; }
    }

}
