using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Assertion = MoreLinq.Pull.Assertion;
using MoreLinq.Pull;

namespace MoreLinq.Test.Pull
{
    [TestFixture]
    public class AssertionTest
    {
        #region AssertCount

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssertCountNullSequence()
        {
            Assertion.AssertCount<object>(null, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AssertCountNegativeCount()
        {
            Assertion.AssertCount(new object[0], -1);
        }

        [Test]
        public void AssertCountSequenceWithMatchingLength()
        {
            "foo,bar,baz".Split(',').AssertCount(3).Exhaust();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void AssertCountShortSequence()
        {
            "foo,bar,baz".Split(',').AssertCount(5).Exhaust();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void AssertCountLongSequence()
        {
            "foo,bar,baz".Split(',').AssertCount(2).Exhaust();
        }

        [Test]
        public void AssertCountDefaultExceptionMessageVariesWithCase()
        {
            var tokens = "foo,bar,baz".Split(',');
            Exception e1 = null, e2 = null;
            try
            {
                tokens.AssertCount(5).Exhaust();
                Assert.Fail("Exception expected.");
            }
            catch (Exception e)
            {
                e1 = e;
            }
            try
            {
                tokens.AssertCount(2).Exhaust();
                Assert.Fail("Exception expected.");
            }
            catch (Exception e)
            {
                e2 = e;
            }
            Assert.That(e1.Message, Is.Not.EqualTo(e2.Message));
        }

        [Test]
        public void AssertCountLongSequenceWithErrorSelector()
        {
            try
            {
                "foo,bar,baz".Split(',').AssertCount(2, (cmp, count) => new TestException(cmp, count)).Exhaust();
                Assert.Fail("Exception expected.");
            }
            catch (TestException e)
            {
                Assert.That(e.Cmp, Is.GreaterThan(0));
                Assert.That(e.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void AssertCountShortSequenceWithErrorSelector()
        {
            try
            {
                "foo,bar,baz".Split(',').AssertCount(5, (cmp, count) => new TestException(cmp, count)).Exhaust();
                Assert.Fail("Exception expected.");
            }
            catch (TestException e)
            {
                Assert.That(e.Cmp, Is.LessThan(0));
                Assert.That(e.Count, Is.EqualTo(5));
            }
        }
        
        private sealed class TestException : Exception
        {
            public int Cmp { get; private set; }
            public int Count { get; private set; }

            public TestException(int cmp, int count)
            {
                Cmp = cmp;
                Count = count;
            }
        }

        [Test]
        public void AssertCountIsLazy()
        {
            Assertion.AssertCount(new BreakingSequence<object>(), 0);
        }
        
        #endregion
    }
}