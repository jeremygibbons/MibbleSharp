// <copyright file="TestCounter64.cs" company="None">
//    <para>
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at</para>
//    <para>
//    http://www.apache.org/licenses/LICENSE-2.0
//    </para><para>
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.</para>
//    <para>
//    Original Java code from Snmp4J Copyright (C) 2003-2016 Frank Fock and 
//    Jochen Katz (SNMP4J.org). All rights reserved.
//    </para><para>
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Test
{
    using System.IO;
    using JunoSnmp.ASN1;
    using JunoSnmp.SMI;
    using NUnit.Framework;

    [TestFixture]
    class TestCounter64
    {
        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TestCounter64()
        {
        }

        [TestCase]
        public void TestToString()
        {
            Counter64 counter64 = new Counter64(unchecked((long)0xFFFFFFFFFFFFFFFFL));
            string stringRet = counter64.ToString();
            Assert.AreEqual("18446744073709551615", stringRet);
        }
        public void TestCompareTo()
        {
            Counter64 counter64 = new Counter64(unchecked((long)0xFFFFFFFFFFFFFFFFL));
            Counter64 counter32 = new Counter64(0x00000000FFFFFFFFL);
            Counter64 counter0 = new Counter64(0);
            Counter64 counter1 = new Counter64(1);
            int intRet = counter64.CompareTo(counter0);
            Assert.AreEqual(1, intRet);
            Assert.AreEqual(-1, counter0.CompareTo(counter64));
            Assert.AreEqual(-1, counter32.CompareTo(counter64));
            Assert.AreEqual(1, counter64.CompareTo(counter32));
            Assert.AreEqual(0, counter32.CompareTo(counter32));
            Assert.AreEqual(0, counter64.CompareTo(counter64));
            Assert.AreEqual(0, counter0.CompareTo(counter0));
            Assert.AreEqual(1, counter1.CompareTo(counter0));
            Assert.AreEqual(-1, counter0.CompareTo(counter1));

            long l = 0;
            for (int i = 0; i < 64; i++)
            {
                Counter64 lesser = new Counter64(l);
                Counter64 greater = new Counter64(1L << i);
                Assert.AreEqual(-1, lesser.CompareTo(greater));
                Assert.AreEqual(1, greater.CompareTo(lesser));
                l = greater.GetValue();
            }
        }

        [TestCase]
        public void TestEquals()
        {
            Counter64 counter64 = new Counter64(unchecked((long)0xFFFFFFFFFFFFFFFFL));
            IVariable o1 = new Counter64(unchecked((long)0xFFFFFFFFFFFFFFFFL));
            bool booleanRet = counter64.Equals(o1);
            Assert.IsTrue(booleanRet);
        }

        [TestCase]
        public void TestBER0()
        {
            MemoryStream bos64 = new MemoryStream();
            try
            {
                BER.EncodeUnsignedInt64(bos64, (byte)SMIConstants.SyntaxCounter64, 0);
            }
            catch (IOException ex)
            {
                log.Debug(ex.StackTrace);
            }

            OctetString os64 = new OctetString(bos64.ToArray());
            MemoryStream bos32 = new MemoryStream();
            try
            {
                BER.EncodeUnsignedInteger(bos32, (byte)SMIConstants.SyntaxCounter64, 0);
            }
            catch (IOException ex)
            {
                log.Debug(ex.StackTrace);
            }
            OctetString os32 = new OctetString(bos32.ToArray());
            Assert.AreEqual(os32, os64);

        }

        [TestCase]
        public void TestBER3()
        {
            MemoryStream bos = new MemoryStream();
            try
            {
                BER.EncodeUnsignedInt64(bos, (byte)SMIConstants.SyntaxCounter64, 3);
            }
            catch (IOException ex)
            {
                log.Debug(ex.StackTrace);
            }
            OctetString os = new OctetString(bos.ToArray());
            Assert.AreEqual(OctetString.FromHexString("46:01:03"), os);
        }

    }
}
