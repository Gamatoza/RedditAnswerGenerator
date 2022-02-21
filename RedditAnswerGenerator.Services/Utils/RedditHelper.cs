using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditAnswerGenerator.Services.Utils
{
    public static class RedditHelper
    {
        public static bool CheckSubredditExists(string subRedditName)
        {
            var baseUrl = $"https://old.reddit.com/r/{subRedditName}/about.json";
            var client = new RestClient();
            var request = new RestRequest(baseUrl, Method.Get);
            var response = client.DownloadDataAsync(request).Result;

            if (response == null)
            {
                return false;
            }

            Stream stream = new MemoryStream(response);

            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonStream(stream)
               .Build();

            if (configuration.GetValue<int>("error") == 404)
            {
                return false;
            }

            return true;
        }
    }
}
