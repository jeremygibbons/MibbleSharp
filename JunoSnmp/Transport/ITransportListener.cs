// <copyright file="ITransportListener.cs" company="None">
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
    using System.IO;
    using JunoSnmp;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <see cref="ITransportListener"/> interface is implemented by objects
    /// that process incoming messages from <see cref="ITransportMapping{A}"/>s, for
    /// example <see cref="IMessageDispatcher"/>.
    /// </summary>
    public interface ITransportListener
    {
        /// <summary>
        /// Processes an incoming message.
        /// </summary>
        /// <param name="sourceTransport">
        /// A <see cref="ITransportMapping{A}"/> instance denoing the transport that
        /// received the message and that will be used to send any responses to
        /// this message.The<code> sourceTransport</code> has to support the
        /// <code>incomingAddress</code>'s implementation class.
        /// </param>
        /// <param name="incomingAddress">The <see cref="IAddress"/> from which the message was received</param>
        /// <param name="wholeMessage">A Stream containing the received message</param>
        /// <param name="tmStateReference">The transport model state reference as defined by RFC 5590</param>
        void ProcessMessage(ITransportMapping<IAddress> sourceTransport,
                            IAddress incomingAddress,
                            Stream wholeMessage,
                            TransportStateReference tmStateReference);
    }
}
