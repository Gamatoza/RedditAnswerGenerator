using RedditAnswerGenerator.Services;
using RedditAnswerGenerator.Services.Utils;
using RedditAnswerGenerator.Services.LearnModule;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace RedditAnswerGenerator.Tests
{
    public class MegaHALTokenizerTests
    {
        public Tokenizer tokenizer;

        [SetUp]
        public void setUp()
        {
            this.tokenizer = new MegaHALTokenizer();
        }

        [Test]
        public void testSplitEmpty()
        {
            Assert.AreEqual(this.tokenizer.split("").Count, 0);
        }

        [Test]
        public void testSplitSentence()
        {
            var words = this.tokenizer.split("hi.");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "HI",
                "."
            }));
        }

        [Test]
        public void testSplitComma()
        {
            var words = this.tokenizer.split("hi, cobe");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "HI",
                ", ",
                "COBE",
                "."
            }));
        }

        [Test]
        public void testSplitImplicitStop()
        {
            var words = this.tokenizer.split("hi");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "HI",
                "."
            }));
        }

        [Test]
        public void testSplitUrl()
        {
            var words = this.tokenizer.split("http://www.google.com/");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "HTTP",
                "://",
                "WWW",
                ".",
                "GOOGLE",
                ".",
                "COM",
                "/."
            }));
        }

        [Test]
        public void testSplitApostrophe()
        {
            var words = this.tokenizer.split("hal's brain");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "HAL'S",
                " ",
                "BRAIN",
                "."
            }));
            words = this.tokenizer.split("',','");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "'",
                ",",
                "'",
                ",",
                "'",
                "."
            }));
        }

        [Test]
        public void testSplitAlphaAndNumeric()
        {
            var words = this.tokenizer.split("hal9000, test blah 12312");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "HAL",
                "9000",
                ", ",
                "TEST",
                " ",
                "BLAH",
                " ",
                "12312",
                "."
            }));
            words = this.tokenizer.split("hal9000's test");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "HAL",
                "9000",
                "'S",
                " ",
                "TEST",
                "."
            }));
        }

        [Test]
        public void testCapitalize()
        {
            var words = this.tokenizer.split("this is a test");
            Assert.AreEqual("This is a test.", this.tokenizer.join(words));
            words = this.tokenizer.split("A.B. Hal test test. will test");
            Assert.AreEqual("A.b. Hal test test. Will test.", this.tokenizer.join(words));
            words = this.tokenizer.split("2nd place test");
            Assert.AreEqual("2Nd place test.", this.tokenizer.join(words));
        }

    }
}
