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
    public class StateReference : ISerializable, IEquatable<StateReference>
    {

        public IAddress Address { get; set; }
        public ITransportMapping<IAddress> TransportMapping { get; set; }
        public byte[] ContextEngineID { get; set; }
        public byte[] ContextName { get; set; }
        public SecurityModel.SecurityModelID SecurityModel { get; set; }
        public byte[] SecurityName { get; set; }
        public SecurityLevel SecurityLevel { get; set; }
        public ISecurityStateReference SecurityStateReference { get; set; }
        public IMessageId MsgID { get; set; }
        public int MaxSizeResponseScopedPDU { get; set;}
        public int MsgFlags { get; set; }
        public byte[] SecurityEngineID { get; set; }
        public int ErrorCode { get; set; } = 0;

        protected List<IMessageId> retryMsgIDs;

        private int matchedMsgID;
        private long responseRuntimeNanos;
        private PduHandle pduHandle;

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
            this.MsgID = CreateMessageID(msgID);
            this.MsgFlags = msgFlags;
            this.MaxSizeResponseScopedPDU = maxSizeResponseScopedPDU;
            this.PDUHandle = pduHandle;
            this.Address = peerAddress;
            this.TransportMapping = peerTransport;
            this.SecurityEngineID = secEngineID;
            this.SecurityModel = secModel;
            this.SecurityName = secName;
            this.SecurityLevel = secLevel;
            this.ContextEngineID = contextEngineID;
            this.ContextName = contextName;
            this.SecurityStateReference = secStateReference;
            this.ErrorCode = errorCode;
        }

        public bool IsReportable()
        {
            return ((this.MsgFlags & 0x04) > 0);
        }
        
        public void SetMsgID(int msgID)
        {
            this.MsgID = CreateMessageID(msgID);
        }
        
        public PduHandle PDUHandle
        {
            get
            {
                return this.pduHandle;
            }

            set
            {
                this.pduHandle = value;
                this.UpdateRequestStatisticsPduHandle(value);
            }
        }

        protected void UpdateRequestStatisticsPduHandle(PduHandle pduHandle)
        {
            if (pduHandle is IRequestStatistics rs)
            {
                rs.TotalMessagesSent = 1 + ((retryMsgIDs != null) ? retryMsgIDs.Count : 0);
                rs.ResponseRuntimeNanos = responseRuntimeNanos;
                if (MsgID.MessageId == matchedMsgID)
                {
                    rs.IndexOfMessageResponded = 0;
                }
                else if (retryMsgIDs != null)
                {
                    int index = 1;
                    foreach(IMessageId mid in retryMsgIDs)
                    {
                        if (mid.MessageId == matchedMsgID)
                        {
                            rs.IndexOfMessageResponded = index;
                            break;
                        }

                        index++;
                    }
                }
            }
        }

        protected bool IsMatchingMessageID(IMessageId msgID)
        {
            return ((this.MsgID == msgID) ||
                    ((retryMsgIDs != null) && (retryMsgIDs.Contains(msgID))));
        }

        public bool IsMatchingMessageID(int msgID)
        {
            if (this.MsgID.MessageId == msgID)
            {
                matchedMsgID = msgID;
                if (this.MsgID is TimedMessageID tmid)
                {
                    responseRuntimeNanos = DateTime.Now.Ticks - tmid.CreationNanoTime;
                }
            }
            else if (retryMsgIDs != null)
            {
                foreach (IMessageId retryMsgID in retryMsgIDs)
                {
                    if (retryMsgID.MessageId == msgID)
                    {
                        matchedMsgID = msgID;
                        if (this.MsgID is TimedMessageID tmid)
                        {
                            responseRuntimeNanos = DateTime.Now.Ticks - tmid.CreationNanoTime;
                        }

                        break;
                    }
                }
            }

            UpdateRequestStatisticsPduHandle(this.PDUHandle);
            return (matchedMsgID == msgID);
        }

        public override bool Equals(object o)
        {
            if (o is StateReference other) {
                return this.Equals(other);
            }

            return false;
        }

        public bool Equals(StateReference that)
        {
            return ((this.IsMatchingMessageID(that.MsgID)
                || ((that.retryMsgIDs != null) && (that.retryMsgIDs.Contains(this.MsgID)))) &&
                        this.EqualsExceptMsgID(that));
        }

        public bool EqualsExceptMsgID(StateReference other)
        {
            return (((this.PDUHandle == null) && (other.PDUHandle == null)) ||
                     (this.PDUHandle != null) && (this.PDUHandle.Equals(other.PDUHandle)) &&
                    (Array.Equals(SecurityEngineID, other.SecurityEngineID)) &&
                    (SecurityModel.Equals(other.SecurityModel)) &&
                    (Array.Equals(SecurityName, other.SecurityName)) &&
                    (SecurityLevel == other.SecurityLevel) &&
                    (Array.Equals(ContextEngineID, other.ContextEngineID)) &&
                    (Array.Equals(ContextName, other.ContextName)));
        }

        public override int GetHashCode()
        {
            return MsgID.MessageId;
        }

        public override string ToString()
        {
            return "StateReference[msgID=" + this.MsgID + ",pduHandle=" + this.PDUHandle +
                ",securityEngineID=" + OctetString.FromByteArray(SecurityEngineID) +
                ",securityModel=" + this.SecurityModel +
                ",securityName=" + OctetString.FromByteArray(SecurityName) +
                ",securityLevel=" + this.SecurityLevel +
                ",contextEngineID=" + OctetString.FromByteArray(ContextEngineID) +
                ",contextName=" + OctetString.FromByteArray(ContextName) +
                ",retryMsgIDs=" + retryMsgIDs + "]";
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddMessageIDs(List<IMessageId> msgIDs)
        {
            if (retryMsgIDs == null)
            {
                retryMsgIDs = new List<IMessageId>(msgIDs.Count);
            }

            retryMsgIDs.AddRange(msgIDs);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<IMessageId> GetMessageIDs()
        {
            List<IMessageId> msgIDs = new List<IMessageId>(1 + ((retryMsgIDs != null) ? retryMsgIDs.Count : 0));
            msgIDs.Add(MsgID);
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
