// <copyright file="SnmpTest.cs" company="None">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using JunoSnmp;
    using JunoSnmp.Event;
    using JunoSnmp.MP;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;
    using JunoSnmp.Transport;
    using NUnit.Framework;


    /**
 * Junit 4 test class for testing the {@link Snmp} class. The tests are run
 * against a {@link DummyTransport} which allows to directly link two virtual
 * {@link TransportMapping}s used blocking queues.
 *
 * @author Frank Fock
 * @version 2.3.2
 */
    [TestFixture]
    public class SnmpTest
    {

        private DummyTransport<UdpAddress> transportMappingCG;
        private AbstractTransportMapping<UdpAddress> transportMappingCR;
        private Snmp snmpCommandGenerator;
        private Snmp snmpCommandResponder;
        private CommunityTarget communityTarget =
            new CommunityTarget(GenericAddress.Parse("udp:127.0.0.1/161"), new OctetString("public"));
        private UserTarget userTarget =
            new UserTarget(GenericAddress.Parse("udp:127.0.0.1/161"), new OctetString("SHADES"), new byte[0]);

        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //static {
        //  JunoSnmpSettings.setForwardRuntimeExceptions(true);
        //  JunoSnmpSettings.setSnmp4jStatistics(JunoSnmpSettings.Snmp4jStatistics.extended);
        //  try {
        //    setupBeforeClass();
        //  } catch (Exception e) {
        //    e.printStackTrace();
        //  }
        //}


        public static void SetupBeforeClass()
        {
            JunoSnmpSettings.ExtensibilityEnabled = true;
            SecurityProtocols.GetInstance().AddDefaultProtocols();
            //System.setProperty(TransportMappings.TRANSPORT_MAPPINGS, "dummy-transports.properties");
            Assert.Equals(
                typeof(DummyTransport<UdpAddress>),
                typeof(TransportMappings.GetInstance().CreateTransportMapping(GenericAddress.Parse("udp:127.0.0.1/161"))));
        }


        public static void TearDownAfterClass()
        {
            SecurityProtocols.SetSecurityProtocols(null);
            //System.setProperty(TransportMappings.TRANSPORT_MAPPINGS, null);
            JunoSnmpSettings.ExtensibilityEnabled = false;
        }


        public void SetUp()
        {
            transportMappingCG = new DummyTransport<UdpAddress>(new UdpAddress("127.0.0.1/4967"));
            transportMappingCR = transportMappingCG.GetResponder(new UdpAddress("127.0.0.1/161"));
            snmpCommandGenerator = new Snmp(transportMappingCG);
            MPv3 mpv3CG = (MPv3)snmpCommandGenerator.MessageDispatcher.getMessageProcessingModel(MPv3.MPId);
            mpv3CG.LocalEngineID = MPv3.CreateLocalEngineID(new OctetString("generator"));
            mpv3CG.setCurrentMsgID(MPv3.RandomMsgID(new Random().Next(MPv3.MAX_MESSAGE_ID)));
            SecurityModels.Instance.AddSecurityModel(
                new USM(SecurityProtocols.GetInstance(), new OctetString(mpv3CG.LocalEngineID), 0));
            snmpCommandResponder = new Snmp(transportMappingCR);
            CounterSupport.Instance.AddCounterListener(new DefaultCounterListener());
            SecurityModels respSecModels = new SecurityModels()
            {

            };
            MPv3 mpv3CR = (MPv3)snmpCommandResponder.MessageDispatcher.getMessageProcessingModel(MPv3.MPId);
            mpv3CR.LocalEngineID = MPv3.CreateLocalEngineID(new OctetString("responder"));
            respSecModels.AddSecurityModel(new USM(SecurityProtocols.GetInstance(),
                                                   new OctetString(mpv3CR.LocalEngineID), 0));
            //mpv3CR.SetSecurityModels(respSecModels);
            addDefaultUsers();
        }

        private void AddDefaultUsers()
        {
            OctetString longUsername = new OctetString(new byte[32]);
            Arrays.fill(longUsername.getValue(), (byte)0x20);
            AddCommandGeneratorUsers(longUsername);
            AddCommandResponderUsers(longUsername);
        }

        private void AddCommandResponderUsers(OctetString longUsername)
        {
            snmpCommandResponder.GetUSM().AddUser(
                new UsmUser(new OctetString("SHADES"), AuthSHA.ID, new OctetString("_12345678_"),
                    PrivDES.ID, new OctetString("_0987654321_")));
            snmpCommandResponder.GetUSM().AddUser(
                new UsmUser(longUsername, AuthSHA.ID, new OctetString("_12345678_"),
                    PrivDES.ID, new OctetString("_0987654321_")));
        }

        private void AddCommandGeneratorUsers(OctetString longUsername)
        {
            snmpCommandGenerator.GetUSM().AddUser(
                new UsmUser(new OctetString("SHADES"), AuthSHA.ID, new OctetString("_12345678_"),
                    PrivDES.ID, new OctetString("_0987654321_")));
            snmpCommandGenerator.GetUSM().AddUser(
                new UsmUser(longUsername, AuthSHA.ID, new OctetString("_12345678_"),
                    PrivDES.ID, new OctetString("_0987654321_")));
        }


        public void tearDown()
        {
            snmpCommandGenerator.close();
            snmpCommandResponder.close();
        }


        public void TestSmiConstants()
        {
            int[] definedConstants = new int[] {
                SMIConstants.SyntaxInteger,
                SMIConstants.SyntaxOctetString,
                SMIConstants.SyntaxNull,
                SMIConstants.SyntaxObjectIdentifier,
                SMIConstants.SyntaxIpAddress,
                SMIConstants.SyntaxInteger32,
                SMIConstants.SyntaxCounter32,
                SMIConstants.SyntaxGauge32,
                SMIConstants.SyntaxUnsignedInteger32,
                SMIConstants.SyntaxTimeTicks,
                SMIConstants.SyntaxOpaque,
                SMIConstants.SyntaxCounter64
            };

            string[] constantNames = new string[] {
                "INTEGER",
                "OCTET_STRING",
                "NULL",
                "OBJECT_IDENTIFIER",
                "IPADDRESS",
                "INTEGER32",
                "COUNTER32",
                "GAUGE32",
                "UNSIGNED_INTEGER32",
                "TIMETICKS",
                "OPAQUE",
                "COUNTER64"
            };

            for (int i = 0; i < definedConstants.Length; i++)
            {
                System.Console.WriteLine(constantNames[i] + " = " + definedConstants[i]);
            }

            for (int i = 0; i < definedConstants.Length; i++)
            {
                System.Console.WriteLine(constantNames[i]);
            }

            for (int i = 0; i < definedConstants.Length; i++)
            {
                System.Console.WriteLine(definedConstants[i]);
            }
        }

        [TestCase]
        public void TestListen()
        {
            Assert.Equals(transportMappingCG.IsListening, false);
            snmpCommandGenerator.Listen();
            Assert.Equals(transportMappingCG.IsListening, true);
        }

        [TestCase]
        public void TestClose()
        {
            Assert.Equals(transportMappingCG.IsListening, false);
            snmpCommandGenerator.Close();
            Assert.Equals(transportMappingCG.IsListening, false);
            TestListen();
            snmpCommandGenerator.Close();
            Assert.Equals(transportMappingCG.IsListening, false);
        }

        [TestCase]
        public void TestGetV1()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version1;
            CounterListener counterListener = CreateSimpleWaitCounterListenerExtended(target);
            snmpCommandGenerator.CounterSupport.AddCounterListener(counterListener);
            PDU pdu = new PDU();
            pdu.Type = PDU.GET;
            AddTestVariableBindings(pdu, false, false, target.Version);
            syncRequestTest(target, pdu);
            snmpCommandGenerator.GetCounterSupport().removeCounterListener(counterListener);
        }

        private CounterListener CreateSimpleWaitCounterListenerExtended(ITarget target)
        {
            //            return new CounterListener()
            //            {
            //      private int status;

            //        public void incrementCounter(CounterEvent event) {
            //            switch (status++)
            //            {
            //                case 0:
            //                    Assert.Equals(SnmpConstants.snmp4jStatsRequestWaitTime, event.getOid());
            //        assertNull(event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //            break;
            //          case 1:
            //            Assert.Equals(SnmpConstants.snmp4jStatsReqTableWaitTime, event.getOid());
            //        Assert.Equals(target.getAddress(), event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //            break;
            //        }
            //    }
            //};
            return null;
        }

        [TestCase]
        public void TestGetV2c()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version2c;
            CounterListener counterListener = CreateSimpleWaitCounterListenerExtended(target);
            snmpCommandGenerator.CounterSupport.AddCounterListener(counterListener);
            PDU pdu = new PDU();
            pdu.Type = PDU.GET;
            AddTestVariableBindings(pdu, false, false, target.Version);
            syncRequestTest(target, pdu);
            snmpCommandGenerator.CounterSupport.RemoveCounterListener(counterListener);
        }

        public void TestDiscoverV3Anomaly65KUserName()
        {
            //            CounterListener counterListener = new CounterListener() {
            //      @Override
            //      public void incrementCounter(CounterEvent event)
            //{
            //            Assert.Equals(SnmpConstants.snmp4jStatsRequestWaitTime, event.getOid());
            //        Assert.Equals(communityTarget, event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //        }
            //    };
            snmpCommandGenerator.CounterSupport.AddCounterListener(counterListener);
            OctetString longUsername = new OctetString(new byte[65416]);
            Arrays.fill(longUsername.GetValue(), (byte)0x20);
            UserTarget target = (UserTarget)userTarget.Clone();
            target.SecurityName = longUsername;
            target.Timeout = 10000;
            target.Version = SnmpConstants.version3;
            ScopedPDU pdu = new ScopedPDU();
            pdu.Type = PDU.GET;
            pdu.ContextName = new OctetString("myContext");
            AddTestVariableBindings(pdu, false, false, target.Version);
            try
            {
                syncRequestTest(target, pdu);
                // fail here
                Assert.IsFalse(true);
            }
            catch (MessageException mex)
            {
                Assert.Equals(SnmpConstants.SNMPv3_USM_UNKNOWN_SECURITY_NAME, mex.getSnmp4jErrorStatus());
            }

            snmpCommandGenerator.CounterSupport.RemoveCounterListener(counterListener);
        }


        [TestCase]
        public void TestGetV3()
        {
            UserTarget target = (UserTarget)userTarget.Clone();
            target.Timeout = 10000;
            target.Version = SnmpConstants.version3;
            ScopedPDU pdu = new ScopedPDU();
            pdu.Type = PDU.GET;
            pdu.ContextName = new OctetString("myContext");
            AddTestVariableBindings(pdu, false, false, target.Version);
            syncRequestTest(target, pdu);
        }

        [TestCase]
        public void TestGetV3_RFC3414_3_2_3()
        {
            UserTarget target = (UserTarget)userTarget.Clone();
            target.Timeout = 5000;
            target.Version = SnmpConstants.version3;
            target.SecurityName = new OctetString("");
            target.AuthoritativeEngineID = (new byte[0]);
            ScopedPDU pdu = new ScopedPDU();
            pdu.Type = PDU.GET;
            CounterListener counterListener = CreateTimeoutCounterListenerExtended(target);
            snmpCommandGenerator.CounterSupport.AddCounterListener(counterListener);
            // test it
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            Map<Integer, RequestResponse> queue = new HashMap<Integer, RequestResponse>(2);
            queue.put(pdu.RequestID.GetValue(), new RequestResponse(pdu, makeResponse(pdu, target.Version)));
            TestCommandResponder responder = new TestCommandResponder(queue);
            snmpCommandResponder.AddCommandResponder(responder);
            snmpCommandGenerator.Listen();
            snmpCommandResponder.Listen();
            //            snmpCommandGenerator.setReportHandler(new Snmp.ReportHandler() {
            //      @Override
            //      public void processReport(PduHandle pduHandle, CommandResponderEvent event)
            //{
            //            PDU expectedResponse = makeReport(pdu, new VariableBinding(SnmpConstants.usmStatsUnknownEngineIDs, new Counter32(1)));
            //            // request ID will be 0 because ScopedPDU could not be parsed:
            //            expectedResponse.setRequestID(new Integer32(0));
            //            ((ScopedPDU)expectedResponse).setContextEngineID(new OctetString(snmpCommandResponder.getUSM().getLocalEngineID()));
            //            Assert.Equals(expectedResponse, event.getPDU());
            //        }
            //    });
            // first try should return local error
            try
            {
                ResponseEvent resp = snmpCommandGenerator.Send(pdu, target);
                Assert.IsNull(resp.Response);
            }
            catch (MessageException mex)
            {
                Assert.Equals(SnmpConstants.SNMPv3_USM_UNKNOWN_SECURITY_NAME, mex.getSnmp4jErrorStatus());
            }
            snmpCommandGenerator.CounterSupport.RemoveCounterListener(counterListener);
        }

        [TestCase]
        public void TestGetV3_RFC3414_3_2_4()
        {
            UserTarget target = (UserTarget)userTarget.Clone();
            target.Timeout = 5000;
            target.Version = SnmpConstants.version3;
            target.SecurityName = new OctetString("unknownSecurityName");
            ScopedPDU pdu = new ScopedPDU();
            pdu.Type = PDU.GET;
            AddTestVariableBindings(pdu, false, false, target.Version);
            //            CounterListener counterListener = new CounterListener()
            //            {
            //      private int state = 0;
            //        @Override
            //      public void incrementCounter(CounterEvent event)
            //{
            //            switch (state++)
            //            {
            //                case 0:
            //                case 1:
            //                    Assert.IsTrue(SnmpConstants.usmStatsUnknownEngineIDs.equals(event.getOid()) ||
            //                       SnmpConstants.usmStatsUnknownUserNames.equals(event.getOid()));
            //            break;
            //        case 2:
            //            Assert.Equals(SnmpConstants.snmp4jStatsRequestTimeouts, event.getOid());
            //        assertNull(event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //            break;
            //        case 3:
            //            Assert.Equals(SnmpConstants.snmp4jStatsReqTableTimeouts, event.getOid());
            //        Assert.Equals(target.getAddress(), event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //            break;
            //        }
            //    }
            //};
            snmpCommandGenerator.getCounterSupport().addCounterListener(counterListener);
            // test it
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            Map<Integer, RequestResponse> queue = new HashMap<Integer, RequestResponse>(2);
            queue.put(pdu.RequestID.GetValue(), new RequestResponse(pdu, makeResponse(pdu, target.Version)));
            TestCommandResponder responder = new TestCommandResponder(queue);
            snmpCommandResponder.AddCommandResponder(responder);
            snmpCommandGenerator.Listen();
            snmpCommandResponder.Listen();
            // first try should return local error
            try
            {
                ResponseEvent resp = snmpCommandGenerator.Send(pdu, target);
                Assert.IsNull(resp);
            }
            catch (MessageException mex)
            {
                Assert.Equals(SnmpConstants.SNMPv3_USM_UNKNOWN_SECURITY_NAME, mex.getSnmp4jErrorStatus());
            }
            // second try: remote error
            target.SecurityName = new OctetString("SHAAES");
            snmpCommandGenerator.GetUSM().AddUser(
                new UsmUser(new OctetString("SHAAES"), AuthSHA.ID, new OctetString("_12345678_"),
                    PrivAES128.ID, new OctetString("_0987654321_")));

            ResponseEvent resp = snmpCommandGenerator.send(pdu, target);
            PDU expectedResponse = makeReport(pdu, new VariableBinding(SnmpConstants.usmStatsUnknownUserNames, new Counter32(1)));
            // request ID will be 0 because ScopedPDU could not be parsed:
            expectedResponse.RequestID = new Integer32(0);
            ((ScopedPDU)expectedResponse).ContextEngineID = new OctetString(snmpCommandResponder.GetUSM().LocalEngineID);
            Assert.Equals(expectedResponse, resp.Response);
            snmpCommandGenerator.CounterSupport.RemoveCounterListener(counterListener);

        }

        [TestCase]
        public void TestUsmSeparation()
        {
            Assert.AreNotSame(snmpCommandGenerator.GetUSM(), snmpCommandResponder.GetUSM());
        }

        private CounterListener CreateTimeoutCounterListenerExtended(ITarget target)
        {
            //            return new CounterListener()
            //            {
            //      private int state = 0;
            //        @Override
            //      public void incrementCounter(CounterEvent event)
            //{
            //            switch (state++)
            //            {
            //                case 0:
            //                    Assert.Equals(SnmpConstants.usmStatsUnknownEngineIDs, event.getOid());
            //            break;
            //        case 1:
            //            Assert.Equals(SnmpConstants.snmp4jStatsRequestTimeouts, event.getOid());
            //        assertNull(event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //            break;
            //        case 2:
            //            Assert.Equals(SnmpConstants.snmp4jStatsReqTableTimeouts, event.getOid());
            //        Assert.Equals(target.getAddress(), event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //            break;
            //        }
            //    }
            //};
            return null;
        }

        [TestCase]
        public void TestGetV3_RFC3414_3_2_5()
        {
            UserTarget target = (UserTarget)userTarget.Clone();
            target.Timeout = 5000;
            target.Version = SnmpConstants.version3;
            target.SecurityLevel = SecurityLevel.AuthPriv;
            ScopedPDU pdu = new ScopedPDU();
            pdu.Type = PDU.GET;
            AddTestVariableBindings(pdu, false, false, target.Version);
            CounterListener counterListener = CreateTimeoutCounterListenerExtended(target);
            snmpCommandGenerator.CounterSupport.AddCounterListener(counterListener);
            // test it
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            Map<Integer, RequestResponse> queue = new HashMap<Integer, RequestResponse>(2);
            queue.put(pdu.getRequestID().getValue(), new RequestResponse(pdu, makeResponse(pdu, target.Version)));
            TestCommandResponder responder = new TestCommandResponder(queue);
            snmpCommandResponder.addCommandResponder(responder);
            snmpCommandGenerator.Listen();
            snmpCommandResponder.Listen();
            // first try should return local error
            target.SecurityName = new OctetString("SHAAES");
            snmpCommandGenerator.GetUSM().AddUser(
                new UsmUser(new OctetString("SHAAES"), AuthSHA.ID, new OctetString("_12345678_"), null, null));
            try
            {
                ResponseEvent resp = snmpCommandGenerator.Send(pdu, target);
                // This will be hit if engine ID discovery is enabled
                Assert.IsNull(resp.Response);
            }
            catch (MessageException mex)
            {
                // This will only happen if no engine ID discovery is needed
                Assert.Equals(SnmpConstants.SNMPv3_USM_UNSUPPORTED_SECURITY_LEVEL, mex.getSnmp4jErrorStatus());
            }
            // second try without engine ID discovery
            try
            {
                ResponseEvent resp = snmpCommandGenerator.send(pdu, target);
                // This will be hit if engine ID discovery is enabled
                Assert.IsNull(resp);
            }
            catch (MessageException mex)
            {
                // This will only happen if no engine ID discovery is needed
                Assert.Equals(SnmpConstants.SNMPv3_USM_UNSUPPORTED_SECURITY_LEVEL, mex.getSnmp4jErrorStatus());
            }
            JunoSnmpSettings.setReportSecurityLevelStrategy(JunoSnmpSettings.ReportSecurityLevelStrategy.noAuthNoPrivIfNeeded);
            // third try: remote error
            snmpCommandGenerator.GetUSM().RemoveAllUsers(new OctetString("SHAAES"));
            snmpCommandResponder.GetUSM().RemoveAllUsers(new OctetString("SHAAES"));
            snmpCommandGenerator.GetUSM().AddUser(
                new UsmUser(new OctetString("SHAAES"), AuthSHA.ID, new OctetString("_12345678_"), PrivAES128.ID,
                    new OctetString("$secure$")));
            snmpCommandResponder.getUSM().addUser(
                new UsmUser(new OctetString("SHAAES"), AuthSHA.ID, new OctetString("_12345678_"), null, null));
            target.AuthoritativeEngineID = snmpCommandResponder.LocalEngineID;
            pdu.setContextEngineID(new OctetString(snmpCommandResponder.LocalEngineID));
            ResponseEvent resp = snmpCommandGenerator.send(pdu, target);
            PDU expectedResponse =
                makeReport(pdu, new VariableBinding(SnmpConstants.usmStatsUnsupportedSecLevels, new Counter32(1)));
            // request ID will be 0 because ScopedPDU could not be parsed:
            expectedResponse.setRequestID(new Integer32(0));
            ((ScopedPDU)expectedResponse).setContextEngineID(new OctetString(snmpCommandResponder.LocalEngineID));
            Assert.Equals(expectedResponse, resp.Response);

            // Test standard behavior
            JunoSnmpSettings.setReportSecurityLevelStrategy(JunoSnmpSettings.ReportSecurityLevelStrategy.standard);
            target.AuthoritativeEngineID = snmpCommandResponder.LocalEngineID;
            pdu.setContextEngineID(new OctetString(snmpCommandResponder.LocalEngineID));
            resp = snmpCommandGenerator.send(pdu, target);
            // We expect null (timeout) as response, because sender has no matching privacy protocol to return message.
            assertNull(resp.Response);
            snmpCommandGenerator.getCounterSupport().removeCounterListener(counterListener);
        }

        [TestCase]
        public void testGetV3_RFC3414_3_2_6()
        {
            UserTarget target = (UserTarget)userTarget.clone();
            target.setTimeout(5000);
            target.setVersion(SnmpConstants.version3);
            target.setSecurityName(new OctetString("SHADES"));
            target.setSecurityLevel(SecurityLevel.AUTH_PRIV);
            ScopedPDU pdu = new ScopedPDU();
            pdu.setType(PDU.GET);
            AddTestVariableBindings(pdu, false, false, target.Version);
            // test it
            snmpCommandGenerator.getUSM().addUser(
                new UsmUser(new OctetString("SHADES"), AuthSHA.ID, new OctetString("_12345678_"),
                    PrivDES.ID, new OctetString("_09876543#1_")));

            pdu.setRequestID(new Integer32(snmpCommandGenerator.GetNextRequestID()));
            Map<Integer, RequestResponse> queue = new HashMap<Integer, RequestResponse>(2);
            queue.put(pdu.getRequestID().getValue(), new RequestResponse(pdu, makeResponse(pdu, target.Version)));
            TestCommandResponder responder = new TestCommandResponder(queue);
            snmpCommandResponder.addCommandResponder(responder);
            snmpCommandGenerator.listen();
            snmpCommandResponder.listen();

            ResponseEvent resp = snmpCommandGenerator.send(pdu, target);
            // no response because receiver cannot decode the message.
            assertNull(resp.Response);

            // next try no authentication, so with standard report strategy we will not receive a report
            snmpCommandGenerator.getUSM().removeAllUsers(new OctetString("SHADES"));
            snmpCommandGenerator.getUSM().addUser(
                new UsmUser(new OctetString("SHADES"), AuthSHA.ID, new OctetString("_12345#78_"),
                    PrivDES.ID, new OctetString("_09876543#1_")));
            target.setSecurityLevel(SecurityLevel.AUTH_NOPRIV);

            resp = snmpCommandGenerator.send(pdu, target);
            assertNull(resp.Response);

            // same but with relaxed report strategy
            JunoSnmpSettings.setReportSecurityLevelStrategy(JunoSnmpSettings.ReportSecurityLevelStrategy.noAuthNoPrivIfNeeded);
            resp = snmpCommandGenerator.send(pdu, target);
            // The usmStatsWrongDigests counter was incremented to 3 because we had already two before
            PDU expectedResponse = makeReport(pdu, new VariableBinding(SnmpConstants.usmStatsWrongDigests, new Counter32(3)));
            expectedResponse.setRequestID(new Integer32(0));
            ((ScopedPDU)expectedResponse).setContextEngineID(new OctetString(snmpCommandResponder.getUSM().getLocalEngineID()));
            Assert.Equals(expectedResponse, resp.Response);
        }

        private void syncRequestTest(Target target, PDU pdu)
        {
            pdu.setRequestID(new Integer32(snmpCommandGenerator.GetNextRequestID()));
            Map<Integer, RequestResponse> queue = new HashMap<Integer, RequestResponse>(2);
            queue.put(pdu.getRequestID().getValue(), new RequestResponse(pdu, makeResponse(pdu, target.Version)));
            TestCommandResponder responder = new TestCommandResponder(queue);
            snmpCommandResponder.addCommandResponder(responder);
            snmpCommandGenerator.listen();
            snmpCommandResponder.listen();
            ResponseEvent resp =
                snmpCommandGenerator.send(pdu, target);
            PDU expectedResponse = makeResponse(pdu, target.Version);
            Assert.Equals(expectedResponse, resp.Response);
        }

        private void asyncRequestTest(Target target, PDU pdu)
        {
            pdu.setRequestID(new Integer32(snmpCommandGenerator.GetNextRequestID()));
            Map<Integer, RequestResponse> queue = new HashMap<Integer, RequestResponse>(2);
            queue.put(pdu.getRequestID().getValue(), new RequestResponse(pdu, makeResponse(pdu, target.Version)));
            TestCommandResponder responder = new TestCommandResponder(queue);
            snmpCommandResponder.addCommandResponder(responder);
            snmpCommandGenerator.listen();
            snmpCommandResponder.listen();
            final AsyncResponseListener asyncResponseListener = new AsyncResponseListener(queue.size());
            snmpCommandGenerator.send(pdu, target, null, asyncResponseListener);
            synchronized(asyncResponseListener)
        {
                try
                {
                    asyncResponseListener.wait(20000);
                }
                catch (InterruptedException e)
                {
                    e.printStackTrace();
                }
            }
        }

        private void asyncRequestTestWithRetry(final Target target, PDU pdu, long timeout, int retries)
        {
            pdu.setRequestID(new Integer32(snmpCommandGenerator.GetNextRequestID()));
            Map<Integer, RequestResponse> queue = new HashMap<Integer, RequestResponse>(2);
            queue.put(pdu.getRequestID().getValue(), new RequestResponse(pdu, makeResponse(pdu, target.Version), retries));
            TestCommandResponder responder = new TestCommandResponder(queue);
            responder.Timeout = timeout;
            snmpCommandResponder.addCommandResponder(responder);
            snmpCommandGenerator.listen();
            snmpCommandResponder.listen();
            final AsyncResponseListener asyncResponseListener = new AsyncResponseListener(queue.size());
            snmpCommandGenerator.send(pdu, target, null, asyncResponseListener);
            synchronized(asyncResponseListener)
        {
                try
                {
                    asyncResponseListener.wait(20000);
                }
                catch (InterruptedException e)
                {
                    e.printStackTrace();
                }
            }
        }


        private void unconfirmedTest(TransportMapping transportMappingCG, Target target, PDU pdu)
        {
            Map<Integer, RequestResponse> queue = new HashMap<Integer, RequestResponse>(2);
            queue.put(pdu.getRequestID().getValue(), new RequestResponse(pdu, null));
            TestCommandResponder responder = new TestCommandResponder(queue);
            snmpCommandResponder.addCommandResponder(responder);
            snmpCommandResponder.listen();
            ResponseEvent resp =
                snmpCommandGenerator.send(pdu, target, transportMappingCG);
            assertNull(resp);
            try
            {
                Thread.sleep(500);
            }
            catch (InterruptedException iex)
            {
                // ignore
            }
            snmpCommandResponder.removeCommandResponder(responder);
            Assert.IsTrue(queue.isEmpty());
        }

        private void unconfirmedTestNullResult(Target target, PDU pdu)
        {
            Map<Integer, RequestResponse> queue = Collections.emptyMap();
            TestCommandResponder responder = new TestCommandResponder(queue);
            snmpCommandResponder.addCommandResponder(responder);
            snmpCommandResponder.listen();
            ResponseEvent resp =
                snmpCommandGenerator.send(pdu, target, transportMappingCG);
            assertNull(resp);
            try
            {
                Thread.sleep(500);
            }
            catch (InterruptedException iex)
            {
                // ignore
            }
            Assert.IsFalse(responder.isAnyResponse());
        }

        private PDU makeResponse(PDU pdu, int version)
        {
            PDU responsePDU = (PDU)pdu.clone();
            responsePDU.setType(PDU.RESPONSE);
            responsePDU.setErrorStatus(PDU.noError);
            responsePDU.setErrorIndex(0);
            responsePDU.getVariableBindings().clear();
            AddTestVariableBindings(responsePDU, true, true, version);
            return responsePDU;
        }

        private PDU makeReport(PDU pdu, VariableBinding reportVariable)
        {
            PDU responsePDU = (PDU)pdu.clone();
            responsePDU.setType(PDU.REPORT);
            responsePDU.setErrorStatus(PDU.noError);
            responsePDU.setErrorIndex(0);
            responsePDU.getVariableBindings().clear();
            responsePDU.add(reportVariable);
            return responsePDU;
        }

        private void AddTestVariableBindings(PDU pdu, bool withValue, bool withNull, int version)
        {
            pdu.Add(new VariableBinding(new OID(SnmpConstants.sysDescr), (withValue) ?
                new OctetString("Test string with öä°#+~§ and normal text.1234567890123456789012345678901234567890{}") : Null.Instance));
            pdu.Add(new VariableBinding(new OID(SnmpConstants.sysObjectID), (withValue) ? new OID("1.3.6.1.4.1.4976") : Null.Instance));

            if (version > SnmpConstants.version1)
            {
                pdu.Add(new VariableBinding(new OID("1.1"), (withValue) ? new Counter64(1234567890123456789L) : Null.Instance));
            }
            pdu.Add(new VariableBinding(new OID("1.2"), (withValue) ? new Integer32(int.MaxValue) : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.1"), (withValue) ? new UnsignedInteger32(((long)int.MinValue & 0xFFFFFF)) : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.2"), (withValue) ? new Counter32(Integer.MAX_VALUE * 2L) : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.3"), (withValue) ? new Gauge32(Integer.MAX_VALUE / 2) : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.4"), (withValue) ? new TimeTicks(12345678) : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.5"), (withValue) ? new IpAddress("127.0.0.1") : Null.Instance));
            pdu.Add(new VariableBinding(new OID("1.3.1.6.6"), (withValue) ? new Opaque(new byte[] { 0, -128, 56, 48, 0, 1 }) : Null.Instance));

            if (withNull)
            {
                pdu.Add(new VariableBinding(new OID("1.3.1.6.7"), (withValue) ? Null.NoSuchInstance : Null.Instance));
            }
        }

        [TestCase]
        [Timeout(30000)]
        public void TestGetNextV3Async()
        {
            ITarget target = userTarget;
            target.Timeout = 50000L;
            target.Retries = 0;
            Dictionary<int, RequestResponse> queue = new Dictionary<int, RequestResponse>(10000);
            for (int i = 0; i < 99; i++)
            {
                ScopedPDU pdu = new ScopedPDU();
                pdu.Add(new VariableBinding(new OID("1.3.6.1.4976.1." + i), new Integer32(i)));
                pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
                RequestResponse rr = new RequestResponse(pdu, (PDU)pdu.Clone());
                rr.response.Type = PDU.RESPONSE;
                queue[pdu.RequestID.GetValue()] = rr;
                pdu[0].Variable = Null.Instance;
            }

            TestCommandResponder responder = new TestCommandResponder(queue);
            snmpCommandResponder.AddCommandResponder(responder);
            snmpCommandGenerator.Listen();
            snmpCommandResponder.Listen();
            int n = 0;
            AsyncResponseListener asyncResponseListener = new AsyncResponseListener(queue.Count);
            List<RequestResponse> requests = new List<RequestResponse>(queue.Values);
            foreach (RequestResponse rr in requests)
            {
                snmpCommandGenerator.Send(rr.request, target, transportMappingCG, n, asyncResponseListener);
                n++;
                //      Thread.sleep(1L);
            }
            lock (asyncResponseListener)
            {
                asyncResponseListener.Wait(20000);
            }
        }

        [TestCase]
        [Timeout(30000)]
        public void TestGetNextV3AsyncUserChange()
        {
            ITarget target = userTarget;
            target.Timeout = 50000L;
            target.Retries = 0;
            Dictionary<int, RequestResponse> queue = new Dictionary<int, RequestResponse>(10000);
            for (int i = 0; i < 999; i++)
            {
                ScopedPDU pdu = new ScopedPDU();
                pdu.Add(new VariableBinding(new OID("1.3.6.1.4976.1." + i), new Integer32(i)));
                pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
                RequestResponse rr = new RequestResponse(pdu, (PDU)pdu.Clone());
                rr.response.Type = PDU.RESPONSE;
                queue[pdu.RequestID.GetValue()] = rr;
                pdu[0].Variable = Null.Instance;

            }

            TestCommandResponder responder = new TestCommandResponder(queue);
            snmpCommandResponder.AddCommandResponder(responder);
            snmpCommandGenerator.Listen();
            snmpCommandResponder.Listen();
            int n = 0;
            AsyncResponseListener asyncResponseListener = new AsyncResponseListener(queue.size());
            List<RequestResponse> requests = new ArrayList<RequestResponse>(queue.values());
            foreach (RequestResponse rr in requests)
            {
                snmpCommandGenerator.Send(rr.request, target, transportMappingCG, n, asyncResponseListener);
                n++;
                //      Thread.sleep(1L);
            }

            lock (asyncResponseListener)
            {
                snmpCommandResponder.GetUSM().RemoveAllUsers(new OctetString("SHADES2"));
                asyncResponseListener.Wait(1000);
                snmpCommandResponder.GetUSM().AddUser(
                    new UsmUser(new OctetString("SHADES2"), AuthSHA.ID, new OctetString("_12345678_"),
                        PrivDES.ID, new OctetString("_0987654321_")));
            }
        }

        [TestCase]
        public void TestTrapV1()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version1;
            PDUv1 pdu = new PDUv1();
            pdu.Type = PDU.V1TRAP;
            pdu.AgentAddress = new IpAddress("127.0.0.1");
            pdu.Enterprise = new OID("1.3.6.1.4.1.4976");
            pdu.SpecificTrap = 9;
            AddTestVariableBindings(pdu, true, false, target.Version);
            unconfirmedTest(transportMappingCG, target, pdu);
        }

        [TestCase]
        public void TestTrapV2WithV1()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version1;
            PDU pdu = new PDU();
            pdu.Type = PDU.NOTIFICATION;
            AddTestVariableBindings(pdu, true, false, target.Version);
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            unconfirmedTestNullResult(target, pdu);
        }

        [TestCase]
        public void TestTrapV2WithV1Allowed()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version1;
            PDU pdu = new PDU();
            pdu.Type = PDU.NOTIFICATION;
            AddTestVariableBindings(pdu, true, false, target.Version);
            JunoSnmpSettings.AllowSNMPv2InV1 = true;
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            unconfirmedTest(transportMappingCG, target, pdu);
        }

        [TestCase]
        public void TestNotifyV2c()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version2c;
            PDU pdu = new PDU();
            pdu.Type = PDU.NOTIFICATION;
            AddTestVariableBindings(pdu, true, false, target.Version);
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            UnconfirmedTest(transportMappingCG, target, pdu);
        }

        [TestCase]
        public void TestNotifyV3()
        {
            NotifyV3(transportMappingCG);
        }

        private void NotifyV3(ITransportMapping<IAddress> transportMappingCG)
        {
            UserTarget target = (UserTarget)userTarget.Clone();
            target.Timeout = 10000;
            target.Version = SnmpConstants.version3;
            ScopedPDU pdu = new ScopedPDU();
            pdu.Type = PDU.NOTIFICATION;
            pdu.ContextName = new OctetString("myContext");
            AddTestVariableBindings(pdu, false, false, target.Version);
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            UnconfirmedTest(transportMappingCG, target, pdu);
        }

        [TestCase]
        public void TestInformV2c()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version2c;
            PDU pdu = new PDU();
            pdu.Type = PDU.INFORM;
            AddTestVariableBindings(pdu, true, false, target.Version);
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            SyncRequestTest(target, pdu);
        }

        [TestCase]
        public void TestInformV3()
        {
            UserTarget target = (UserTarget)userTarget.Clone();
            target.Timeout = 10000;
            target.Version = SnmpConstants.version3;
            ScopedPDU pdu = new ScopedPDU();
            pdu.Type = PDU.INFORM;
            pdu.ContextName = new OctetString("myContext");
            AddTestVariableBindings(pdu, false, false, target.Version);
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            SyncRequestTest(target, pdu);
        }

        [TestCase]
        public void TestInformV2cAsync()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version2c;
            PDU pdu = new PDU();
            pdu.Type = PDU.INFORM;
            AddTestVariableBindings(pdu, true, false, target.Version);
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            asyncRequestTest(target, pdu);
        }

        [TestCase]
        public void TestInformV2cAsyncWithRetry()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version2c;
            target.Timeout = 1500;
            target.Retries = 2;
            PDU pdu = new PDU();
            pdu.Type = PDU.INFORM;
            AddTestVariableBindings(pdu, true, false, target.Version);
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            AsyncRequestTestWithRetry(target, pdu, 1000, 1);
        }

        [TestCase]
        public void TestInformV3Async()
        {
            UserTarget target = (UserTarget)userTarget.clone();
            target.Timeout = 10000;
            target.Version = SnmpConstants.version3;
            ScopedPDU pdu = new ScopedPDU();
            pdu.Type = PDU.INFORM;
            pdu.ContextName = new OctetString("myContext");
            AddTestVariableBindings(pdu, false, false, target.Version);
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            AsyncRequestTest(target, pdu);
        }

        [TestCase]
        public void TestInformV3AsyncWithRetry()
        {
            JunoSnmpSettings.JunoSnmpStatistics junoSnmpStatistics = JunoSnmpSettings.JunoSnmpStatisticsLevel;
            JunoSnmpSettings.JunoSnmpStatisticsLevel = JunoSnmpSettings.JunoSnmpStatistics.extended;
            UserTarget target = (UserTarget)userTarget.Clone();
            target.Retries = 2;
            target.Timeout = 1000;
            target.Version = SnmpConstants.version3;
            //            CounterListener counterListener = new CounterListener()
            //            {
            //      private int state = 0;
            //        @Override
            //      public void incrementCounter(CounterEvent event)
            //{
            //            switch (state++)
            //            {
            //                case 0:
            //                    Assert.Equals(SnmpConstants.usmStatsUnknownEngineIDs, event.getOid());
            //            break;
            //        case 1:
            //            Assert.Equals(SnmpConstants.snmp4jStatsRequestRetries, event.getOid());
            //        assertNull(event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //            break;
            //        case 2:
            //            Assert.Equals(SnmpConstants.snmp4jStatsReqTableRetries, event.getOid());
            //        Assert.Equals(target.getAddress(), event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //            break;
            //        case 3:
            //            Assert.Equals(SnmpConstants.snmp4jStatsRequestWaitTime, event.getOid());
            //        assertNull(event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //            break;
            //        case 4:
            //            Assert.Equals(SnmpConstants.snmp4jStatsReqTableWaitTime, event.getOid());
            //        Assert.Equals(target.getAddress(), event.getIndex());
            //        Assert.IsTrue(event.getCurrentValue().toLong() > 0);
            //            break;
            //        }
            //    }
            //};
            snmpCommandGenerator.CounterSupport.AddCounterListener(counterListener);
            ScopedPDU pdu = new ScopedPDU();
            pdu.Type = PDU.INFORM;
            pdu.ContextName = new OctetString("myContext");
            AddTestVariableBindings(pdu, false, false, target.Version);
            pdu.RequestID = new Integer32(snmpCommandGenerator.GetNextRequestID());
            AsyncRequestTestWithRetry(target, pdu, 1000, 2);
            snmpCommandGenerator.CounterSupport.RemoveCounterListener(counterListener);
            JunoSnmpSettings.JunoSnmpStatisticsLevel = junoSnmpStatistics;
        }


        [TestCase]
        public void TestSetV1()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version1;
            PDU pdu = new PDU();
            pdu.Type = PDU.SET;
            AddTestVariableBindings(pdu, true, false, target.Version);
            syncRequestTest(target, pdu);
        }

        [TestCase]
        public void TestSetV2c()
        {
            CommunityTarget target = (CommunityTarget)communityTarget.Clone();
            target.Version = SnmpConstants.version2c;
            PDU pdu = new PDU();
            pdu.Type = PDU.SET;
            AddTestVariableBindings(pdu, true, false, target.Version);
            syncRequestTest(target, pdu);
        }

        [TestCase]
        public void TestSetV3()
        {
            UserTarget target = (UserTarget)userTarget.Clone();
            target.Timeout = 10000;
            target.Version = SnmpConstants.version3;
            ScopedPDU pdu = new ScopedPDU();
            pdu.ContextName = new OctetString("myContext");
            pdu.Type = PDU.SET;
            AddTestVariableBindings(pdu, true, false, target.Version);
            syncRequestTest(target, pdu);
        }

        [TestCase]
        public void TestSend()
        {

        }

        [TestCase]
        public void TestMPv3EngineIdCache()
        {
            Snmp backupSnmp = snmpCommandGenerator;
            int backupEngineIdCacheSize = ((MPv3)snmpCommandResponder.GetMessageProcessingModel((int)MPv3.MPId)).MaxEngineIdCacheSize;
            ((MPv3)snmpCommandResponder.GetMessageProcessingModel((int)MPv3.MPId)).MaxEngineIdCacheSize = 5;
            OctetString longUsername = new OctetString(new byte[32]);
            Arrays.fill(longUsername.GetValue(), (byte)0x20);
            for (int i = 0; i < 7; i++)
            {
                System.Console.WriteLine("Testing iteration " + i);
                DummyTransport<UdpAddress> transportMappingCG = new DummyTransport<UdpAddress>(new UdpAddress("127.0.0.1/" + (i + 30000)));
                snmpCommandGenerator = new Snmp(transportMappingCG);
                ITransportMapping<UdpAddress> responderTM = transportMappingCG.GetResponder(new UdpAddress("127.0.0.1/161"));
                snmpCommandResponder.AddTransportMapping(responderTM);
                transportMappingCG.listen();
                MPv3 mpv3CG = (MPv3)snmpCommandGenerator.MessageDispatcher.GetMessageProcessingModel((int)MPv3.MPId);
                mpv3CG.LocalEngineID = MPv3.CreateLocalEngineID(new OctetString("generator"));
                SecurityModels.GetInstance().addSecurityModel(
                    new USM(SecurityProtocols.GetInstance(), new OctetString(mpv3CG.LocalEngineID), 0));
                addCommandGeneratorUsers(longUsername);
                notifyV3(transportMappingCG);
                snmpCommandResponder.RemoveTransportMapping(responderTM);
                Assert.IsTrue(((MPv3)snmpCommandResponder.GetMessageProcessingModel(MPv3.ID)).getEngineIdCacheSize() <= 5);
            }

            snmpCommandGenerator = backupSnmp;
            ((MPv3)snmpCommandResponder.GetMessageProcessingModel(MPv3.ID)).setMaxEngineIdCacheSize(backupEngineIdCacheSize);
        }

        [TestCase]
        public void TestRandomMsgID()
        {
            int engineBoots = 1;
            int randomMsgID1 = MPv3.RandomMsgID(engineBoots);
            Assert.Equals(0x00010000, randomMsgID1 & 0xFFFF0000);
            engineBoots = 0xABCDEF12;
            int randomMsgID2 = MPv3.RandomMsgID(engineBoots);
            Assert.Equals(0xEF120000, randomMsgID2 & 0xFFFF0000);
            Assert.AreNotSame(randomMsgID1 & 0xFFFF0000, randomMsgID2 & 0xFFFF0000);
        }

        class TestCommandResponder : CommandResponder
        {

            private Dictionary<int, RequestResponse> expectedPDUs;
            private bool anyResponse;
            private long timeout = 0;

            public TestCommandResponder(PDU request, PDU response)
            {
                this.expectedPDUs = new HashMap<Integer, RequestResponse>(1);
                expectedPDUs.put(request.getRequestID().getValue(), new RequestResponse(request, response));
            }

            public TestCommandResponder(Map<Integer, RequestResponse> expectedPDUs)
            {
                this.expectedPDUs = expectedPDUs;
            }


            public long Timeout
            {
                get
                {
                    return timeout;
                }

                set
                {
                    this.timeout = value;
                }
            }

            public bool IsAnyResponse()
            {
                return anyResponse;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void ProcessPdu(CommandResponderArgs evt)
            {
                anyResponse = true;
                PDU pdu = evt.PDU;
                if (expectedPDUs.Count > 0)
                {
                    Assert.IsNotNull(pdu);
                    RequestResponse expected = expectedPDUs[pdu.RequestID.GetValue()];
                    expectedPDUs.Remove(pdu.RequestID.GetValue());
                    Assert.IsNotNull(expected);
                    Assert.Equals(expected.request, pdu);

                    if (expected.retries > 0)
                    {
                        expected.retries--;
                        expectedPDUs[pdu.RequestID.GetValue()] = expected;
                    }

                    try
                    {
                        // adjust context engine ID after engine ID discovery
                        if (expected.response != null)
                        {
                            if (expected.request is ScopedPDU)
                            {
                                ScopedPDU scopedPDU = (ScopedPDU)expected.request;
                                OctetString contextEngineID = scopedPDU.ContextEngineID;
                                if ((contextEngineID != null) && (contextEngineID.Length > 0))
                                {
                                    ((ScopedPDU)expected.response).ContextEngineID = contextEngineID;
                                }
                            }

                            if (timeout > 0)
                            {
                                try
                                {
                                    Thread.sleep(timeout);
                                }
                                catch (InterruptedException e)
                                {
                                    e.printStackTrace();
                                }
                            }

                            snmpCommandResponder.MessageDispatcher.returnResponsePdu(
                                evt.MessageProcessingModel,
                                evt.SecurityModel,
                                evt.SecurityName,
                                evt.SecurityLevel,
                                expected.response,
                                evt.MaxSizeResponsePDU,
                                evt.StateReference,
                                new StatusInformation());
                        }
                    }
                    catch (MessageException e)
                    {
                        assertNull(e);
                    }
                }
                else
                {
                    assertNull(pdu);
                }
            }
        }

        class RequestResponse
        {
            public PDU request;
            public PDU response;
            public int retries;

            public RequestResponse(PDU request, PDU response)
            {
                this.request = request;
                this.response = response;
            }

            public RequestResponse(PDU request, PDU response, int retries) : this(request, response)
            {
                this.retries = retries;
            }

            public override string ToString()
            {
                return "RequestResponse{" +
                    "request=" + request +
                    ", response=" + response +
                    '}';
            }
        }

        private class AsyncResponseListener : ResponseListener
        {

            private int maxCount = 0;
            private int received = 0;
            private Set<Integer32> receivedIDs = new HashSet<Integer32>();

            public AsyncResponseListener(int maxCount)
            {
                this.maxCount = maxCount;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void onResponse(ResponseEvent evt)
            {
                ((ISession)evt.Source).Cancel(evt.Request, this);
                Assert.IsTrue(receivedIDs.add(evt.Request.RequestID));
                ++received;
                Assert.IsNotNull(evt.Response);
                Assert.IsNotNull(evt.Response[0]);
                Assert.IsNotNull(evt.Response[0].Variable);
                if (received >= maxCount)
                {
                    Notify();
                }
                Assert.IsFalse((received > maxCount));
            }
        }
    }
