using RedditAnswerGenerator.Services;
using RedditAnswerGenerator.Services.Utils;
using RedditAnswerGenerator.Services.LearnModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace RedditAnswerGenerator.Tests
{
    
    public class LearnTests : BaseTests
    {
        [SetUp]
        public void setUp()
        {
            if (File.Exists(TEST_BRAIN_FILE))
            {
                File.Delete(TEST_BRAIN_FILE);
            }
        }

        [Test]
        public void testExpandContexts()
        {
            Brain.init(TEST_BRAIN_FILE, order: 2);
            var brain = new Brain(TEST_BRAIN_FILE);
            var tokens = new List<string>() {
                "this",
                Convert.ToString(Brain.SPACE_TOKEN_ID),
                "is",
                Convert.ToString(Brain.SPACE_TOKEN_ID),
                "a",
                Convert.ToString(Brain.SPACE_TOKEN_ID),
                "test"
            };

            var edges = brain._to_edges(tokens);
            var example = new List<Tuple<List<string>, bool>>() {
                Tuple.Create(new List<string>() { "1", "1" }, false),
                Tuple.Create(new List<string>() { "1", "this" }, false),
                Tuple.Create(new List<string>() { "this", "is" }, true),
                Tuple.Create(new List<string>() { "is", "a" }, true),
                Tuple.Create(new List<string>() { "a", "test" }, true),
                Tuple.Create(new List<string>() { "test", "1" }, false),
                Tuple.Create(new List<string>() { "1", "1" }, false)
            };

            Assert.IsTrue(!example.Except(edges, new EdgeTupleEqualityComparer()).Any());

            tokens = new List<string>() {
                "this",
                "is",
                "a",
                "test"
            };

            edges = brain._to_edges(tokens);
            example = new List<Tuple<List<string>, bool>>() {
                Tuple.Create(new List<string>() { "1", "1" }, false),
                Tuple.Create(new List<string>() { "1", "this" }, false),
                Tuple.Create(new List<string>() { "this", "is" }, false),
                Tuple.Create(new List<string>() { "is", "a" }, false),
                Tuple.Create(new List<string>() { "a", "test" }, false),
                Tuple.Create(new List<string>() { "test", "1" }, false),
                Tuple.Create(new List<string>() { "1", "1" }, false),
            };

            Assert.IsTrue(!example.Except(edges, new EdgeTupleEqualityComparer()).Any());


        }

        [Test]
        public void testExpandGraph()
        {
            Brain.init(TEST_BRAIN_FILE, order: 2);
            var brain = new Brain(TEST_BRAIN_FILE);
            var tokens = new List<string> {
                "this",
                Convert.ToString(Brain.SPACE_TOKEN_ID),
                "is",
                Convert.ToString(Brain.SPACE_TOKEN_ID),
                "a",
                Convert.ToString(Brain.SPACE_TOKEN_ID),
                "test"
            };

            var graph = brain._to_graph(brain._to_edges(tokens));
            var example = new List<Tuple<List<string>, bool, List<string>>>() {
                Tuple.Create(new List<string>() { "1", "1" }, false, new List<string>() { "1", "this" }),
                Tuple.Create(new List<string>() { "1", "this" }, true, new List<string>() { "this", "is" }),
                Tuple.Create(new List<string>() { "this", "is" }, true, new List<string>() { "is", "a" }),
                Tuple.Create(new List<string>() { "is", "a" }, true, new List<string>() { "a", "test" }),
                Tuple.Create(new List<string>() { "a", "test" }, false, new List<string>() { "test", "1" }),
                Tuple.Create(new List<string>() { "test", "1" }, false, new List<string>() { "1", "1" })
            };

            Assert.IsTrue(!example.Except(graph, new GraphTupleEqualityComparer()).Any());
        }

        [Test]
        public void testLearn()
        {
            Brain.init(TEST_BRAIN_FILE, order: 2);
            var brain = new Brain(TEST_BRAIN_FILE);

            brain.learn("this is a test");
            brain.learn("this is also a test");
        }

        [Test]
        public void testLearnStems()
        {
            Brain.init(TEST_BRAIN_FILE, order: 2);
            var brain = new Brain(TEST_BRAIN_FILE);
            brain.set_stemmer("english");

            brain.learn("this is testing");
            var dt = SqliteHelper.Execute(brain.graph._conn, "SELECT count(*) FROM token_stems");

            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);

            var stem_count = dt.Rows[0];
            Assert.AreEqual((long)3, stem_count[0]);
            Assert.IsTrue(brain.graph.get_token_stem_id(brain.stemmer.stem("test")).SequenceEqual(brain.graph.get_token_stem_id(brain.stemmer.stem("testing"))));
        }

    }
}
