# GitHubWebhook
An ASP.NET (.NET 5) webhook to process GitHub pull request events. The goal is to capture close events from Pull Requests, and delete corresponding resources in Azure related to the Pull Request.

## Setup

1. Create a service principal in Azure, running this command (with your sub id) in the Azure Portal shell. Note that this should be scoped to subscription:
```
az ad sp create-for-rbac --name "MyGitHubWebHookServicePrincipal" --role contributor --scopes /subscriptions/[your subscription id] --sdk-auth
```
2. the result should look like this. Make a note of the client id and client secret.
![image](https://user-images.githubusercontent.com/8389039/133949278-26c2c430-ac2e-47e1-b481-73b32011ed84.png)
3. Add the service principal to the contributor role on the subscription. This will give the service principal permission to delete resource groups
![image](https://user-images.githubusercontent.com/8389039/133949340-8326ddb5-c95e-4f81-b7df-28a7587fa462.png)


## References

- [GitHub web hook docs](https://docs.github.com/en/developers/webhooks-and-events/webhooks/webhook-events-and-payloads#pull_request)


## Azure DevOps Note:
There is an [Azure DevOps version](https://github.com/samsmithnz/AzureDevOpsWebhook) too! 
