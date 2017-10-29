// <copyright file="TestSnmp.cs" company="None">
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
    using System;
    using JunoSnmp.MP;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;
    using JunoSnmp.Transport;
    using NUnit.Framework;

    [TestFixture]
    class TestSnmp
    {
        [TestCase]
        public void TestRandomMsgID()
        {
            int engineBoots = 1;
            int randomMsgID1 = MPv3.RandomMsgID(engineBoots);
            Assert.AreEqual(0x00010000, randomMsgID1 & 0xFFFF0000);
            unchecked
            {
                engineBoots = (int)0xABCDEF12;
            }
            int randomMsgID2 = MPv3.RandomMsgID(engineBoots);
            Assert.AreEqual(0xEF120000, randomMsgID2 & 0xFFFF0000);
            Assert.AreNotEqual(randomMsgID1 & 0xFFFF0000, randomMsgID2 & 0xFFFF0000);
        }

        private PDU MakeResponse(PDU pdu, int version)
        {
            PDU responsePDU = (PDU)pdu.Clone();
            responsePDU.Type = PDU.RESPONSE;
            responsePDU.ErrorStatus = PDU.noError;
            responsePDU.ErrorIndex = 0;
            responsePDU.VariableBindings.Clear();
            AddTestVariableBindings(responsePDU, true, true, version);
            return responsePDU;
        }

        private PDU MakeReport(PDU pdu, VariableBinding reportVariable)
        {
            PDU responsePDU = (PDU)pdu.Clone();
            responsePDU.Type = PDU.REPORT;
            responsePDU.ErrorStatus = PDU.noError;
            responsePDU.ErrorIndex = 0;
            responsePDU.VariableBindings.Clear();
            responsePDU.Add(reportVariable);
            return responsePDU;
        }

        private void AddTestVariableBindings(PDU pdu, bool withValue, bool withNull, int version)
        {
            pdu.Add(new VariableBinding(new OID(SnmpConstants.sysDescr), (withValue) ?
               (IVariable) new OctetString("Test string with öä°#+~§ and normal text.1234567890123456789012345678901234567890{}") : Null.Instance));
            pdu.Add(new VariableBinding(new OID(SnmpConstants.sysObjectID), (withValue) ? (IVariable) new OID("1.3.6.1.4.1.4976") : Null.Instance));

            if (version > SnmpConstants.version1)
            {
                pdu.Add(new VariableBinding(new OID("1.1"), (withValue) ? (IVariable) new Counter64(1234567890123456789L) : Null.Instance));
            }

            pdu.Add(new VariableBinding(new OID("1.2"), (withValue) ? (IVariable) new Integer32(int.MaxValue) : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.1"), (withValue) ? (IVariable)new UnsignedInteger32(((long)int.MinValue & 0xFFFFFF)) : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.2"), (withValue) ? (IVariable)new Counter32(int.MaxValue * 2L) : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.3"), (withValue) ? (IVariable)new Gauge32(int.MaxValue / 2) : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.4"), (withValue) ? (IVariable)new TimeTicks(12345678) : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.5"), (withValue) ? (IVariable) new IpAddress("127.0.0.1") : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.6"), (withValue) ? (IVariable) new Opaque(new byte[] { 0, 255, 56, 48, 0, 1 }) : Null.Instance));

            if (withNull)
            {
                pdu.Add(new VariableBinding(new OID("1.3.1.6.7"), (withValue) ? Null.NoSuchInstance : Null.Instance));
            }
        }

        class RequestResponse
        {
            public PDU Request { get; set; }
            public PDU Response { get; set; }
            public int Retries { get; set; }

            public RequestResponse(PDU request, PDU response)
            {
                this.Request = request;
                this.Response = response;
            }

            public RequestResponse(PDU request, PDU response, int retries) : this(request, response)
            {
                this.Retries = retries;
            }

            public override string ToString()
            {
                return "RequestResponse{" +
                    "request=" + this.Request +
                    ", response=" + this.Response +
                    '}';
            }
        }
    }
}
