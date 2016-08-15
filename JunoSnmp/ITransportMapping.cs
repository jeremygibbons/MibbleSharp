// <copyright file="ITransportMapping.cs" company="None">
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
//    Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp
{
    using System;
    using JunoSnmp.SMI;
    using JunoSnmp.Transport;

    /// <summary>
    /// The <see cref="ITransportMapping{A}"/> defines the common interface for SNMP
    /// transport mappings. A transport mapping can only support a single
    /// transport protocol.
    /// </summary>
    /// <typeparam name="A">The address type supported by this transport mapping</typeparam>
    public interface ITransportMapping<A> where A : IAddress 
    {   
        /// <summary>
        /// Gets a value indicating whether the transport mapping is listening for
        /// incoming messages.For connection oriented transport mappings this
        /// is a prerequisite to be able to send SNMP messages. For connectionless
        /// transport mappings it is a prerequisite to be able to receive responses.
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// Gets the maximum length of an incoming message that can be successfully
        /// processed by this transport mapping implementation.
        /// </summary>
        int MaxInboundMessageSize { get; }

        /// <summary>
        /// Gets the <c>IAddress</c> class that is supported by this transport mapping.
        /// </summary>
        /// <returns>A type implementing IAddress</returns>
        Type GetSupportedAddressClass();

        /// <summary>
        /// Returns the address that represents the actual incoming address this transport
        /// mapping uses to listen for incoming packets.
        /// </summary>
        /// <returns>
        /// The address for incoming packets or <code>null</code> this transport
        /// mapping is not configured to listen for incoming packets.
        /// </returns>
        A GetListenAddress();

        /// <summary>
        /// Sends a message to the supplied address using this transport.
        /// </summary>
        /// <param name="address">An IAddress instance denoting the target address</param>
        /// <param name="message">The whole message as an array of bytes</param>
        /// <param name="transStateReference">
        /// The (optional) transport model state reference as defined by RFC 5590 section 6.1.
        /// </param>
        /// <exception cref="IOException">If an underlying IO operation fails</exception>
        void SendMessage(A address, byte[] message, TransportStateReference transStateReference);

        /// <summary>
        /// Adds a transport listener to the transport. Normally, at least one
        /// transport listener needs to be added to process incoming messages.
        /// </summary>
        /// <param name="transportListener">A <see cref="ITransportListener"/> instance</param>
        void AddTransportListener(ITransportListener transportListener);

        /// <summary>
        /// Removes a transport listener. Incoming messages will no longer be
        /// propagated to the supplied<code>ITransportListener</code>.
        /// </summary>
        /// <param name="transportListener">A <see cref="ITransportListener"/> instance</param>
        void RemoveTransportListener(ITransportListener transportListener);

        /// <summary>
        /// Closes the transport an releases all bound resources synchronously.
        /// </summary>
        void Close();

        /// <summary>
        /// Listen for incoming messages. For connection oriented transports, this
        /// method needs to be called before <see cref="SendMessage(A, byte[], TransportStateReference)"/> 
        /// is called for the first time.
        /// </summary>
        void Listen();
    }
}
