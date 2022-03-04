using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pastel;
using RedditAnswerGenerator.Services.Extensions;
using RedditAnswerGenerator.Services.LearnModule;
using RedditAnswerGenerator.Services.Utils;

namespace RedditAnswerGenerator.Services
{
    public class TextGenerator
    {
        #region Locals
        public string FullBrainPath => OutputPath + GeneratorSettings.BrainDefaultPath + SubRedditName + ".brain";
        public string FullReplyPath => OutputPath + GeneratorSettings.ReplyDefaultPath + SubRedditName + "_reply.txt";
        private string? SubRedditName { get; set; }
        public string? OutputPath { get; set; }
        private PushShiftSearch pushShift { get; set; }
        private Logger logger;
        #endregion
        public TextGenerator (string subReddit)
        {
            pushShift = new PushShiftSearch(subReddit);
            SubRedditName = subReddit;
            logger = new Logger(subReddit);
        }

        public bool IsBrainExists() => File.Exists(FullBrainPath);

        public string GetAnswer(string textToAnswer, bool writeFile = false, int retryCount = 0)
        {
            string reply = string.Empty;
            if (File.Exists(FullBrainPath))
            {
                Directory.CreateDirectory(OutputPath + GeneratorSettings.BrainDefaultPath);

                if (!textToAnswer.IsUnicode())
                {
                    logger.Warning("Text not on formate unicode, rebuild");
                    textToAnswer = textToAnswer.RemoveUnicodeCharacters();
                }

                logger.Action("Getting answer...");

                do
                {
                    try
                    {
                        var brain = new Brain(FullBrainPath);
                        reply = brain.reply(textToAnswer);

                        if (writeFile)
                        {
                            File.WriteAllText(FullReplyPath, reply);
                        }

                        if (string.IsNullOrWhiteSpace(reply))
                        {
                            retryCount--;
                            continue;
                        }

                        logger.Info("Answer found!");
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.Warning("Answer not found!");
                        logger.Error(ex);
                    }
                }
                while (retryCount-- > 0);
            }
            else
            {
                logger.Warning("Answer not found!");
            }
            return reply;
        }

        delegate bool Condition();
        public async Task<bool> LearnAsync(LearnMode mode, long comparator, bool rewriteBrains = false)
        {

            if (rewriteBrains && File.Exists(FullBrainPath))
            {
                logger.Info($"Remove old brain file named {SubRedditName}.brain");
                File.Delete(FullBrainPath);
            }

            Directory.CreateDirectory(OutputPath + GeneratorSettings.BrainDefaultPath);
            
            if (File.Exists(FullBrainPath))
            {
                logger.Info($"File {SubRedditName}.brain already exists!");
                return false;
            }

            try
            {
                DateTime time = DateTime.Now;
                logger.Action("Learning....");

                FileInfo brainFile = new FileInfo(FullBrainPath);
                Brain.Init(FullBrainPath, order: 2);
                var brain = new Brain(FullBrainPath);
                int dayCount = 1;
                int totalCount = 0;
                long sizeCount = 0;

                Condition exitCondition;

                switch (mode)
                {
                    case LearnMode.ByCommentCount:
                        exitCondition = () => totalCount < comparator;
                        break;
                    case LearnMode.ByDaysPass:
                        exitCondition = () => dayCount < comparator;
                        break;
                    case LearnMode.ByFileSize:
                        exitCondition = () => sizeCount < comparator;
                        break;
                    default:
                        return false;
                }

                while (exitCondition())
                {
                    var comments = pushShift
                        .AvoidURL()
                        .AvoidDeleted()
                        .Size(500)
                        .After(dayCount + "d")
                        .GetCommentsInfo()
                        .LimitLength(GeneratorSettings.CommentLengthMin, GeneratorSettings.CommentLengthMax)
                        .RemoveCharacters('\'', '`', '’')
                        .RemoveUnicodeCharacters()
                        .Distinct()
                        .Select(item => item.body).ToList();

                    if (comments.Any())
                    {
                        await Task.Run(async () => await LearnCommentsAsync(brain, comments));
                        dayCount++;
                        totalCount += comments.Count;
                        brainFile = new FileInfo(FullBrainPath);
                        sizeCount = brainFile.Length;
                        logger.Info($"Comment after settings {comments.Count}");
                    }
                    else
                    {
                        Thread.Sleep(3000);
                    }
                }

                logger.Info($"Learn ended! File {SubRedditName}.brain created! Total commentary count {totalCount}");
                logger.Info($"File size: {sizeCount} bytes");
                logger.Info($"Total time spend: {(DateTime.Now - time).ToString().Pastel(Color.Coral)}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Info($"Learn faild! File {SubRedditName}.brain not created!");
                logger.Error(ex);
                return false;
            }

        }

        private async Task LearnCommentsAsync(Brain brain, List<string> comments)
        {
            foreach (var item in comments)
            {
                try
                {
                    await Task.Run(() => brain.learn(item));
                }
                catch (Exception ex)
                {}
            }
        }
    }

    
}
