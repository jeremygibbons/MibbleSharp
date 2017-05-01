// <copyright file="IConnectionOrientedTransportMapping.cs" company="None">
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
    using JunoSnmp.Event;
    using JunoSnmp.SMI;

    public delegate void ConnectionStateChangeHandler(object o, TransportStateEventArgs args);

    /// <summary>
    /// Transport mappings for connection oriented transport protocols have to
    /// implement this interface.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public interface IConnectionOrientedTransportMapping<A> : ITransportMapping<A> where A : IAddress
    {
        /// <summary>
        /// Gets or sets the <see cref="MessageLengthDecoder"/> used by this
        /// transport mapping
        /// </summary>
        MessageLengthDecoder MessageLengthDecoder { get; set; }

        event ConnectionStateChangeHandler OnConnectionStateChange;
        
        /// <summary>
        /// Sets the connection timeout. This timeout specifies the time a connection
        /// may be idle before it is closed.
        /// </summary>
        /// <param name="connectionTimeout">
        /// the idle timeout in milliseconds. A zero or negative value will disable
        /// any timeout and connections opened by this transport mapping will stay
        /// opened until they are explicitly closed.
        /// </param>
        void SetConnectionTimeout(long connectionTimeout);
        
        /// <summary>
        /// Closes the connection to the given remote address (socket).
        /// </summary>
        /// <param name="remoteAddress">The address of the remote socket</param>
        /// <returns>
        /// <c>True</c> if the connection could be closed, <c>False</c> if it does not exist.</returns>
        /// <exception cref="IOException">If closing the connection failed</exception>
        bool Close(A remoteAddress);
    }
}
