using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyHttpGatewayApi.Controllers
{
    [Route("api/url")]
    [ApiController]
    public class urlController : ControllerBase
    {
        // GET: api/<urlController>
        [HttpGet]
        public string Get()
        {
            string html = string.Empty;
            string url = @"https://bot.whatismyipaddress.com";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }

        // GET api/<urlController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
    }
}
