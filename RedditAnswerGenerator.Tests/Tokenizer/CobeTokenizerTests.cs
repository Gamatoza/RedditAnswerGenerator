using RedditAnswerGenerator.Services;
using RedditAnswerGenerator.Services.Utils;
using RedditAnswerGenerator.Services.LearnModule;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace RedditAnswerGenerator.Tests
{
    public class CobeTokenizerTests
    {
        public Tokenizer tokenizer;

        [SetUp]
        public void setUp()
        {
            this.tokenizer = new CobeTokenizer();
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
            Assert.IsTrue(words.SequenceEqual(new List<string>() {
                "hi",
                "."
            }));
        }

        [Test]
        public void testSplitComma()
        {
            var words = this.tokenizer.split("hi, cobe");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "hi",
                ",",
                " ",
                "cobe"
            }));
        }

        [Test]
        public void testSplitDash()
        {
            var words = this.tokenizer.split("hi - cobe");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "hi",
                " ",
                "-",
                " ",
                "cobe"
            }));
        }

        [Test]
        public void testSplitMultipleSpacesWithDash()
        {
            var words = this.tokenizer.split("hi  -  cobe");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "hi",
                " ",
                "-",
                " ",
                "cobe"
            }));
        }

        [Test]
        public void testSplitLeadingDash()
        {
            var words = this.tokenizer.split("-foo");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "-foo"
            }));
        }

        [Test]
        public void testSplitLeadingSpace()
        {
            var words = this.tokenizer.split(" foo");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "foo"
            }));
            words = this.tokenizer.split("  foo");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "foo"
            }));
        }

        [Test]
        public void testSplitTrailingSpace()
        {
            var words = this.tokenizer.split("foo ");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "foo"
            }));
            words = this.tokenizer.split("foo  ");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "foo"
            }));
        }

        [Test]
        public void testSplitSmiles()
        {
            var words = this.tokenizer.split(":)");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                ":)"
            }));
            words = this.tokenizer.split(";)");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                ";)"
            }));
            // not smiles
            words = this.tokenizer.split(":(");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                ":("
            }));
            words = this.tokenizer.split(";(");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                ";("
            }));
        }

        [Test]
        public void testSplitUrl()
        {
            var words = this.tokenizer.split("http://www.google.com/");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "http://www.google.com/"
            }));
            words = this.tokenizer.split("https://www.google.com/");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "https://www.google.com/"
            }));
            // odd protocols
            words = this.tokenizer.split("cobe://www.google.com/");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "cobe://www.google.com/"
            }));
            words = this.tokenizer.split("cobe:www.google.com/");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "cobe:www.google.com/"
            }));
            words = this.tokenizer.split(":foo");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                ":",
                "foo"
            }));
        }

        [Test]
        public void testSplitMultipleSpaces()
        {
            var words = this.tokenizer.split("this is  a test");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "this",
                " ",
                "is",
                " ",
                "a",
                " ",
                "test"
            }));
        }

        [Test]
        public void testSplitVerySadFrown()
        {
            var words = this.tokenizer.split("testing :    (");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "testing",
                " ",
                ":    ("
            }));
            words = this.tokenizer.split("testing          :    (");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "testing",
                " ",
                ":    ("
            }));
            words = this.tokenizer.split("testing          :    (  foo");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "testing",
                " ",
                ":    (",
                " ",
                "foo"
            }));
        }

        [Test]
        public void testSplitHyphenatedWord()
        {
            var words = this.tokenizer.split("test-ing");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "test-ing"
            }));
            words = this.tokenizer.split(":-)");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                ":-)"
            }));
            words = this.tokenizer.split("test-ing :-) 1-2-3");
            Assert.IsTrue(words.SequenceEqual(new List<string> {
                "test-ing",
                " ",
                ":-)",
                " ",
                "1-2-3"
            }));
        }

        [Test]
        public void testSplitApostrophes()
        {
            var words = this.tokenizer.split("don't :'(");
            Assert.IsTrue(words.SequenceEqual(new List<string>() {
                "don't",
                " ",
                ":'("
            }));
        }

        [Test]
        public void testJoin()
        {
            Assert.AreEqual("foo bar baz", this.tokenizer.join(new List<string> {
                "foo",
                " ",
                "bar",
                " ",
                "baz"
            }));
        }

    }
}
