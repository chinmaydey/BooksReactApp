using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net.Http.Headers;

namespace NETCORE.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class TestController : ControllerBase
    {      
        private readonly ILogger<WeatherForecastController> _logger;

        public TestController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //[HttpGet]
        private async Task<string> GetToken()
        {
             var auth0Client = new HttpClient();
                string token = "";
                var bodyString = $@"{{""client_id"":""7oltoQo9D9MLwG51mkUFwDrApqotjNFz"", ""client_secret"":""PFQU7jntwOcG4yqeafPy8Y75x1EcMti85wrWEtXWnIjpadZQ4VATM3ls2ZxkLC9e"", ""audience"":""http://localhost:5000"", ""grant_type"":""client_credentials""}}";
                var response = await auth0Client.PostAsync("https://dev-y2mwzdf7.au.auth0.com/oauth/token", new StringContent(bodyString, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseJson = JObject.Parse(responseString);
                    token = (string)responseJson["access_token"];

                }

                return token;
        }

        [HttpGet]
        [Route("GetData")]
        public async Task<string> GetData()
        {
            var token = await GetToken();
            var client = new HttpClient();

            Console.Write(token);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/WeatherForecast");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var booksResponse = await client.SendAsync(requestMessage);

            var bookResponseString = await booksResponse.Content.ReadAsStringAsync();
            var bookResponseJson = JArray.Parse(bookResponseString);

            Console.Write(bookResponseJson);
            
            return bookResponseJson.ToString();
        }
    }
}
