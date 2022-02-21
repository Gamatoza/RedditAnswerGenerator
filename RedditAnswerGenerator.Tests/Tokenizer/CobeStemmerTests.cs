using RedditAnswerGenerator.Services;
using RedditAnswerGenerator.Services.Utils;
using RedditAnswerGenerator.Services.LearnModule;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
namespace RedditAnswerGenerator.Tests
{
    public class CobeStemmerTests
    {
        public CobeStemmer stemmer;

        [SetUp]
        public void setUp()
        {
            this.stemmer = new CobeStemmer("english");
        }

        [Test]
        public void testStemmer()
        {
            Assert.AreEqual("foo", this.stemmer.stem("foo"));
            Assert.AreEqual("jump", this.stemmer.stem("jumping"));
            Assert.AreEqual("run", this.stemmer.stem("running"));
        }

        [Test]
        public void testStemmerCase()
        {
            Assert.AreEqual("foo", this.stemmer.stem("Foo"));
            Assert.AreEqual("foo", this.stemmer.stem("FOO"));
            Assert.AreEqual("foo", this.stemmer.stem("FOO'S"));
            Assert.AreEqual("foo", this.stemmer.stem("FOOING"));
            Assert.AreEqual("foo", this.stemmer.stem("Fooing"));
        }

    }
}
