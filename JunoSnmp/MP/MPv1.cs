// <copyright file="MPv1.cs" company="None">
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

namespace JunoSnmp.MP
{
    using System;
    using System.IO;
    using JunoSnmp.ASN1;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;
    using JunoSnmp.Util;

    /// <summary>
    /// The <c>MPv1</c> is the message processing model for SNMPv1.
    /// </summary>
    public class MPv1 : MessageProcessingModel
    {

        public static readonly MessageProcessingModels Model = MessageProcessingModels.MPv1;
        private static readonly log4net.ILog log = log4net.LogManager
           .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected IPDUFactory incomingPDUFactory = new PDUv1Factory();

        /// <summary>
        /// Initializes a new instance of the <see cref="MPv1"/> class. Creates a SNMPv1 message processing model 
        /// with a PDU factory for incoming messages that uses <see cref="PDUv1"/>
        /// </summary>
        public MPv1()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MPv1"/> class. Creates a SNMPv1 message processing model with a custom PDU factory that
        /// must ignore the target parameter when creating a PDU for parsing incoming
        /// messages.
        /// </summary>
        /// <param name="incomingPDUFactory">
        /// a <see cref="IPDUFactory"/>. If <c>null</c> the default factory will be
        /// used which creates <see cref="ScopedPDU"/> instances
        /// </param>
        public MPv1(IPDUFactory incomingPDUFactory)
        {
            if (incomingPDUFactory != null)
            {
                this.incomingPDUFactory = incomingPDUFactory;
            }
        }

        public override int PrepareOutgoingMessage(
            IAddress transportAddress,
            int maxMessageSize,
            MessageProcessingModels messageProcessingModel,
            SecurityModel.SecurityModelID securityModel,
            byte[] securityName,
            SecurityLevel securityLevel,
            PDU pdu,
            bool expectResponse,
            PduHandle sendPduHandle,
            IAddress destTransportAddress,
            BEROutputStream outgoingMessage, TransportStateReference tmStateReference)
        {
            if ((securityLevel != SecurityLevel.NoAuthNoPriv) ||
                (securityModel != SecurityModel.SecurityModelID.SECURITY_MODEL_SNMPv1))
            {
                log.Error("MPv1 used with unsupported security model");
                return SnmpConstants.SNMP_MP_UNSUPPORTED_SECURITY_MODEL;
            }

            if (pdu is ScopedPDU)
            {
                string txt = "ScopedPDU must not be used with MPv1";
                log.Error(txt);
                throw new ArgumentException(txt);
            }

            if (!IsProtocolVersionSupported((int)messageProcessingModel))
            {
                log.Error("MPv1 used with unsupported SNMP version");
                return SnmpConstants.SNMP_MP_UNSUPPORTED_SECURITY_MODEL;
            }


            OctetString community = new OctetString(securityName);
            Integer32 version = new Integer32((int)messageProcessingModel);
            // compute total length
            int length = pdu.BERLength;
            length += community.BERLength;
            length += version.BERLength;

            ////ByteBuffer buf = ByteBuffer.allocate(length +
            ////                                     BER.GetBERLengthOfLength(length) + 1);
            ////// set the buffer of the outgoing message
            ////outgoingMessage.setBuffer(buf);

            // encode the message
            BER.EncodeHeader(outgoingMessage, BER.SEQUENCE, length);
            version.EncodeBER(outgoingMessage);

            community.EncodeBER(outgoingMessage);
            pdu.EncodeBER(outgoingMessage);

            return SnmpConstants.SNMP_MP_OK;
        }

        public override int PrepareResponseMessage(
            MessageProcessingModels messageProcessingModel,
            int maxMessageSize,
            SecurityModel.SecurityModelID securityModel,
            byte[] securityName,
            SecurityLevel securityLevel,
            PDU pdu,
            int maxSizeResponseScopedPDU,
            StateReference stateReference,
            StatusInformation statusInformation,
            BEROutputStream outgoingMessage)
        {
            return this.PrepareOutgoingMessage(
                stateReference.Address,
                maxMessageSize,
                messageProcessingModel,
                securityModel,
                securityName,
                securityLevel,
                pdu,
                false,
                stateReference.PDUHandle,
                null,
                outgoingMessage,
                null);
        }

        public override int PrepareDataElements(
            IMessageDispatcher messageDispatcher,
            IAddress transportAddress,
            BERInputStream wholeMsg,
            TransportStateReference tmStateReference,
            MessageProcessingModels messageProcessingModel,
            SecurityModel.SecurityModelID securityModel,
            OctetString securityName,
            SecurityLevel securityLevel,
            MutablePDU pdu,
            PduHandle sendPduHandle,
            int maxSizeResponseScopedPDU,
            StatusInformation statusInformation,
            MutableStateReference mutableStateReference)
        {
            byte type;
            int length = BER.DecodeHeader(wholeMsg, out type);
            int startPos = (int)wholeMsg.Position;
            if (type != BER.SEQUENCE)
            {
                string txt = "SNMPv1 PDU must start with a SEQUENCE";
                log.Error(txt);
                throw new IOException(txt);
            }
            Integer32 version = new Integer32();
            version.DecodeBER(wholeMsg);

            securityName.DecodeBER(wholeMsg);
            securityLevel = SecurityLevel.NoAuthNoPriv;
            securityModel = SecurityModel.SecurityModelID.SECURITY_MODEL_SNMPv1;
            messageProcessingModel = MPv1.Model;

            PDU v1PDU = incomingPDUFactory.CreatePDU(this);
            pdu.Pdu = v1PDU;
            v1PDU.DecodeBER(wholeMsg);

            BER.CheckSequenceLength(length, (int)wholeMsg.Position - startPos,
                                    v1PDU);

            sendPduHandle.TransactionID = v1PDU.RequestID.GetValue();

            // create state reference
            StateReference stateRef =
                new StateReference(sendPduHandle,
                                   transportAddress,
                                   null,
                                   securityModel,
                                   securityName.GetValue(),
                                   SnmpConstants.SNMP_ERROR_SUCCESS);
            mutableStateReference.StateReference = stateRef;

            return SnmpConstants.SNMP_MP_OK;
        }

        public override bool IsProtocolVersionSupported(int snmpProtocolVersion)
        {
            return (snmpProtocolVersion == SnmpConstants.version1);
        }

        public override void ReleaseStateReference(PduHandle pduHandle)
        {
            // we do not cache state information -> do nothing
        }

        private class PDUv1Factory : IPDUFactory
        {
            public PDU CreatePDU(ITarget target)
            {
                return new PDUv1();
            }
            public PDU CreatePDU(MessageProcessingModel messageProcessingModel)
            {
                return new PDUv1();
            }
        }
    }
}
