using RedditAnswerGenerator;
using RedditAnswerGenerator.Services.Utils;
using RedditAnswerGenerator.Services.LearnModule;
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace RedditAnswerGenerator.Tests
{
    public class ReplyTests : BaseTests
    {
        public Brain _brain;

        [SetUp]
        public void setUp()
        {
            if (File.Exists(TEST_BRAIN_FILE))
            {
                File.Delete(TEST_BRAIN_FILE);
            }
            Brain.Init(TEST_BRAIN_FILE, order: 2);
            this._brain = new Brain(TEST_BRAIN_FILE);
        }

        [Test]
        public void testReply()
        {
            var brain = this._brain;
            brain.learn("this is a test");
            brain.reply("this is a test");
        }

    }
}
