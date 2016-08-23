// <copyright file="TSM.cs" company="None">
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
    using System;
    using JunoSnmp.ASN1;
    using JunoSnmp.Event;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;

    /**
 * The <code>TSM</code> (Transport Security Model) implements a
 * {@link SecurityModel} which uses transport security mechanisms
 * as defined in RFC 5591.
 *
 * @author Frank Fock
 * @version 2.0
 * @since 2.0
 */
    public class TSM : SNMPv3SecurityModel
    {

        private static readonly int MAX_PREFIX_LENGTH = 4;
        private static readonly byte PREFIX_SEPARATOR = 0x3a;

        private static readonly log4net.ILog log = log4net.LogManager
        .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /**
         * The snmpTsmConfigurationUsePrefix flag as defined in RFC 5591.
         */
        private bool usePrefix;
        private CounterSupport counterSupport;

        public TSM() : this(new OctetString(MPv3.CreateLocalEngineID()), false)
        {
        }

        public TSM(OctetString localEngineID, bool usePrefix)
        {
            this.counterSupport = CounterSupport.Instance;
            this.localEngineID = localEngineID;
            this.usePrefix = usePrefix;
        }

        public void SetLocalEngineID(OctetString localEngineID)
        {
            this.localEngineID = localEngineID;
        }

        protected void FireIncrementCounter(CounterIncrArgs e)
        {
            counterSupport.IncrementCounter(this, e);
        }

        public override SecurityModelID ID
        {
            get
            {
                return SecurityModel.SecurityModelID.SECURITY_MODEL_TSM;
            }

        }

        public override bool SupportsEngineIdDiscovery
        {
            get
            {
                // RFC 5343 section 3.2
                return false;
            }
        }

        public override bool HasAuthoritativeEngineID
        {
            get
            {
                return false;
            }
        }

        public override ISecurityParameters NewSecurityParametersInstance()
        {
            return new TsmSecurityParameters();
        }


        public override ISecurityStateReference NewSecurityStateReference()
        {
            return new TsmSecurityStateReference();
        }

        public override int GenerateRequestMessage(MessageProcessingModel.MessageProcessingModels messageProcessingModel,
                                          byte[] globalData,
                                          int maxMessageSize,
                                          SecurityModel.SecurityModelID securityModel,
                                          byte[] securityEngineID,
                                          byte[] securityName,
                                          SecurityLevel securityLevel,
                                          BERInputStream scopedPDU,
                                          ISecurityParameters securityParameters,
                                          BEROutputStream wholeMsg,
                                          TransportStateReference tmStateReference)
        {
            return GenerateMessage(messageProcessingModel,
                globalData,
                maxMessageSize,
                securityModel,
                securityEngineID,
                securityName,
                securityLevel,
                scopedPDU,
                null,
                securityParameters,
                wholeMsg,
                tmStateReference);
        }

        public CounterSupport CounterSupport
        {
            get
            {
                return this.counterSupport;
            }
        }

        private int GenerateMessage(MessageProcessingModel.MessageProcessingModels messageProcessingModel,
                                    byte[] globalData,
                                    int maxMessageSize,
                                    SecurityModel.SecurityModelID securityModel,
                                    byte[] securityEngineID,
                                    byte[] securityName,
                                    SecurityLevel securityLevel,
                                    BERInputStream scopedPDU,
                                    ISecurityStateReference securityStateReference,
                                    ISecurityParameters securityParameters,
                                    BEROutputStream wholeMsg,
                                    TransportStateReference tmStateReference)
        {
            TransportStateReference activeTmStateReference = tmStateReference;
            TsmSecurityStateReference tsmSecurityStateReference =
        (TsmSecurityStateReference)securityStateReference;
            if ((tsmSecurityStateReference != null) &&
                (tsmSecurityStateReference.TmStateReference != null))
            {
                activeTmStateReference = tsmSecurityStateReference.TmStateReference;
                activeTmStateReference.RequestedSecurityLevel = activeTmStateReference.TransportSecurityLevel;
                // TSM uses same security for responses/reports as defined in RFC 5591 §4.2.1
                activeTmStateReference.SameSecurity = true;
            }
            else
            {
                // TSM does not use same security for requests as defined in RFC 5591 §4.2.2
                activeTmStateReference.SameSecurity = false;
                if (usePrefix)
                {
                    string prefix = GetTransportDomainPrefix(tmStateReference.Address);
                    if (prefix == null)
                    {
                        CounterIncrArgs evt = new CounterIncrArgs(SnmpConstants.snmpTsmUnknownPrefixes);
                        FireIncrementCounter(evt);
                        return SnmpConstants.SNMPv3_TSM_UNKNOWN_PREFIXES;
                    }
                    else
                    {
                        string secNamePrefix = GetSecurityNamePrefix(securityName);
                        if ((secNamePrefix == null) || (!secNamePrefix.Equals(prefix)))
                        {
                            CounterIncrArgs evt = new CounterIncrArgs(SnmpConstants.snmpTsmInvalidPrefixes);
                            FireIncrementCounter(evt);
                            return SnmpConstants.SNMPv3_TSM_UNKNOWN_PREFIXES;
                        }
                        // remove prefix and assign tmSecurityName
                        activeTmStateReference.SecurityName =
                                        new OctetString(System.Text.Encoding.UTF8.GetString(securityName).Substring(secNamePrefix.Length + 1));
                    }
                }
                else
                {
                    activeTmStateReference.SecurityName = new OctetString(securityName);
                }
            }
            // SecurityParameters already set to zero length OctetString by MPv3.
            // Build Message without authentication
            byte[]
                scopedPduBytes = BuildMessageBuffer(scopedPDU);
            byte[]
                wholeMessage =
              BuildWholeMessage(new Integer32((int)messageProcessingModel),
                                scopedPduBytes, globalData, securityParameters);

            wholeMsg.Write(wholeMessage, 0, wholeMessage.Length);
            return SnmpConstants.SNMPv3_TSM_OK;
        }

        protected string GetSecurityNamePrefix(byte[] securityName)
        {
            OctetString secName = new OctetString(securityName);
            string prefix = System.Text.Encoding.UTF8.GetString(secName.GetValue());
            int colonPos = prefix.IndexOf(':');
            if ((colonPos <= 0) || (colonPos > MAX_PREFIX_LENGTH))
            {
                return null;
            }

            return prefix.Substring(0, colonPos);
        }

        protected string GetTransportDomainPrefix(IAddress address)
        {
            return GenericAddress.GetTDomainPrefix(address.GetType());
        }

        public override int GenerateResponseMessage(MessageProcessingModel.MessageProcessingModels messageProcessingModel,
                                           byte[] globalData,
                                           int maxMessageSize,
                                           SecurityModel.SecurityModelID securityModel,
                                           byte[] securityEngineID,
                                           byte[] securityName,
                                           SecurityLevel securityLevel,
                                           BERInputStream scopedPDU,
                                           ISecurityStateReference securityStateReference,
                                           ISecurityParameters securityParameters,
                                           BEROutputStream wholeMsg)
        {
            return GenerateMessage(messageProcessingModel,
                globalData, maxMessageSize, securityModel, securityEngineID, securityName,
                securityLevel, scopedPDU, securityStateReference, securityParameters,
                wholeMsg, null);
        }


        public override int ProcessIncomingMsg(MessageProcessingModel.MessageProcessingModels messageProcessingModel,
                                      int maxMessageSize,
                                      ISecurityParameters securityParameters,
                                      SecurityModel.SecurityModelID securityModel,
                                      SecurityLevel securityLevel,
                                      BERInputStream wholeMsg,
                                      TransportStateReference tmStateReference,
                                      OctetString securityEngineID,
                                      OctetString securityName,
                                      BEROutputStream scopedPDU,
                                      int maxSizeResponseScopedPDU,
                                      ISecurityStateReference securityStateReference,
                                      StatusInformation statusInfo)
        {
            // 1. Set the securityEngineID to the local snmpEngineID.
            securityEngineID.SetValue(localEngineID.GetValue());
            // 2. Check tmStateReference
            if ((tmStateReference == null) ||
                (!tmStateReference.IsTransportSecurityValid))
            {
                CounterIncrArgs evt = new CounterIncrArgs(SnmpConstants.snmpTsmInvalidCaches);
                FireIncrementCounter(evt);
                return SnmpConstants.SNMPv3_TSM_INVALID_CACHES;
            }
            // 3. Copy the tmSecurityName to securityName.
            if (usePrefix)
            {
                string prefix =
                    GenericAddress.GetTDomainPrefix(tmStateReference.Address.GetType());
                if (prefix == null)
                {
                    CounterIncrArgs evt = new CounterIncrArgs(SnmpConstants.snmpTsmUnknownPrefixes);
                    FireIncrementCounter(evt);
                    UpdateStatusInfo(securityLevel, statusInfo, evt);
                    return SnmpConstants.SNMPv3_TSM_UNKNOWN_PREFIXES;
                }
                else if ((prefix.Length <= 0) || (prefix.Length > 4))
                {
                    CounterIncrArgs evt = new CounterIncrArgs(SnmpConstants.snmpTsmInvalidPrefixes);
                    FireIncrementCounter(evt);
                    UpdateStatusInfo(securityLevel, statusInfo, evt);
                    return SnmpConstants.SNMPv3_TSM_UNKNOWN_PREFIXES;
                }
                else
                {
                    OctetString secName = new OctetString(prefix);
                    secName.Append(PREFIX_SEPARATOR);
                    secName.Append(tmStateReference.SecurityName);
                    securityName.SetValue(secName.GetValue());
                }
            }
            else
            {
                securityName.SetValue(tmStateReference.SecurityName.GetValue());
            }

            // 4. Compare the value of tmTransportSecurityLevel:
            if (securityLevel > tmStateReference.TransportSecurityLevel)
            {
                CounterIncrArgs evt = new CounterIncrArgs(SnmpConstants.snmpTsmInadequateSecurityLevels);
                FireIncrementCounter(evt);
                UpdateStatusInfo(securityLevel, statusInfo, evt);
                return SnmpConstants.SNMPv3_TSM_INADEQUATE_SECURITY_LEVELS;
            }
          // 5. The tmStateReference is cached as cachedSecurityData:
          ((TsmSecurityStateReference)securityStateReference).TmStateReference = tmStateReference;
            // 6. The scopedPDU component is extracted from the wholeMsg.
            TsmSecurityParameters tsmSecurityParameters =
                (TsmSecurityParameters)securityParameters;
            int scopedPDUPosition = tsmSecurityParameters.ScopedPduPosition;
            byte[]
            message = BuildMessageBuffer(wholeMsg);
            int scopedPduLength = message.Length - scopedPDUPosition;
            scopedPDU.Write(message, scopedPDUPosition, scopedPduLength);
            
            // 7. The maxSizeResponseScopedPDU is calculated.
            //    Compute real max size response pdu according  to RFC3414 §3.2.9
            int maxSecParamsOverhead = tsmSecurityParameters.GetBERMaxLength((int)securityLevel);
            maxSizeResponseScopedPDU = maxMessageSize - maxSecParamsOverhead;

            // 8. The statusInformation is set to success.
            return SnmpConstants.SNMPv3_TSM_OK;
        }

        private void UpdateStatusInfo(SecurityLevel securityLevel, StatusInformation statusInfo, CounterIncrArgs evt)
        {
            if (statusInfo != null)
            {
                statusInfo.SecurityLevel = securityLevel;
                statusInfo.ErrorIndication = new VariableBinding(evt.Oid, evt.CurrentValue);
            }
        }

        /**
         * Returns whether the transport domain prefix is prepended to the securityName.
         * @return
         *    <code>true</code> if the transport domain prefix is prepended to the securityName.
         */
        public bool UsePrefix
        {
            get
            {
                return this.usePrefix;
            }

            set
            {
                this.usePrefix = value;
            }
        }

        /**
         * Sets the flag that controls whether the transport domain prefix is prepended to the securityName.
         * @param usePrefix
         *     if <code>true</code> the transport domain prefix is prepended to the securityName.
         */
    }
}
