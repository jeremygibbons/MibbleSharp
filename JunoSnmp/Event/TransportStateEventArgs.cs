// <copyright file="TransportStateEventArgs.cs" company="None">
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
    using System.Collections.Generic;
    using System.IO;
    using JunoSnmp;
    using JunoSnmp.SMI;

    public class TransportStateEventArgs : EventArgs
    {
        public static readonly int STATE_UNKNOWN = 0;
        public static readonly int STATE_CONNECTED = 1;
        public static readonly int STATE_DISCONNECTED_REMOTELY = 2;
        public static readonly int STATE_DISCONNECTED_TIMEOUT = 3;
        public static readonly int STATE_CLOSED = 4;

        private int newState;
        private IAddress peerAddress;
        private IOException causingException;
        private List<byte[]> discardedMessages;
        ITransportMapping<IAddress> source;

        private bool cancelled = false;

        public TransportStateEventArgs(ITransportMapping<IAddress> source,
                                   IAddress peerAddress,
                                   int newState,
                                   IOException causingException) : base()
        {
            this.newState = newState;
            this.peerAddress = peerAddress;
            this.causingException = causingException;
            this.source = source;
        }

        //public TransportStateEventArgs(TcpTransportMapping source,
        //                           IAddress peerAddress,
        //                           int newState,
        //                           IOException causingException,
        //                           List<byte[]> discardedMessages)
        //    : this(source, peerAddress, newState, causingException)
        //{
        //    this.discardedMessages = new List<byte[]>(discardedMessages);
        //}

        public IOException CausingException
        {
            get
            {
                return this.causingException;
            }
        }

        public int NewState
        {
            get
            {
                return this.newState;
            }
        }

        public IAddress PeerAddress
        {
            get
            {
                return this.peerAddress;
            }
        }

        /**
         * Gets the messages that were discarded due to a state change of the transport connection.
         * @return
         *    a (possibly empty) list of messages that were discarded or <code>null</code> if the event has not terminated
         *    the transport connection.
         * @since 2.4.0
         */
        public List<byte[]> DiscardedMessages
        {
            get
            {
                return this.discardedMessages;
            }
        }

        /**
         * Indicates whether this event has been canceled. Only
         * {@link #STATE_CONNECTED} events can be canceled.
         * @return
         *    <code>true</code> if the event has been canceled.
         * @since 1.8
         */
        public bool Cancelled
        {
            get
            {
                return this.cancelled;
            }

            set
            {
                this.cancelled = value;
            }
        }

        public override string ToString()
        {
            return nameof(TransportStateEventArgs) +
        ",peerAddress=" + this.peerAddress +
        ",newState=" + this.newState +
        ",cancelled=" + this.cancelled +
        ",causingException=" + this.causingException + "]";
        }
    }
}
