using RedditAnswerGenerator.Services;
using RedditAnswerGenerator.Services.LearnModule;
using RedditAnswerGenerator.Services.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RedditAnswerGenerator
{
    public class Program
    {
        private static string fullBrainPath => (path ?? Settings.BrainDefaultPath) + subRedditName + ".brain";
        private static string fullReplyPath => (path ?? Settings.ReplyDefaultPath) + subRedditName + "_reply.txt";
        private static string? subRedditName { get; set; }
        private static string? answerString { get; set; }
        private static string? path { get; set; }
        private static int retryCount { get; set; }
        public static int Main(string[] argsv)
        {
            Console.Title = "RedditAnswerGenerator";
            Directory.CreateDirectory(Settings.BrainDefaultPath);
            Directory.CreateDirectory(Settings.ReplyDefaultPath);
            bool subredditNameFlag = false;
            bool learnFlag = false;
            bool answerFlag = false;
            bool removeOldFlag = false;
            bool prePathFlag = false;
            bool logsFlag = false;
            bool retryFlag = false;
            PushShiftSearch search = null;

            foreach (var param in argsv)
            {
                switch (param)
                {
                    case "-s":
                    case "--subreddit":
                        subredditNameFlag = true;
                        break;

                    case "-l":
                    case "--learn":
                        learnFlag = true;
                        break;

                    case "-a":
                    case "--answer":
                        answerFlag = true;
                        break;

                    case "-ro":
                    case "--remove-old":
                        removeOldFlag = true;
                        break;

                    case "-p":
                    case "--path":
                        prePathFlag = true;
                        break;

                    case "--retry":
                        retryFlag = true;
                        break;

                    case "--logs":
                        logsFlag = true;
                        break;

                    default:
                        if (subredditNameFlag)
                        {
                            if (string.IsNullOrEmpty(param))
                            {
                                if (logsFlag)
                                {
                                    Log.Error("Subreddit was empty");
                                }
                                return -1;
                            }

                            subRedditName = param;
                            search = new PushShiftSearch(subRedditName);
                            subredditNameFlag = false;
                        }
                        else if (answerFlag)
                        {
                            if (string.IsNullOrEmpty(param))
                            {
                                if (logsFlag)
                                {
                                    Log.Error("Answer string was empty");
                                }

                                return -1;
                            }
                            
                            answerString = param;
                        }
                        else if (prePathFlag)
                        {
                            if (string.IsNullOrEmpty(param))
                            {
                                if (logsFlag)
                                {
                                    Log.Error("Answer string was empty");
                                }
                                return -1;
                            }

                            path = param;

                            if (param.Last() != '\\')
                            {
                                path += '\\';
                            }

                            prePathFlag = false;
                        }
                        else if (retryFlag)
                        {
                            int.TryParse(param, out var count);
                            retryCount = count;
                        }
                        else
                        {
                            if (logsFlag)
                            {
                                Log.Warning($"Wrong parameter {param}");
                            }
                        }
                        break;
                }
            }
            if (!RedditHelper.CheckSubredditExists(subRedditName))
            {
                Log.Error("Subreddit is down or");
                return -1;
            }
            if (learnFlag)
            {
                if (removeOldFlag)
                {
                    if (File.Exists(fullBrainPath))
                    {
                        File.Delete(fullBrainPath);
                    }
                }

                /*.GetRecycleCommentsInfo(Settings.LearnRecycleCount)
                .LimitLength(Settings.CommentLengthMin, Settings.CommentLengthMax);*/
                //.ConvertFromToEncode(Encoding.UTF8,Encoding.Unicode);

                if (!File.Exists(fullBrainPath))
                {
                    try
                    {
                        if(logsFlag)
                        {
                            Log.Action("Learning....");
                        }

                        int i = 1;
                        Brain.init(fullBrainPath, order: 2);
                        var brain = new Brain(fullBrainPath);

                        while (i < Settings.LearnRecycleCount)
                        {
                            var comments = search
                                .AvoidURL()
                                .AvoidDeleted()
                                .Size(Settings.LearnCommentSize)
                                .After(i + "d")
                                .GetCommentsInfo()
                                .LimitLength(Settings.CommentLengthMin, Settings.CommentLengthMax)
                                .RemoveCharacters('\'', '`', '’')
                                .RemoveUnicodeCharacters()
                                .Select(item => item.body).ToList();
                            if (comments != null)
                            {
                                Task.Run(async () => await Learn(brain, comments));
                                i++;
                            }
                            else
                            {
                                Thread.Sleep(2000);
                            }
                        }
                        if (logsFlag)
                        {
                            Log.Info($"Learn ended! File {subRedditName}.brain created!");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (logsFlag)
                        {
                            Log.Error(ex);
                        }
                        return -1;
                    }
                }
                else
                {
                    if (logsFlag)
                    {
                        Log.Error("Brain file exists");
                    }
                }
            }

            if (answerFlag)
            {
                try
                {
                    if (logsFlag)
                    {
                        Log.Action("Getting answer...");
                    }

                    do
                    {
                        try
                        {
                            var brain = new Brain(fullBrainPath);
                            var reply = brain.reply(answerString);
                            File.WriteAllText(fullReplyPath, reply);
                            Console.WriteLine(reply);
                            return 0;
                        }
                        catch (Exception ex)
                        {
                            if (logsFlag)
                            {
                                Log.Error(ex);
                            }
                        }
                    }
                    while (retryFlag || retryCount-- > 0);
                    
                }
                catch (Exception ex)
                {
                    if (logsFlag)
                    {
                        Log.Error(ex);
                    }
                    return -1;
                }
            }

            return 0;
        }

        public static async Task Learn(Brain brain, List<string> comments)
        {
            foreach (var item in comments)
            {
                try
                {
                    await Task.Run(() => brain.learn(item));
                }
                catch (Exception)
                { }
            }
        }
    }
}
