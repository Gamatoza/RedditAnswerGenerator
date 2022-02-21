using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedditAnswerGenerator.Tests
{
    public abstract class BaseTests
    {
        private TestContext TestContextInstance;

        public TestContext TestContext
        {
            get
            {
                return TestContextInstance;
            }
            set
            {
                TestContextInstance = value;
            }
        }

        public string TEST_BRAIN_FILE = "test_cobe.brain";

    }
}
