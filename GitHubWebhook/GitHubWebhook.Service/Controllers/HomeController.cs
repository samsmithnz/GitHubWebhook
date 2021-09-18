using GitHubWebhook.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookReceiver.Service.Repos;

namespace GitHubWebhook.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IRepo _repo;

        public HomeController( IConfiguration configuration, IRepo repo)
        {
            _configuration = configuration;
            _repo = repo;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post(JObject payload)
        {
            string clientId = _configuration["AppSettings:ClientId"];
            string clientSecret = _configuration["AppSettings:ClientSecret"];
            //string clientId = _configuration["WebhookClientId"];
            //string clientSecret = _configuration["WebhookClientSecret"];
            string tenantId = _configuration["WebhookTenantId"];
            string subscriptionId = _configuration["WebhookSubscriptionId"];
            string resourceGroupName = _configuration["AppSettings:WebhookResourceGroup"];
            string keyVaultQueueName = _configuration["AppSettings:KeyVaultQueue"];
            string keyVaultSecretsQueueName = _configuration["AppSettings:KeyVaultSecretsQueue"];
            string storageConnectionString = _configuration["AppSettings:StorageConnectionString"];

            //Add identities to queue, if they don't exist.
            PullRequest result = await _repo.ProcessPullRequest(payload,
                clientId, clientSecret, tenantId, subscriptionId, resourceGroupName,
                keyVaultQueueName, keyVaultSecretsQueueName, storageConnectionString);

            return (result != null) ? new OkResult() : new StatusCodeResult(500);
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
    }
}
