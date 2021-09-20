using GitHubWebhook.Service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using WebhookReceiver.Service.Repos;

namespace GitHubWebhook.Tests
{
    [TestClass]
    public class WebhookControllerTests
    {
        private string ClientId;
        private string ClientSecret;
        private string TenantId;
        private string SubscriptionId;
        private string ResourceGroupName;

        [TestInitialize]
        public void InitializeTests()
        {
            //Key vault access
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .AddUserSecrets<WebhookControllerTests>();
            IConfigurationRoot Configuration = configBuilder.Build();

            //Setup the repo
            ClientId = Configuration["AppSettings:ClientId"];
            ClientSecret = Configuration["AppSettings:ClientSecret"];
            TenantId = Configuration["AppSettings:TenantId"];
            SubscriptionId = Configuration["AppSettings:SubscriptionId"];
            ResourceGroupName = Configuration["AppSettings:ResourceGroup"];
        }

        [TestMethod]
        public async Task ReadingSamplePayloadTest()
        {
            //Arrange
            JObject payload = Common.ReadJSON(@"/Samples/ClosedPR.json");
            Repo code = new();

            //Act
            PullRequest pr = await code.ProcessPullRequest(payload,
                ClientId, ClientSecret,
                TenantId, SubscriptionId, ResourceGroupName);

            //Assert
            Assert.IsTrue(pr != null);
            Assert.AreEqual("2", pr.Number);
            Assert.AreEqual("closed", pr.Action);
        }

        [TestMethod]
        public async Task ReadingInvalidPRSamplePayloadTest()
        {
            //Arrange
            JObject payload = Common.ReadJSON(@"/Samples/RepoPushSample.json");
            Repo code = new();

            //Act
            PullRequest pr = await code.ProcessPullRequest(payload,
                ClientId, ClientSecret,
                TenantId, SubscriptionId, ResourceGroupName);

            //Assert
            Assert.IsTrue(pr != null);
            Assert.AreEqual(null, pr.Number);
            Assert.AreEqual(null, pr.Action);
        }

        [TestMethod]
        public async Task ReadingEditingPRSamplePayloadTest()
        {
            //Arrange
            JObject payload = Common.ReadJSON(@"/Samples/EditedPR.json");
            Repo code = new();

            //Act
            PullRequest pr = await code.ProcessPullRequest(payload,
                ClientId, ClientSecret,
                TenantId, SubscriptionId, ResourceGroupName);

            //Assert
            Assert.IsTrue(pr != null);
            Assert.AreEqual("4", pr.Number);
            Assert.AreEqual("edited", pr.Action);
        }

        //[TestMethod]
        //public async Task ProcessingSamplePayloadTest()
        //{
        //    //Arrange
        //    JObject payload = Common.ReadJSON(@"/Samples/ClosedPR.json");
        //    Repo code = new();

        //    //Act


        //    PullRequest pr = await code.ProcessPullRequest(payload,
        //        ClientId, ClientSecret,
        //        TenantId, SubscriptionId, ResourceGroupName);

        //    //Assert
        //    Assert.IsTrue(pr != null);
        //    Assert.AreEqual("2", pr.Number);
        //    Assert.AreEqual("closed", pr.Action);
        //}
    }
}
