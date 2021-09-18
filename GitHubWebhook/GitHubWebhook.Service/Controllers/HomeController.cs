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
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IRepo _repo;

        public HomeController(IConfiguration configuration, IRepo repo)
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
            string tenantId = _configuration["WebhookTenantId"];
            string subscriptionId = _configuration["WebhookSubscriptionId"];
            string resourceGroupName = _configuration["AppSettings:WebhookResourceGroup"];

            //Add identities to queue, if they don't exist.
            PullRequest result = await _repo.ProcessPullRequest(payload,
                clientId, clientSecret, tenantId, subscriptionId, resourceGroupName);

            return (result != null) ? new OkResult() : new StatusCodeResult(500);
        }

    }
}
