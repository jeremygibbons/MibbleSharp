// <copyright file="TestOID.cs" company="None">
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
    using System;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;
    using NUnit.Framework;

    [TestFixture]
    class TestOID
    {
        private OID oID = new OID(SnmpConstants.usmStatsUnknownEngineIDs);

        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [TestCase]
        public void TestCompareTo()
        {
            OID o = SnmpConstants.usmStatsNotInTimeWindows;
            int expectedReturn = 1;
            int actualReturn = oID.CompareTo(o);
            Assert.AreEqual(expectedReturn, actualReturn);

            o = SnmpConstants.usmStatsUnknownEngineIDs;
            expectedReturn = 0;
            actualReturn = oID.CompareTo(o);
            Assert.AreEqual(expectedReturn, actualReturn);

            o = SnmpConstants.usmStatsWrongDigests;
            expectedReturn = -1;
            actualReturn = oID.CompareTo(o);
            Assert.AreEqual(expectedReturn, actualReturn);

            OID a = new OID(new long[] { 1, 2, 3, 6, 0x80000000 });
            OID b = new OID(new long[] { 1, 2, 3, 6, 0x80000001 });
            expectedReturn = 1;
            actualReturn = b.CompareTo(a);
            Assert.AreEqual(expectedReturn, actualReturn);

            expectedReturn = -1;
            actualReturn = a.CompareTo(b);
            Assert.AreEqual(expectedReturn, actualReturn);
        }

        [TestCase]
        public void testLeftMostCompare()
        {
            OID other = SnmpConstants.snmpInASNParseErrs;
            int n = Math.Min(other.Size, oID.Size);
            int expectedReturn = 1;
            int actualReturn = oID.LeftMostCompare(n, other);
            Assert.AreEqual(expectedReturn, actualReturn);
        }

        [TestCase]
        public void testRightMostCompare()
        {
            int n = 2;
            OID other = SnmpConstants.usmStatsUnsupportedSecLevels;
            int expectedReturn = 1;
            int actualReturn = oID.RightMostCompare(n, other);
            Assert.AreEqual(expectedReturn, actualReturn);
        }

        [TestCase]
        public void TestPredecessor()
        {
            OID oid = new OID("1.3.6.4.1.5");
            PrintOIDs(oid);
            Assert.AreEqual(oid.Predecessor.Successor, oid);
            oid = new OID("1.3.6.4.1.5.0");
            PrintOIDs(oid);
            Assert.AreEqual(oid.Predecessor.Successor, oid);
            oid = new OID("1.3.6.4.1.5.2147483647");
            PrintOIDs(oid);
            Assert.AreEqual(oid.Predecessor.Successor, oid);
        }

        private static void PrintOIDs(OID oid)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("OID=" + oid + ", predecessor=" + oid.Predecessor +
                             ",successor=" + oid.Successor);
            }
        }

        [TestCase]
        public void TestStartsWith()
        {
            OID other = new OID(SnmpConstants.usmStatsDecryptionErrors.GetValue());
            other.RemoveLast();
            other.RemoveLast();
            bool expectedReturn = true;
            bool actualReturn = oID.StartsWith(other);
            Assert.AreEqual(expectedReturn, actualReturn, "Return value");

            other = new OID(SnmpConstants.usmStatsUnknownEngineIDs.GetValue());
            expectedReturn = true;
            actualReturn = oID.StartsWith(other);
            Assert.AreEqual(expectedReturn, actualReturn, "Return value");

            other = new OID(SnmpConstants.usmStatsUnknownEngineIDs.GetValue());
            other.Append("33.44");
            expectedReturn = false;
            actualReturn = oID.StartsWith(other);
            Assert.AreEqual(expectedReturn, actualReturn, "Return value");
        }

        [TestCase]
        public void TestStringParse()
        {
            OID a = new OID("1.3.6.2.1.5.'hallo'.1");
            OID b = new OID("1.3.6.2.1.5.104.97.108.108.111.1");
            Assert.AreEqual(a, b);

            a = new OID("1.3.6.2.1.5.'hal.lo'.1");
            b = new OID("1.3.6.2.1.5.104.97.108.46.108.111.1");
            Assert.AreEqual(a, b);

            a = new OID("1.3.6.2.1.5.'hal.'.'''.'lo'.1");
            b = new OID("1.3.6.2.1.5.104.97.108.46.39.108.111.1");
            Assert.AreEqual(a, b);
        }

    }
}
