using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubWebhook.Service.Models
{
    public class PullRequest
    {
        public string Number { get; set; }
        public string Action { get; set; }

    }
}
