


using RedditAnswerGenerator.Services.Extensions;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace RedditAnswerGenerator.Services.Utils
{
    public class PushShiftRedditResult
    {
        public string selftext { get; set; }
        public string full_link { get; set; }
        public string score { get; set; }
        public string url { get; set; }
        public string post_hint { get; set; }
        public int num_comments { get; set; }
        public bool no_follow { get; set; }
        public bool hasVideo
        {
            get
            {
                if (post_hint != null)
                    return post_hint.Contains("video");
                return false;
            }
        }
        public bool isLink
        {
            get
            {
                if (post_hint != null)
                    return post_hint.Contains("link");
                return false;
            }
        }
    }

    public class PushShiftCommentResult
    {
        public string id { get; set; }
        public string body { get; set; }
        public string author { get; set; }
        public string permalink { get; set; }
        public string score { get; set; }
    }

    public class PushShiftSearch
    {
        #region Private SearchSettings
        private string _before { get; set; }
        private string _after { get; set; }
        private string _score { get; set; }
        private string _size { get; set; }
        private string _author { get; set; }
        private string _selftext { get; set; }
        private string _avoidDeleted { get; set; }
        private string _avoidVideosStr { get; set; }
        private string _avoidNTFS { get; set; }
        private string _title { get; set; }
        private bool _avoidURLsInText { get; set; }
        private bool _avoidVideos { get; set; }
        #endregion

        private string mainUrlPart = @"https://api.pushshift.io/reddit/";
        private string subRedditSearch => mainUrlPart + "search/submission/?subreddit=" + subRedditNamePart + getSearchParts();
        private string commentSearch => mainUrlPart + "comment/search/?subreddit=" + subRedditNamePart + getSearchParts();
        private string subRedditNamePart;
        public PushShiftSearch(string reddit)
        {
            subRedditNamePart = reddit;
        }

        private List<T> GetJsonResult<T>(string fullUrl)
        {
            //Log.Info($"pushshift-url: {fullUrl}");
            var client = new RestClient();
            var request = new RestRequest(fullUrl, Method.Get);
            try
            {
                var response = client.DownloadDataAsync(request).Result ?? throw new Exception("Response was null!");
                Stream stream = new MemoryStream(response);

                IConfiguration configuration = new ConfigurationBuilder()
                   .AddJsonStream(stream)
                   .Build();

                if (configuration.GetSection("error").Get<int>() == 404)
                {
                    throw new ArgumentNullException();
                }

                return configuration.GetSection("data").Get<List<T>>();
            }
            catch (ArgumentNullException ex)
            {
                //Log.Error($"Reddit not found \n{ex.Message}=>{ex.StackTrace}");
            }
            catch (Exception ex)
            {
                //Log.Error($"{ex.Message}=>{ex.StackTrace}");
            }
            
            return new List<T>();

        }
        public List<PushShiftRedditResult> GetSubredditInfo()
        {
            var list = GetJsonResult<PushShiftRedditResult>(subRedditSearch);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].no_follow)
                {
                    list.RemoveAt(i--);
                }
                else if (_avoidURLsInText && list[i].isLink || list[i].selftext.IsHasUrl())
                {
                    list.RemoveAt(i--);
                }
                else if (_avoidVideos && list[i].hasVideo)
                {
                    list.RemoveAt(i--);
                }
            }

            return list;
        }

        public List<PushShiftCommentResult> GetCommentsInfo()
        {
            var list = GetJsonResult<PushShiftCommentResult>(commentSearch);

            for (int i = 0; i < list.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(list[i].body))
                {
                    list.RemoveAt(i--);
                }
                else if (_avoidURLsInText && list[i].body.IsHasUrl())
                {
                    list.RemoveAt(i--);
                }
                else if (list[i].body.Contains("[deleted]") || list[i].body.Contains("[removed]"))
                {
                    list.RemoveAt(i--);
                }
                else if (list[i].author.Contains("[deleted]") || list[i].author.Contains("[removed]"))
                {
                    list.RemoveAt(i--);
                }
            }

            return list;
        }

        public List<PushShiftCommentResult> GetRecycleCommentsInfo(int count)
        {
            List<PushShiftCommentResult> resultList = new List<PushShiftCommentResult>();
            int i = 0;
            while(i < count)
            {
                resultList.AddRange(GetCommentsInfo());
                After(++i + "d");
            }
            
            return resultList;
        }

        #region Parts
        private string getSearchParts()
        {
            string parts = "";
            parts += _before ?? "";
            parts += _after ?? "";
            parts += _score ?? "";
            parts += _size ?? "";
            parts += _author ?? "";
            parts += _selftext ?? "";
            parts += _avoidDeleted ?? "";
            parts += _avoidVideosStr ?? "";
            parts += _avoidNTFS ?? "";
            parts += _title ?? "";
            return parts;
        }

        public PushShiftSearch Before(string before)
        {
            _before = "&before=" + before;
            return this;
        }

        public PushShiftSearch After(string after)
        {
            _after = "&after=" + after;
            return this;
        }

        public PushShiftSearch ScoreMoreThan(int score)
        {
            _score = "&score>=" + score;
            return this;
        }

        public PushShiftSearch ScoreLessThan(int score)
        {
            _score = "&score<=" + score;
            return this;
        }

        public PushShiftSearch ScoreEquals(int score)
        {
            _score = "&score=" + score;
            return this;
        }

        public PushShiftSearch Size(int Size = 25)
        {
            _size = "&size=" + Size;
            return this;
        }

        public PushShiftSearch Author(params string[] author)
        {
            _author = String.Join("&author=", author);
            return this;
        }

        public PushShiftSearch SelfText(params string[] selftext)
        {
            _selftext = String.Join("&selftext=", selftext);
            return this;
        }

        public PushShiftSearch Title(string title)
        {
            _title = "&title=" + title;
            return this;
        }

        #region Avoid
        public PushShiftSearch AvoidDeleted(bool avoid = true)
        {
            if (avoid)
                _avoidDeleted = "&author!=[deleted]&selftext:not=[deleted]";
            return this;
        }

        public PushShiftSearch AvoidVideos(bool avoid = true)
        {
            if (avoid)
            {
                _avoidVideosStr = "&is_video=false";
            }
            else
            {
                _avoidVideosStr = null;
            }
            _avoidVideos = avoid;
            return this;
        }
        public PushShiftSearch AvoidNTFS(bool avoid = true)
        {
            if (avoid)
            {
                _avoidNTFS = "&over_18=false";
            }
            else
            {
                _avoidNTFS = null;
            }
            return this;
        }

        public PushShiftSearch AvoidURL(bool avoid = true)
        {
            _avoidURLsInText = avoid;
            return this;
        }

        #endregion

        #endregion

    }

    public static class PishShiftExtension
    {
        public static List<PushShiftCommentResult> LimitLength(this List<PushShiftCommentResult> obj, int max)
        {
            return obj.Where(i => i.body.Length >= 0 && i.body.Length <= max).ToList();
        }
        public static List<PushShiftCommentResult> LimitLength(this List<PushShiftCommentResult> obj, int min, int max)
        {
            return obj.Where(i => i.body.Length >= min && i.body.Length <= max).ToList();
        }

        public static List<PushShiftCommentResult> ConvertFromToEncode(this List<PushShiftCommentResult> obj, Encoding encFrom, Encoding encTo)
        {
            obj.ForEach(i => i.body = encTo.GetString(encFrom.GetBytes(i.body)));
            return obj;
        }

        public static List<PushShiftCommentResult> RemoveCharacters(this List<PushShiftCommentResult> obj, params char[] characters)
        {
            foreach (var ch in characters)
            {
                foreach (var item in obj)
                {
                    if (item.body.Contains(ch))
                    {
                        item.body = item.body.Replace(ch.ToString(), "");
                    }
                }
            }
            return obj;
        }

        public static List<PushShiftCommentResult> RemoveCharacters(this List<PushShiftCommentResult> obj, Regex regex)
        {   
            foreach (var item in obj)
            {
                item.body = regex.Replace(item.body, "");
            }
            return obj;
        }

        public static List<PushShiftCommentResult> RemoveUnicodeCharacters(this List<PushShiftCommentResult> obj)
        {
            foreach (var item in obj)
            {
                item.body = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(item.body));
            }
            return obj;
        }
    }

}
