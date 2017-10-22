

namespace JunoSnmp.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using JunoSnmp.SMI;
    using NUnit.Framework;

    [TestFixture]
    class TestOctetString
    {

        private OctetString octetString = new OctetString();


        protected void tearDown()
        {
            octetString = null;
        }

        [TestCase]
        public void TestConstructors()
        {
            byte[] ba = {
        (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g', (byte)'h', (byte)'i'};

            octetString = new OctetString(ba);

            Assert.AreEqual(octetString.ToString(), "abcdefghi");

            octetString = new OctetString(ba, 2, 2);
            Assert.AreEqual(octetString.ToString(), "cd");
        }

        [TestCase]
        public void TestSlip()
        {
            string s = "A short string with several delimiters  and a short word!";
            OctetString sp = new OctetString(s);
            OctetString[] words = OctetString.Split(sp, new OctetString("! ")).ToArray();
            var sarr = s.Split(new char[] { '!', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(sarr.Length, words.Length);
            for(int i = 0; i < words.Length; i++)
            {
                Assert.AreEqual(words[i].ToString(), sarr[i]);
            }
        }

        [TestCase]
        public void TestIsPrintable()
        {
            OctetString nonPrintable = OctetString.FromHexString("1C:32:41:1C:4E:38");
            Assert.False(nonPrintable.IsPrintable);
        }

    }
}
