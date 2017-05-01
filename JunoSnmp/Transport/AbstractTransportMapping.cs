// <copyright file="AbstractTransportMapping.cs" company="None">
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

namespace JunoSnmp.Transport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using JunoSnmp.Event;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <code>AbstractTransportMapping</code> provides an abstract
    /// implementation for the message dispatcher list and the maximum inbound
    /// message size.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public abstract class AbstractTransportMapping<A> : ITransportMapping<A> where A : IAddress
    {

        protected List<ITransportListener> transportListener = new List<ITransportListener>(1);
        protected bool asyncMsgProcessingSupported = true;

        public event IncomingMessageHandler OnIncomingMessage;

        public abstract Type SupportedAddressClass { get; }

        public abstract void SendMessage(
            A address,
            byte[] message,
            TransportStateReference tmStateReference);

        protected void FireProcessMessage(IAddress address, MemoryStream buf,
                                          TransportStateReference tmStateReference)
        {
            IncomingMessageEventArgs imea = new IncomingMessageEventArgs(address, buf, tmStateReference);
            OnIncomingMessage(this, imea);            
        }


        public abstract void Close();
        public abstract Task Listen();
        public abstract A ListenAddress { get; }

        public virtual int MaxInboundMessageSize { get; set; } = (1 << 16) - 1;

        /// <summary>
        /// Gets or sets a value indicating whether this transport mapping has to support asynchronous
        /// message processing or not.
        /// </summary>
        /// <remarks>
        /// if <c>false</c> the {@link MessageDispatcher#processMessage(org.snmp4j.TransportMapping, org.snmp4j.smi.Address, java.nio.ByteBuffer, org.snmp4j.TransportStateReference)}
        /// method must not return before the message has been entirely processed,
        /// because the incoming message buffer is not copied before the message
        /// is being processed.If<code>true</code> the message buffer is copied
        /// for each call, so that the message processing can be implemented
        /// asynchronously.
        /// </remarks>
        public bool AsyncMsgProcessingSupported { get; set; } = true;

        public abstract bool IsListening { get; }

    }
}
