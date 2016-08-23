// <copyright file="SecurityModel.cs" company="None">
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

namespace JunoSnmp.Security
{
    using JunoSnmp.ASN1;
    using JunoSnmp.SMI;
    using JunoSnmp.MP;

    /// <summary>
    /// The <c>SecurityModel</c> class as described in RFC3411 section 4.4
    /// and RFC 5590 section 5.
    /// </summary>
    public abstract class SecurityModel
    {
        public enum SecurityModelID
        {
            SECURITY_MODEL_ANY = 0,
            SECURITY_MODEL_SNMPv1 = 1,
            SECURITY_MODEL_SNMPv2c = 2,
            SECURITY_MODEL_USM = 3,
            SECURITY_MODEL_TSM = 4
        }

        /**
         * Gets the ID of the security model.
         * @return
         *    one of the integer constants defined in the {@link SecurityModel}
         *    interface.
         * @see SecurityModel#SECURITY_MODEL_ANY
         * @see SecurityModel#SECURITY_MODEL_SNMPv1
         * @see SecurityModel#SECURITY_MODEL_SNMPv2c
         * @see SecurityModel#SECURITY_MODEL_USM
         */
        public abstract SecurityModelID ID { get; }

        /**
         * Creates a new {@link SecurityParameters} instance that corresponds to this
         * security model.
         * @return
         *    a new <code>SecurityParameters</code> instance.
         */
        public abstract ISecurityParameters NewSecurityParametersInstance();

        /**
         * Creates a new {@link SecurityStateReference} instance that corresponds to
         * this security model.
         * @return
         *    a new <code>SecurityStateReference</code> instance.
         */
        public abstract ISecurityStateReference NewSecurityStateReference();

        /**
         * Generate a request message.
         * @param messageProcessingModel
         *    the ID of the message processing model (SNMP version) to use.
         * @param globalData
         *    the message header and admin data.
         * @param maxMessageSize
         *    the maximum message size of the sending (this) SNMP entity for the
         *    selected transport mapping (determined by the message processing model).
         * @param securityModel
         *    the security model for the outgoing message.
         * @param securityEngineID
         *    the authoritative SNMP entity.
         * @param securityName
         *    the principal on behalf of this message is generated.
         * @param securityLevel
         *    the requested {@link SecurityLevel}.
         * @param scopedPDU
         *    a BERInputStream containing the message (plain text) payload.
         * @param securityParameters
         *    returns the {@link SecurityParameters} filled by the security model.
         * @param wholeMsg
         *    returns the complete generated message in a <code>BEROutputStream</code>.
         *    The buffer of <code>wholeMsg</code> is set to <code>null</code> by the
         *    caller and must be set by the implementation of this method.
         * @param tmStateReference
         *    the transport model state reference as defined by RFC 5590.
         * @throws IOException
         *    if generation of the message fails because of an internal or an resource
         *    error.
         * @return
         *    the error status of the message generation. On success
         *    {@link SnmpConstants#SNMPv3_USM_OK} is returned, otherwise one of the
         *    other <code>SnmpConstants.SNMPv3_USM_*</code> values is returned.
         */
        public abstract int GenerateRequestMessage(MessageProcessingModel.MessageProcessingModels messageProcessingModel,
                                   byte[] globalData,
                                   int maxMessageSize,
                                   SecurityModelID securityModel,
                                   byte[] securityEngineID,
                                   byte[] securityName,
                                   SecurityLevel securityLevel,
                                   BERInputStream scopedPDU,
                                   // out parameters
                                   ISecurityParameters securityParameters,
                                   BEROutputStream wholeMsg,
                                   TransportStateReference tmStateReference);

