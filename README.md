# GitHubWebhook
An ASP.NET (.NET 5) webhook to process GitHub pull request events. The goal is to capture close events from Pull Requests, and delete corresponding resources in Azure related to the Pull Request.

## Setup

1. Create a service principal in Azure. Note that this should be scoped to subscription:
```
az ad sp create-for-rbac --name "MyGitHubWebHookServicePrincipal" --role contributor --scopes /subscriptions/[your subscription id] --sdk-auth
```

## References

- [GitHub web hook docs](https://docs.github.com/en/developers/webhooks-and-events/webhooks/webhook-events-and-payloads#pull_request)


## Azure DevOps Note:
There is an [Azure DevOps version](https://github.com/samsmithnz/AzureDevOpsWebhook) too! 
