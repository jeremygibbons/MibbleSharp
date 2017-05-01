// <copyright file="CommandResponderArgs.cs" company="None">
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

namespace JunoSnmp.Event
{
    using System;
    using JunoSnmp.MP;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;

    public class CommandResponderArgs : EventArgs
    {
        private IMessageDispatcher messageDispatcher;
        private int securityModel;
        private SecurityLevel securityLevel;
        private int maxSizeResponsePDU;
        private PduHandle pduHandle;
        private StateReference stateReference;
        private PDU pdu;
        private int messageProcessingModel;
        private byte[] securityName;
        private bool processed;
        private IAddress peerAddress;
        private ITransportMapping<IAddress> transportMapping;
        private TransportStateReference tmStateReference;

        /**
         * Constructs an event for processing an incoming request or notification PDU.
         * @param messageDispatcher
         *    the source of the event. May be used to send response PDUs.
         * @param transportMapping
         *    the <code>TransportMapping</code> which received the PDU.
         * @param sourceAddress
         *    the source transport address of the SNMP message.
         * @param messageProcessingModel
         *    the message processing model ID.
         * @param securityModel
         *    the security model ID.
         * @param securityName
         *    the principal.
         * @param securityLevel
         *    the requested security level.
         * @param pduHandle
         *    the PDU handle that uniquely identifies the <code>pdu</code>.
         * @param pdu
         *    the SNMP request PDU to process.
         * @param maxSizeResponseScopedPDU
         *    the maximum size of a possible response PDU.
         * @param stateReference
         *    needed for responding a request, will be <code>null</code> for
         *    notifications.
         */
        public CommandResponderArgs(IMessageDispatcher messageDispatcher,
                                     ITransportMapping<IAddress> transportMapping,
                                     IAddress sourceAddress,
                                     int messageProcessingModel,
                                     int securityModel,
                                     byte[] securityName,
                                     SecurityLevel securityLevel,
                                     PduHandle pduHandle,
                                     PDU pdu,
                                     int maxSizeResponseScopedPDU,
                                     StateReference stateReference)
            : base()
        {
            this.MessageDispatcher  = messageDispatcher;
            this.TransportMapping = transportMapping;
            this.MessageProcessingModel = messageProcessingModel;
            this.SecurityModel = securityModel;
            this.SecurityName = securityName;
            this.SecurityLevel = securityLevel;
            this.PduHandle = pduHandle;
            this.PDU = pdu;
            this.MaxSizeResponsePDU = maxSizeResponseScopedPDU;
            this.StateReference = stateReference;
            this.PeerAddress = sourceAddress;
        }

        /**
         * Creates shallow copy of the supplied <code>CommandResponderEvent</code>
         * but the source of the event is set to the supplied source.
         *
         * @param source
         *    the (new) source of event copy to create.
         * @param other
         *    the <code>CommandResponderEvent</code> to copy.
         * @since 1.1
         */
        public CommandResponderArgs(CommandResponderArgs other)
            : base()
        {
            this.TransportMapping = other.transportMapping;
            this.MessageProcessingModel = other.messageProcessingModel;
            this.SecurityModel = other.securityModel;
            this.SecurityName = other.securityName;
            this.SecurityLevel = other.securityLevel;
            this.PduHandle = other.pduHandle;
            this.PDU = other.pdu;
            this.MaxSizeResponsePDU = other.maxSizeResponsePDU;
            this.StateReference = other.stateReference;
            this.PeerAddress = other.PeerAddress;
        }

        /**
         * Gets the message dispatcher instance that received the command
         * (request PDU) or unconfirmed PDU like a report, trap, or notification..
         * @return
         *    the <code>MessageDispatcher</code> instance that received the command.
         */
        public IMessageDispatcher MessageDispatcher
        {
            get
            {
                return this.messageDispatcher;
            }

            set
            {
                this.messageDispatcher = value;
            }
        }

        /**
         * Gets the security model used by the command.
         * @return int
         */
        public int SecurityModel
        {
            get
            {
                return securityModel;
            }

            set
            {
                this.securityModel = value;
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

        public int MaxSizeResponsePDU
        {
            get
            {
                return maxSizeResponsePDU;
            }

            set
            {
                this.maxSizeResponsePDU = value;
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
            }
        }

        public StateReference StateReference
        {
            get
            {
                return stateReference;
            }

            set
            {
                this.stateReference = value;
            }
        }

        public PDU PDU
        {
            get
            {
                return pdu;
            }

            set
            {
                this.pdu = value;
            }
        }

        public int MessageProcessingModel
        {
            get
            {
                return messageProcessingModel;
            }

            set
            {
                this.messageProcessingModel = value;
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

        /**
         * Checks whether this event is already processed or not.
         * @return
         *    <code>true</code> if this event has been processed, <code>false</code>
         *    otherwise.
         */

        /**
         * Sets the status of this PDU.
         * @param processed
         *    If set to <code>true</code>, the dispatcher stops dispatching this
         *    event to other event listeners, because it has been successfully
         *    processed.
         */
        public bool Processed
        {
            get
            {
                return processed;
            }

            set
            {
                this.processed = value;
            }
        }

        /**
         * Gets the transport address of the sending entity.
         * @return
         *    the <code>Address</code> of the PDU sender.
         */

        /**
         * Sets the transport address of the sending entity.
         * @param peerAddress
         *    the <code>Address</code> of the PDU sender.
         */
        public IAddress PeerAddress
        {
            get
            {
                return peerAddress;
            }

            set
            {
                this.peerAddress = value;
            }
        }

        /**
         * Returns the transport mapping that received the PDU that triggered this
         * event.
         * @return
         *   a <code>TransportMapping</code> instance.
         */

        public ITransportMapping<IAddress> TransportMapping
        {
            get
            {
                return transportMapping;
            }

            protected set
            {
                this.transportMapping = value;
            }
        }

        /**
         * Gets the transport model state reference as defined by RFC 5590.
         * @return
         *    a {@link TransportStateReference} instance if the transport and/or
         *    the security model supports it or <code>null</code> otherwise.
         * @since 2.0
         */

        /**
         * Sets the transport model state reference as defined by RFC 5590.
         * @param tmStateReference
         *    the transport model (mapping) state information associated with
         *    this command responder event.
         */
        public TransportStateReference TmStateReference
        {
            get
            {
                return tmStateReference;
            }

            set
            {
                this.tmStateReference = value;
            }
        }

        public override string ToString()
        {
            return "CommandResponderEvent[" +
                "securityModel=" + securityModel +
                ", securityLevel=" + securityLevel +
                ", maxSizeResponsePDU=" + maxSizeResponsePDU +
                ", pduHandle=" + pduHandle +
                ", stateReference=" + stateReference +
                ", pdu=" + pdu +
                ", messageProcessingModel=" + messageProcessingModel +
                ", securityName=" + new OctetString(securityName) +
                ", processed=" + processed +
                ", peerAddress=" + peerAddress +
                ", transportMapping=" + transportMapping +
                ", tmStateReference=" + tmStateReference +
                ']';
        }
    }
}
