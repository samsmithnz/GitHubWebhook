using GitHubWebhook.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using WebhookReceiver.Service.Repos;

namespace GitHubWebhook.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IRepo _repo;

        public WebhookController(IConfiguration configuration, IRepo repo)
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
            string tenantId = _configuration["AppSettings:TenantId"];
            string subscriptionId = _configuration["AppSettings:SubscriptionId"];
            string resourceGroupName = _configuration["AppSettings:ResourceGroup"];

            //Add identities to queue, if they don't exist.
            PullRequest result = await _repo.ProcessPullRequest(payload,
                clientId, clientSecret, tenantId, subscriptionId, resourceGroupName);

            return (result != null) ? new OkResult() : new StatusCodeResult(500);
        }

        [HttpGet]
        public string Get()
        {
            return "Hello world";
        }

    }
}
