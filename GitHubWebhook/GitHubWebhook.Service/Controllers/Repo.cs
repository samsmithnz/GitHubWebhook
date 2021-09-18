using GitHubWebhook.Service.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace WebhookReceiver.Service.Repos
{
    public class Repo : IRepo
    {
        public async Task<PullRequest> ProcessPullRequest(JObject payload,
                string clientId, string clientSecret,
                string tenantId, string subscriptionId, string resourceGroupName)
        {
            //Validate the payload
            if (payload["resource"] == null)
            {
                throw new Exception("Payload resource is null");
            }
            else if (payload["resource"]["status"] == null)
            {
                throw new Exception("Payload resource status is null");
            }
            else if (payload["resource"]["title"] == null)
            {
                throw new Exception("Payload title is null");
            }
            else if (payload["resource"]["pullRequestId"] == null)
            {
                throw new Exception("Payload pullRequestId is null");
            }
            else if (string.IsNullOrEmpty(clientId) == true)
            {
                throw new Exception("Misconfiguration: client id is null");
            }
            else if (string.IsNullOrEmpty(clientSecret) == true)
            {
                throw new Exception("Misconfiguration: client secret is null");
            }
            else if (string.IsNullOrEmpty(tenantId) == true)
            {
                throw new Exception("Misconfiguration: tenant id is null");
            }
            else if (string.IsNullOrEmpty(subscriptionId) == true)
            {
                throw new Exception("Misconfiguration: subscription id is null");
            }
            else if (string.IsNullOrEmpty(resourceGroupName) == true)
            {
                throw new Exception("Misconfiguration: resource group is null");
            }

            //Get pull request details
            PullRequest pr = new()
            {
                Number = payload["number"]?.ToString(),
                Action = payload["action"]?.ToString()

            };
            resourceGroupName = resourceGroupName.Replace("__###__", "PR" + pr.Number.ToString());

            //If the PR is completed or abandoned, clean up the secrets and permissions from key vault and then delete the resource group/resources
            if (pr != null && (pr.Action == "closed"))
            {
                //Authenicate and connect to Azure subscription
                AzureCredentials creds = new AzureCredentialsFactory().FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud);
                Microsoft.Azure.Management.Fluent.IAzure azure = Microsoft.Azure.Management.Fluent.Azure.Authenticate(creds).WithSubscription(subscriptionId);

                //Delete the resource group
                await azure.ResourceGroups.DeleteByNameAsync(resourceGroupName);
            }

            return pr;
        }
    }

    public interface IRepo
    {
        Task<PullRequest> ProcessPullRequest(JObject payload,
                string clientId, string clientSecret,
                string tenantId, string subscriptionId, string resourceGroupName);
    }

}