        /**
         * Generates a response message.
         * @param messageProcessingModel
         *    the ID of the message processing model (SNMP version) to use.
         * @param globalData
         *    the message header and admin data.
         * @param maxMessageSize
         *    the maximum message size of the sending (this) SNMP entity for the
         *    selected transport mapping (determined by the message processing model).
         * @param securityModel
         *    the security model for the outgoing message.
         * @param securityEngineID
         *    the authoritative SNMP entity.
         * @param securityName
         *    the principal on behalf of this message is generated.
         * @param securityLevel
         *    the requested {@link SecurityLevel}.
         * @param scopedPDU
         *    a BERInputStream containing the message (plain text) payload.
         * @param securityStateReference
         *    a {@link SecurityStateReference} instance providing information from
         *    original request.
         * @param securityParameters
         *    returns the {@link SecurityParameters} filled by the security model.
         * @param wholeMsg
         *    returns the complete generated message in a <code>BEROutputStream</code>.
         *    The buffer of <code>wholeMsg</code> is set to <code>null</code> by the
         *    caller and must be set by the implementation of this method.
         * @throws IOException
         *    if generation of the message fails because of an internal or an resource
         *    error.
         * @return
         *    the error status of the message generation. On success
         *    {@link SnmpConstants#SNMPv3_USM_OK} is returned, otherwise one of the
         *    other <code>SnmpConstants.SNMPv3_USM_*</code> values is returned.
         */
        public abstract int GenerateResponseMessage(MessageProcessingModel.MessageProcessingModels messageProcessingModel,
                                    byte[] globalData,
                                    int maxMessageSize,
                                    SecurityModelID securityModel,
                                    byte[] securityEngineID,
                                    byte[] securityName,
                                    SecurityLevel securityLevel,
                                    BERInputStream scopedPDU,
                                    ISecurityStateReference securityStateReference,
                                    // out parameters
                                    ISecurityParameters securityParameters,
                                    BEROutputStream wholeMsg);

        /**
         * Processes an incoming message and returns its plaintext payload.
         * @param messageProcessingModel
         *    the ID of the message processing model (SNMP version) to use.
         * @param maxMessageSize
         *    the maximum message size of the message processing model for the
         *    transport mapping associated with this message's source address less
         *    the length of the maximum header length of the message processing model.
         *    This value is used by the security model to determine the
         *    <code>maxSizeResponseScopedPDU</code> value.
         * @param securityParameters
         *    the {@link SecurityParameters} for the received message.
         * @param securityModel
         *    the {@link SecurityModel} instance for the received message.
         * @param securityLevel
         *    the {@link SecurityLevel} ID.
         * @param wholeMsg
         *    the <code>BERInputStream</code> containing the whole message as received
         *    on the wire.
         * @param tmStateReference
         *    the transport model state reference as defined by RFC 5590.
         * @param securityEngineID
         *    the authoritative SNMP entity.
         * @param securityName
         *    the identification of the principal.
         * @param scopedPDU
         *    returns the message (plaintext) payload into the supplied
         *    <code>BEROutputStream</code>.
         *    The buffer of <code>scopedPDU</code> is set to <code>null</code> by the
         *    caller and must be set by the implementation of this method.
         * @param maxSizeResponseScopedPDU
         *    the determined maximum size for a response PDU.
         * @param securityStateReference
         *    the <code>SecurityStateReference</code> information needed for
         *    a response.
         * @param statusInfo
         *    the <code>StatusInformation</code> needed to generate reports if
         *    processing of the incoming message failed.
         * @throws IOException
         *    if an unexpected (internal) or an resource error occurred.
         * @return
         *    the error status of the message processing. On success
         *    {@link SnmpConstants#SNMPv3_USM_OK} is returned, otherwise one of the
         *    other <code>SnmpConstants.SNMPv3_USM_*</code> values is returned.
         */
        public abstract int ProcessIncomingMsg(MessageProcessingModel.MessageProcessingModels messageProcessingModel,
                               int maxMessageSize,
                               ISecurityParameters securityParameters,
                               SecurityModel.SecurityModelID securityModel,
                               SecurityLevel securityLevel,
                               BERInputStream wholeMsg,
                               TransportStateReference tmStateReference,
                               // out parameters
                               OctetString securityEngineID,
                               OctetString securityName,
                               BEROutputStream scopedPDU,
                               int maxSizeResponseScopedPDU,
                               ISecurityStateReference securityStateReference,
                               StatusInformation statusInfo);

        /**
         * Checks whether this {@link SecurityModel} supports authoritative
         * engine ID discovery.
         * The {@link USM} for instance, returns <code>true</code> whereas
         * {@link TSM} returns <code>false</code>.
         * See also RFC 5343 3.2 for details.
         * @return
         *    <code>true</code> if this security model has its own authoritative
         *    engine ID discovery mechanism.
         */
        public abstract bool SupportsEngineIdDiscovery { get; }

        /**
         * Checks whether this {@link SecurityModel} has an authoritative engine ID.
         * @return
         *    <code>true</code> if an authoritative engine ID is exchanged between
         *    command sender and responder using this security model, <code>false</code>
         *    otherwise.
         */
        public abstract bool HasAuthoritativeEngineID { get; }
    }
}
