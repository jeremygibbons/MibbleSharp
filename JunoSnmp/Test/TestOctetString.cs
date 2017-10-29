// <copyright file="TestOctetString.cs" company="None">
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
//    Original Java code from Snmp4J Copyright (C) 2003-2017 Frank Fock and 
//    Jochen Katz (SNMP4J.org). All rights reserved.
//    </para><para>
//    C# conversion Copyright (c) 2017 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Test
{
    using System.Linq;
    using JunoSnmp.SMI;
    using NUnit.Framework;

    [TestFixture]
    class TestOctetString
    {
        [TestCase]
        public void TestConstructors()
        {
            byte[] ba = {
                (byte)'a', (byte)'b', (byte)'c',
                (byte)'d', (byte)'e', (byte)'f',
                (byte)'g', (byte)'h', (byte)'i'
            };

            OctetString octetString = new OctetString(ba);

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
