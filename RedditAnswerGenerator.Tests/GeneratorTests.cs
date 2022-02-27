using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RedditAnswerGenerator.Services;
using RedditAnswerGenerator.Services.LearnModule;

namespace RedditBot.Tests
{
    public class GeneratorTests
    {
        TextGenerator generator;

        [SetUp]
        public void Init()
        {
            generator = new TextGenerator("Etsy");
            generator.OutputPath = "username/";
        }

        [Test]
        public async Task LearnTest()
        {
            generator.LearnAsync(LearnMode.ByFileSize,5*1024*1024,true).Wait();

            Assert.IsTrue(File.Exists(generator.FullBrainPath));
        }

        [Test]
        public void AnswerTest()
        {
            string text = "Students compile a collection of their texts in a variety of genres over time and choose two pieces to present for summative assessment. In the majority of cases, the work in the student’s collection will arise from normal classwork, as the examples below illustrate. ";
            string answer = generator.GetAnswer(text);
            Assert.IsTrue(answer != text && !string.IsNullOrEmpty(answer));
        }
    }
}
