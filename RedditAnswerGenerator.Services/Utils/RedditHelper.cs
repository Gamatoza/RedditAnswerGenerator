using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string GetSubredditName(string match)
        {
            string Name;
            Regex regex = new Regex(@"\/?r\/.\w*\/?");
            if (regex.IsMatch(match))
            {
                Name = regex.Match(match).Value.Replace("/r/", "");
                if (string.Join("", Name.Take(2)).Equals("r/"))
                    Name = string.Join("", Name.Skip(2));
                Name = Name.Replace("//", "/").Replace("/", "");
                Name = Name.ToLower();
            }
            else
            {
                Name = match;
            }
            return Name;
        }
    }
}
