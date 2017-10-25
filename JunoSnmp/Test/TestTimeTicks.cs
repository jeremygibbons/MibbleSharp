

namespace JunoSnmp.Test
{
    using JunoSnmp.SMI;
    using NUnit.Framework;

    [TestFixture]
    class TestTimeTicks
    {
        public TestTimeTicks()
        {
        }

        [TestCase]
        public void TestToString()
        {
            TimeTicks timeticks = new TimeTicks();
            string stringRet = timeticks.ToString();
            Assert.AreEqual("0:00:00.00", stringRet);
        }

        [TestCase]
        public void TestToMaxValue()
        {
            TimeTicks timeticks = new TimeTicks(4294967295L);
            string stringRet = timeticks.ToString();
            Assert.AreEqual("497 days, 2:27:52.95", stringRet);
        }
    }
}
