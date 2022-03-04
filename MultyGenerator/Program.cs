using System;
using RedditAnswerGenerator.Services;
using RedditAnswerGenerator;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RedditAnswerGenerator.Services.LearnModule;
using System.Threading.Tasks;

namespace MultyGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "MultyGenerator";
            FileInfo file = new FileInfo("subs.txt");
            List<string> subReddits = File.ReadAllLines(file.FullName).Where(sub => !string.IsNullOrEmpty(sub)).ToList();
            int subredditIndex = 0;
            int limit = AppSettings.LimitThreadCount;
            List<Task> tasks = new List<Task>();
            while (tasks.Count < subReddits.Count)
            {
                if (limit > 0 && subredditIndex < subReddits.Count)
                {
                    var task = new Task(async () =>
                    {
                        limit--;
                        TextGenerator generator = new TextGenerator(subReddits[subredditIndex++]);
                        await generator.LearnAsync(LearnMode.ByCommentCount, GeneratorSettings.LearnCommentCount);
                        limit++;
                    });
                    tasks.Add(task);
                    task.Start();
                }
                else
                {
                    Task.WhenAny(tasks).Wait();
                    Thread.Sleep(9000);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
