using Pastel;
using RedditAnswerGenerator.Services;
using RedditAnswerGenerator.Services.LearnModule;
using RedditAnswerGenerator.Services.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

        private static string fullBrainPath => (path ?? GeneratorSettings.BrainDefaultPath) + subRedditName + ".brain";
        private static string fullReplyPath => (path ?? GeneratorSettings.ReplyDefaultPath) + subRedditName + "_reply.txt";
        private static string? subRedditName { get; set; }
        private static string? answerString { get; set; }
        private static string? path { get; set; }
        private static int retryCount { get; set; }
        private static int commentNeedCount { get; set; }
        public static int Main(string[] argsv)
        {
            Console.Title = "RedditAnswerGenerator";
            Directory.CreateDirectory(GeneratorSettings.BrainDefaultPath);
            Directory.CreateDirectory(GeneratorSettings.ReplyDefaultPath);
            bool subredditNameFlag = false;
            bool learnFlag = false;
            bool answerFlag = false;
            bool removeOldFlag = false;
            bool prePathFlag = false;
            bool logsFlag = false;
            bool retryFlag = false;
            bool countFlag = false;
            PushShiftSearch search = null;

            foreach (var param in argsv)
            {
                switch (param)
                {
                    case "-n":
                    case "--name":
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

                    case "-ra":
                    case "--remove-after":
                        break;

                    case "-c":
                    case "--count":
                        countFlag = true;
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

                        if (search != null)
                        {
                            search.Loginning = true;
                        }
                        break;

                    default:
                        if (subredditNameFlag)
                        {
                            if (string.IsNullOrEmpty(param))
                            {
                                if (logsFlag)
                                {
                                    logger.Error("Subreddit was empty");
                                }
                                return -1;
                            }

                            subRedditName = param;
                            search = new PushShiftSearch(subRedditName,logsFlag);
                            subredditNameFlag = false;
                        }
                        else if (answerFlag)
                        {
                            if (string.IsNullOrEmpty(param))
                            {
                                if (logsFlag)
                                {
                                    logger.Error("Answer string was empty");
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
                                    logger.Error("Answer string was empty");
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
                        else if (countFlag)
                        {
                            int.TryParse(param, out var count);
                            commentNeedCount = count;
                        }
                        else
                        {
                            if (logsFlag)
                            {
                                logger.Warning($"Wrong parameter {param}");
                            }
                        }
                        break;
                }
            }

            if (!RedditHelper.CheckSubredditExists(subRedditName))
            {
                if (logsFlag)
                {
                    logger.Error("Subreddit is down or");
                }
                return -1;
            }

            if (learnFlag)
            {
                if (removeOldFlag)
                {
                    if (logsFlag)
                    {
                        logger.Info($"-ro flag detected, remove old brain file named {subRedditName}.brain");
                    }

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
                    DateTime time = DateTime.Now;
                    try
                    {
                        if(logsFlag)
                        {
                            logger.Action("Learning....");
                        }

                        int i = 1;
                        Brain.Init(fullBrainPath, order: 2);
                        var brain = new Brain(fullBrainPath);
                        int totalCount = 0;
                        while (countFlag ? (totalCount < commentNeedCount) : (i < GeneratorSettings.LearnRecycleCount))
                        {
                            var comments = search
                                .AvoidURL()
                                .AvoidDeleted()
                                .Size(GeneratorSettings.LearnCommentSize)
                                .After(i + "d")
                                .GetCommentsInfo()
                                .LimitLength(GeneratorSettings.CommentLengthMin, GeneratorSettings.CommentLengthMax)
                                .RemoveCharacters('\'', '`', '’')
                                .RemoveUnicodeCharacters()
                                .Distinct()
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
                            totalCount += comments.Count;
                            if (logsFlag)
                            {
                                logger.Info($"Comment after settings {comments.Count}, count left to: {commentNeedCount - totalCount}");
                            }
                        }
                        if (logsFlag)
                        {
                            logger.Info($"Learn ended! File {subRedditName}.brain created! Total commentary count {totalCount}");
                            logger.Info($"Total time spend: {(DateTime.Now - time).ToString().Pastel(Color.Coral)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (logsFlag)
                        {
                            logger.Error(ex);
                        }
                        return -1;
                    }
                }
                else
                {
                    if (logsFlag)
                    {
                        logger.Error("Brain file exists");
                    }
                }
            }

            if (answerFlag)
            {
                try
                {
                    if (logsFlag)
                    {
                        logger.Action("Getting answer...");
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
                                logger.Error(ex);
                            }
                        }
                    }
                    while (retryFlag || retryCount-- > 0);
                    
                }
                catch (Exception ex)
                {
                    if (logsFlag)
                    {
                        logger.Error(ex);
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
