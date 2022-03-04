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
            FileInfo file = new FileInfo("subs.txt");
            List<string> subReddits = File.ReadAllLines(file.FullName).ToList();
            int subredditIndex = 0;
            int limit = 20;
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
                }
                Thread.Sleep(3000);
            }
        }
    }
}
