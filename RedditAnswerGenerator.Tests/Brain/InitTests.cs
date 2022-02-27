using RedditAnswerGenerator.Services;
using RedditAnswerGenerator.Services.Utils;
using RedditAnswerGenerator.Services.LearnModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace RedditAnswerGenerator.Tests
{
    public class InitTests : BaseTests
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
        public void testInit()
        {
            Brain.Init(TEST_BRAIN_FILE);
            Assert.IsTrue(File.Exists(TEST_BRAIN_FILE), "missing brain file after init");

            var brain = new Brain(TEST_BRAIN_FILE);
            Assert.IsNotNull(brain.order, "missing brain order after init");
            Assert.IsNotNull(brain._end_token_id, "missing brain _end_token_id after init");
        }

        [Test]
        public void testInitWithOrder()
        {
            var order = 2;
            Brain.Init(TEST_BRAIN_FILE, order: order);
            var brain = new Brain(TEST_BRAIN_FILE);
            Assert.AreEqual(order, brain.order);
        }

        [Test]
        public void testVersion()
        {
            Brain.Init(TEST_BRAIN_FILE);
            var brain = new Brain(TEST_BRAIN_FILE);
            Assert.AreEqual("2", brain.graph.get_info_text("version"));
        }

        [Test]
        public void testEmptyReply()
        {
            Brain.Init(TEST_BRAIN_FILE);
            var brain = new Brain(TEST_BRAIN_FILE);
            Assert.IsTrue(!string.IsNullOrEmpty(brain.reply("")));
        }

        [Test]
        public void testWrongVersion()
        {
            Brain.Init(TEST_BRAIN_FILE);
            // manually change the brain version to 1
            var brain = new Brain(TEST_BRAIN_FILE);
            brain.graph.set_info_text("version", "1");
            //brain.graph.commit();
            //brain.graph.close();
            try
            {
                new Brain(TEST_BRAIN_FILE);
            }
            catch (CobeError e)
            {
                Assert.IsTrue(Convert.ToString(e.Message).Contains("cannot read a version"));
            }
        }

        [Test]
        public void testInitWithTokenizer()
        {
            var tokenizer = "MegaHAL";
            Brain.Init(TEST_BRAIN_FILE, order: 2, tokenizer: tokenizer);
            var brain = new Brain(TEST_BRAIN_FILE);
            Assert.IsTrue(brain.tokenizer is MegaHALTokenizer);
        }

        [Test]
        public void testInfoText()
        {
            var order = 2;
            Brain.Init(TEST_BRAIN_FILE, order: order);
            var brain = new Brain(TEST_BRAIN_FILE);
            var db = brain.graph;
            var key = "test_text";
            Assert.AreEqual(null, db.get_info_text(key));
            db.set_info_text(key, "test_value");
            Assert.AreEqual("test_value", db.get_info_text(key));
            db.set_info_text(key, "test_value2");
            Assert.AreEqual("test_value2", db.get_info_text(key));
            db.set_info_text(key, null);
            Assert.AreEqual(null, db.get_info_text(key));
        }

        //[Test]
        //public void testInfoPickle()
        //{
        //    var order = 2;
        //    Brain.init(TEST_BRAIN_FILE, order: order);
        //    var brain = new Brain(TEST_BRAIN_FILE);
        //    var db = brain.graph;
        //    var key = "pickle_test";
        //    var obj = new Dictionary<object, object> {
        //        { "dummy", "object"},
        //        { "to", "pickle"}};
        //    db.set_info_text(key, pickle.dumps(obj));
        //    var get_info_text = () => pickle.loads(db.get_info_text(key, text_factory: str));
        //}

    }
}
