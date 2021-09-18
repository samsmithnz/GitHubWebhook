using GitHubWebhook.Service.Models;
//using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
//using fluent = Microsoft.Azure.Management.Fluent;
//using queue = Azure.Storage.Queues;

namespace WebhookReceiver.Service.Repos
{
    public class Repo : IRepo
    {
        public async Task<PullRequest> ProcessPullRequest(JObject payload,
                string clientId, string clientSecret,
                string tenantId, string subscriptionId, string resourceGroupName,
                string keyVaultQueueName, string keyVaultSecretsQueueName, string storageConnectionString)
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
            else if (string.IsNullOrEmpty(keyVaultQueueName) == true)
            {
                throw new Exception("Misconfiguration: storage policies queue name is null");
            }
            else if (string.IsNullOrEmpty(keyVaultSecretsQueueName) == true)
            {
                throw new Exception("Misconfiguration: storage secrets queue name is null");
            }
            else if (string.IsNullOrEmpty(storageConnectionString) == true)
            {
                throw new Exception("Misconfiguration: storage connection string is null");
            }

            //Get pull request details
            PullRequest pr = new PullRequest
            {
                Number = payload["number"]?.ToString(),
                Action = payload["action"]?.ToString()

            };
            resourceGroupName = resourceGroupName.Replace("__###__", "PR" + pr.Number.ToString());

            //If the PR is completed or abandoned, clean up the secrets and permissions from key vault and then delete the resource group/resources
            if (pr != null && (pr.Action == "closed"))
            {
                //Clean up the key vault
                AzureCredentials creds = new AzureCredentialsFactory().FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud);
                Microsoft.Azure.Management.Fluent.IAzure azure = Microsoft.Azure.Management.Fluent.Azure.Authenticate(creds).WithSubscription(subscriptionId);

                RestClient _restClient = RestClient
                   .Configure()
                   .WithEnvironment(AzureEnvironment.AzureGlobalCloud)
                   .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                   .WithCredentials(creds)
                   .Build();

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
                string tenantId, string subscriptionId, string resourceGroupName,
                string keyVaultQueueName, string keyVaultSecretsQueueName, string storageConnectionString);
    }

}
