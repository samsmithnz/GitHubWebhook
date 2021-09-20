using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace GitHubWebhook.Tests
{
    public static class Common
    {
        public static JObject ReadJSON(string fileName)
        {
            JObject payload;
            // read JSON directly from a file
            using (StreamReader file = System.IO.File.OpenText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + fileName))
            {
                using (JsonTextReader reader = new(file))
                {
                    payload = (JObject)JToken.ReadFrom(reader);
                }
            }

            return payload;
        }
    }
}
