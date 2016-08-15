// <copyright file="MessageProcessingModel.cs" company="None">
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
    using JunoSnmp.ASN1;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;

    public abstract class MessageProcessingModel
    {
        public enum MessageProcessingModels
        {
            MPv1 = 0,
            MPv2c = 1,
            MPv2u = 2,
            MPv3 = 3
        }
        
        /// <summary>
        /// Gets the numerical ID of the message processing model as defined by the
        /// constants in this interface or by an appropriate constant in the
        /// class implementing this interface.
        /// </summary>
        MessageProcessingModels MessageProcModelId { get; }

        /// <summary>
        /// Prepares an outgoing message as defined in RFC3412 §7.1.
        /// </summary>
        /// <param name="transportAddress">
        /// The destination transport <c>Address</c>.
        /// </param>
        /// <param name="maxMsgSize">
        /// the maximum message size the transport mapping for the destination address is capable of.
        /// </param>
        /// <param name="messageProcessingModel">
        /// The <see cref="MessageProcessingModels"/> ID (typically, the SNMP version).
        /// </param>
        /// <param name="securityModel">
        /// The security model ID (see <see cref="SecurityModel"/>) to use.
        /// </param>
        /// <param name="securityName">
        /// The principal on behalf the message is to be sent.
        /// </param>
        /// <param name="securityLevel">
        /// The level of security requested (see <see cref="SecurityLevel"/>).
        /// </param>
        /// <param name="pdu">
        /// the <c>PDU</c> to send. For a SNMPv1 trap <code>pdu</code> has
        /// to be a <see cref="PDUv1"/> instance, for SNMPv3 messages it has to be a
        /// <see cref="ScopedPDU"/> instance.
        /// </param>
        /// <param name="expectResponse">
        /// indicates if a message expects a response. This has to be
        /// <c>true</c> for confirmed class PDUs and<c>false</c> otherwise.
        /// </param>
        /// <param name="sendPduHandle">
        /// The <code>PduHandle</code> that uniquely identifies the sent message.
        /// </param>
        /// <param name="destTransportAddress">
        /// returns the destination transport address (currently set always set to 
        /// <c>transportAddress</c>.
        /// </param>
        /// <param name="outgoingMessage">
        /// Returns the message to send.
        /// </param>
        /// <param name="tmStateReference">
        /// the transport model state reference as defined by RFC 5590.
        /// </param>
        /// <exception cref="IOExecption">
        /// if the supplied PDU could not be encoded to the <c>outgoingMessage</c>
        /// </exception>
        /// <returns>
        /// The status of the message preparation. <see cref="SnmpConstants.SNMP_MP_OK"/>
        /// is returned if on success, otherwise any of the
        /// <c>SnmpConstants.SNMP_MP*</c> values may be returned.
        /// </returns>
        public abstract int PrepareOutgoingMessage(
            IAddress transportAddress,
            int maxMsgSize,
            MessageProcessingModels messageProcessingModel,
            SecurityModel.SecurityModels securityModel,
            byte[] securityName,
            SecurityLevel securityLevel,
            /* the following parameters are given in ScopedPDU
                  byte[] contextEngineID,
                  byte[] contextName,
            */
            PDU pdu,
            bool expectResponse,
            PduHandle sendPduHandle,
            IAddress destTransportAddress,
            BEROutputStream outgoingMessage,
            TransportStateReference tmStateReference);
        
        /// <summary>
        /// Prepares a response message as defined in RFC3412 §7.1.
        /// </summary>
        /// <param name="messageProcessingModel">
        /// The <see cref="MessageProcessingModels"/> ID (typically, the SNMP version).
        /// </param>
        /// <param name="maxMsgSize">
        /// the maximum message size the transport mapping for the destination address is capable of.
        /// </param>
        /// <param name="securityModel">
        /// The security model ID (see <see cref="SecurityModel"/>) to use.
        /// </param>
        /// <param name="securityName">
        /// The principal on behalf the message is to be sent.
        /// </param>
        /// <param name="securityLevel">
        /// The level of security requested (see <see cref="SecurityLevel"/>).
        /// </param>
        /// <param name="pdu">
        /// the <c>PDU</c> to send. For a SNMPv1 trap <code>pdu</code> has
        /// to be a <see cref="PDUv1"/> instance, for SNMPv3 messages it has to be a
        /// <see cref="ScopedPDU"/> instance.
        /// </param>
        /// <param name="maxSizeResponseScopedPDU">
        /// the maximum size of the scoped PDU the sender (of the request) can accept.
        /// </param>
        /// <param name="stateReference">
        /// Reference to state information presented with the request.
        /// </param>
        /// <param name="statusInformation">
        /// returns success or error indication. When an error occured, the error
        /// counter OID and value are included.
        /// </param>
        /// <param name="outgoingMessage">
        /// Returns the message to send.
        /// </param>
        /// <exception cref="IOExecption">
        /// if the supplied PDU could not be encoded to the <c>outgoingMessage</c>
        /// </exception>
        /// <returns>
        /// The status of the message preparation. <see cref="SnmpConstants.SNMP_MP_OK"/>
        /// is returned if on success, otherwise any of the
        /// <c>SnmpConstants.SNMP_MP*</c> values may be returned.
        /// </returns>
        public abstract int PrepareResponseMessage(
            MessageProcessingModels messageProcessingModel,
            int maxMsgSize,
            SecurityModel.SecurityModels securityModel,
            byte[] securityName,
            SecurityLevel securityLevel,
            /* the following parameters are given in ScopedPDU
                  byte[] contextEngineID,
                  byte[] contextName,
            */
            PDU pdu,
            int maxSizeResponseScopedPDU,
            StateReference stateReference,
            StatusInformation statusInformation,
            BEROutputStream outgoingMessage);
        
        /// <summary>
        /// Prepare data elements from an incoming SNMP message as described in RFC3412 §7.2.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The <see cref="IMessageDispatcher"/> instance to be used to send reports.
        /// It is typically the calling module.
        /// </param>
        /// <param name="transportAddress">The origin transport address.</param>
        /// <param name="wholeMsg">The whole message as received from the network.</param>
        /// <param name="tmStateReference">
        /// the transport model state reference as defined by RFC 5590.
        /// </param>
        /// <param name="messageProcessingModel">
        /// The <see cref="MessageProcessingModels"/> ID (typically, the SNMP version).
        /// </param>
        /// <param name="securityModel">
        /// The security model ID (see <see cref="SecurityModel"/>) to use.
        /// </param>
        /// <param name="securityName">
        /// The principal on behalf the message is to be sent.
        /// </param>
        /// <param name="securityLevel">
        /// The level of security requested (see <see cref="SecurityLevel"/>).
        /// </param>
        /// <param name="pdu">
        /// Returns SNMP protocol data unit(the payload of the received message).
        /// </param>
        /// <param name="sendPduHandle">
        /// Returns the handle to match request.
        /// </param>
        /// <param name="maxSizeResponseScopedPDU">
        /// Returns the maximum size of the scoped PDU the sender can accept.
        /// </param>
        /// <param name="statusInformation">
        /// returns success or error indication.When an error occured, the error
        /// counter OID and value are included.
        /// </param>
        /// <param name="mutableStateReference">
        /// returns the state reference to be used for a possible response.On input
        /// the stateReference may contain information about the transport mapping
        /// of the incoming request.This allows the
        /// <code>MessageProcessingModel</code> to send reports over the same
        /// transport as it received them.
        /// </param>
        /// <exception cref="IOExecption">
        /// if the supplied PDU could not be encoded to the <c>outgoingMessage</c>
        /// </exception>
        /// <returns>
        /// The status of the message preparation. <see cref="SnmpConstants.SNMP_MP_OK"/>
        /// is returned if on success, otherwise any of the
        /// <c>SnmpConstants.SNMP_MP*</c> values may be returned.
        /// </returns>
        public abstract int PrepareDataElements(
            IMessageDispatcher messageDispatcher,
            IAddress transportAddress,
            BERInputStream wholeMsg,
            TransportStateReference tmStateReference,
            MessageProcessingModels messageProcessingModel,
            SecurityModel.SecurityModels securityModel,
            OctetString securityName,
            SecurityLevel securityLevel,
            /* the following parameters are given in ScopedPDU
                  byte[] contextEngineID,
                  byte[] contextName,
            */
            MutablePDU pdu,
            PduHandle sendPduHandle,
            int maxSizeResponseScopedPDU,
            StatusInformation statusInformation,
            MutableStateReference mutableStateReference);
        
        /// <summary>
        /// Checks whether the supplied SNMP protocol version is supported by this
        /// message processing model.
        /// </summary>
        /// <returns>
        /// <c>True</c> if the supplied SNMP protocol is supported,
        /// <c>False</c> if not
        /// </returns>
        public abstract bool IsProtocolVersionSupported(int snmpProtocolVersion);
        
        /// <summary>
        /// Release the state reference associated with the supplied
        /// <c>PduHandle</c>.
        /// </summary>
        /// <param name="pduHandle">A <see cref="PduHandle"/></param>
        public abstract void ReleaseStateReference(PduHandle pduHandle);
    }
}
