// <copyright file="StateReference.cs" company="None">
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
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;

    /**
 * The <code>StateReference</code> class represents state information associated
 * with SNMP messages. The state reference is used to send response or report
 * (SNMPv3 only). Depending on the security model not all fields may be filled.
 *
 * @author Frank Fock
 * @version 2.0
 */
    public class StateReference : ISerializable
    {

        private IAddress address;
        private ITransportMapping<IAddress> transportMapping;
        private byte[] contextEngineID;
        private byte[] contextName;
        private SecurityModel.SecurityModelID securityModel;
        private byte[] securityName;
        private SecurityLevel securityLevel;
        private ISecurityStateReference securityStateReference;
        private IMessageId msgID;
        private int maxSizeResponseScopedPDU;
        private int msgFlags;
        private PduHandle pduHandle;
        private byte[] securityEngineID;
        private int errorCode = 0;
        protected List<IMessageId> retryMsgIDs;
        private int matchedMsgID;
        private long responseRuntimeNanos;

        /**
         * Default constructor.
         */
        public StateReference()
        {
        }

        /**
         * Creates a state reference for community based security models.
         * @param pduHandle PduHandle
         * @param peerAddress Address
         * @param peerTransport
         *    the <code>TransportMapping</code> to be used to communicate with the
         *    peer.
         * @param secModel SecurityModel
         * @param secName
         *    a community string.
         * @param errorCode
         *    an error code associated with the SNMP message.
         */
        public StateReference(PduHandle pduHandle,
                              IAddress peerAddress,
                              ITransportMapping<IAddress> peerTransport,
                              SecurityModel.SecurityModelID secModel,
                              byte[] secName,
                              int errorCode)
                : this(
                      0, 
                      0, 
                      65535, 
                      pduHandle, 
                      peerAddress, 
                      peerTransport,
                      null, 
                      secModel, 
                      secName,
                      JunoSnmp.Security.SecurityLevel.NoAuthNoPriv, 
                      null, 
                      null, 
                      null, 
                      errorCode)
        {
        }

        /**
         * Creates a state reference for SNMPv3 messages.
         * @param msgID int
         * @param msgFlags int
         * @param maxSizeResponseScopedPDU int
         * @param pduHandle PduHandle
         * @param peerAddress Address
         * @param peerTransport
         *    the <code>TransportMapping</code> to be used to communicate with the
         *    peer.
         * @param secEngineID byte[]
         * @param secModel SecurityModel
         * @param secName byte[]
         * @param secLevel int
         * @param contextEngineID byte[]
         * @param contextName byte[]
         * @param secStateReference SecurityStateReference
         * @param errorCode int
         */
        public StateReference(int msgID,
                              int msgFlags,
                              int maxSizeResponseScopedPDU,
                              PduHandle pduHandle,
                              IAddress peerAddress,
                              ITransportMapping<IAddress> peerTransport,
                              byte[] secEngineID,
                              SecurityModel.SecurityModelID secModel,
                              byte[] secName,
                              JunoSnmp.Security.SecurityLevel secLevel,
                              byte[] contextEngineID,
                              byte[] contextName,
                              ISecurityStateReference secStateReference,
                              int errorCode)
        {
            this.msgID = CreateMessageID(msgID);
            this.msgFlags = msgFlags;
            this.maxSizeResponseScopedPDU = maxSizeResponseScopedPDU;
            this.pduHandle = pduHandle;
            this.address = peerAddress;
            this.transportMapping = peerTransport;
            this.securityEngineID = secEngineID;
            this.securityModel = secModel;
            this.securityName = secName;
            this.securityLevel = secLevel;
            this.contextEngineID = contextEngineID;
            this.contextName = contextName;
            this.securityStateReference = secStateReference;
            this.errorCode = errorCode;
        }

        public bool isReportable()
        {
            return ((msgFlags & 0x04) > 0);
        }

        public IAddress Address
        {
            get
            {
                return this.address;
            }

            set
            {
                this.address = value;
            }
        }

        public byte[] ContextEngineID
        {
            get
            {
                return contextEngineID;
            }

            set
            {
                this.contextEngineID = value;
            }
        }

        public byte[] ContextName
        {
            get
            {
                return contextName;
            }

            set
            {
                this.contextName = value;
            }
        }

        public SecurityModel.SecurityModelID SecurityModel
        {
            get
            {
                return this.securityModel;
            }

            set
            {
                this.securityModel = value;
            }
        }

        public byte[] SecurityName
        {
            get
            {
                return securityName;
            }

            set
            {
                this.securityName = value;
            }
        }

        public SecurityLevel SecurityLevel
        {
            get
            {
                return securityLevel;
            }

            set
            {
                this.securityLevel = value;
            }
        }

        public ISecurityStateReference SecurityStateReference
        {
            get
            {
                return securityStateReference;
            }

            set
            {
                this.securityStateReference = value;
            }
        }

        public void SetMsgID(int msgID)
        {
            this.msgID = CreateMessageID(msgID);
        }

        public IMessageId MsgID
        {
            get
            {
                return msgID;
            }

            set
            {
                this.msgID = value;
            }
        }

        public int MsgFlags
        {
            get
            {
                return msgFlags;
            }

            set
            {
                this.msgFlags = value;
            }
        }

        public int MaxSizeResponseScopedPDU
        {
            get
            {
                return maxSizeResponseScopedPDU;
            }

            set
            {
                this.maxSizeResponseScopedPDU = value;
            }
        }


        public PduHandle PduHandle
        {
            get
            {
                return pduHandle;
            }

            set
            {
                this.pduHandle = value;
                this.UpdateRequestStatisticsPduHandle(value);
            }
        }

        public byte[] SecurityEngineID
        {
            get
            {
                return securityEngineID;
            }

            set
            {
                this.securityEngineID = value;
            }
        }


        public int ErrorCode
        {
            get
            {
                return errorCode;
            }

            set
            {
                this.errorCode = value;
            }
        }


        public ITransportMapping<IAddress> TransportMapping
        {
            get
            {
                return transportMapping;
            }

            set
            {
                this.transportMapping = value;
            }
        }

        protected void UpdateRequestStatisticsPduHandle(PduHandle pduHandle)
        {
            if (pduHandle is IRequestStatistics)
            {
                IRequestStatistics requestStatistics = (IRequestStatistics)pduHandle;
                requestStatistics.TotalMessagesSent = 1 + ((retryMsgIDs != null) ? retryMsgIDs.Count : 0);
                requestStatistics.ResponseRuntimeNanos = responseRuntimeNanos;
                if (msgID.MessageId == matchedMsgID)
                {
                    requestStatistics.IndexOfMessageResponded = 0;
                }
                else if (retryMsgIDs != null)
                {
                    int index = 1;
                    foreach(IMessageId mid in retryMsgIDs)
                    {
                        if (mid.MessageId == matchedMsgID)
                        {
                            requestStatistics.IndexOfMessageResponded = index;
                            break;
                        }
                        index++;
                    }
                }
            }
        }

        protected bool IsMatchingMessageID(IMessageId msgID)
        {
            return ((this.msgID == msgID) ||
                    ((retryMsgIDs != null) && (retryMsgIDs.Contains(msgID))));
        }

        public bool IsMatchingMessageID(int msgID)
        {
            if (this.msgID.MessageId == msgID)
            {
                matchedMsgID = msgID;
                if (this.msgID is TimedMessageID)
                {
                    responseRuntimeNanos = DateTime.Now.Ticks - ((TimedMessageID)this.msgID).CreationNanoTime;
                }
            }
            else if (retryMsgIDs != null)
            {
                foreach (IMessageId retryMsgID in retryMsgIDs)
                {
                    if (retryMsgID.MessageId == msgID)
                    {
                        matchedMsgID = msgID;
                        if (this.msgID is TimedMessageID)
                        {
                            responseRuntimeNanos = DateTime.Now.Ticks - ((TimedMessageID)this.msgID).CreationNanoTime;
                        }

                        break;
                    }
                }
            }

            UpdateRequestStatisticsPduHandle(pduHandle);
            return (matchedMsgID == msgID);
        }

        public override bool Equals(object o)
        {
            if (o is StateReference) {
                StateReference other = (StateReference)o;
                return ((IsMatchingMessageID(other.msgID) ||
                         ((other.retryMsgIDs != null) && (other.retryMsgIDs.Contains(msgID)))) &&
                        EqualsExceptMsgID(other));
            }
            return false;
        }

        public bool EqualsExceptMsgID(StateReference other)
        {
            return (((pduHandle == null) && (other.pduHandle == null)) ||
                     (pduHandle != null) && (pduHandle.Equals(other.PduHandle)) &&
                    (Array.Equals(securityEngineID, other.securityEngineID)) &&
                    (securityModel.Equals(other.securityModel)) &&
                    (Array.Equals(securityName, other.securityName)) &&
                    (securityLevel == other.securityLevel) &&
                    (Array.Equals(contextEngineID, other.contextEngineID)) &&
                    (Array.Equals(contextName, other.contextName)));
        }

        public override int GetHashCode()
        {
            return msgID.MessageId;
        }

        public override string ToString()
        {
            return "StateReference[msgID=" + msgID + ",pduHandle=" + pduHandle +
                ",securityEngineID=" + OctetString.FromByteArray(securityEngineID) +
                ",securityModel=" + securityModel +
                ",securityName=" + OctetString.FromByteArray(securityName) +
                ",securityLevel=" + securityLevel +
                ",contextEngineID=" + OctetString.FromByteArray(contextEngineID) +
                ",contextName=" + OctetString.FromByteArray(contextName) +
                ",retryMsgIDs=" + retryMsgIDs + "]";
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addMessageIDs(List<IMessageId> msgIDs)
        {
            if (retryMsgIDs == null)
            {
                retryMsgIDs = new List<IMessageId>(msgIDs.Count);
            }
            retryMsgIDs.AddRange(msgIDs);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<IMessageId> getMessageIDs()
        {
            List<IMessageId> msgIDs = new List<IMessageId>(1 + ((retryMsgIDs != null) ? retryMsgIDs.Count : 0));
            msgIDs.Add(msgID);
            if (retryMsgIDs != null)
            {
                msgIDs.AddRange(retryMsgIDs);
            }
            return msgIDs;
        }

        protected IMessageId CreateMessageID(int msgID)
        {
            if (JunoSnmpSettings.JunoSnmpStatisticsLevel == JunoSnmpSettings.JunoSnmpStatistics.extended)
            {
                return new TimedMessageID(msgID);
            }

            return new SimpleMessageID(msgID);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
