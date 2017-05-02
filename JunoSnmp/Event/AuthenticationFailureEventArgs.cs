// <copyright file="AuthenticationFailureArgs.cs" company="None">
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
    using JunoSnmp.ASN1;
    using JunoSnmp.SMI;
    
    /// <summary>
    /// The <code>AuthenticationFailureEvent</code> class describes the source
    /// and type of an authentication failure as well as the message that caused
    /// the error.
    /// </summary>
    public class AuthenticationFailureEventArgs : EventArgs
    {
        /// <summary>
        /// The source address from which the message has been received.
        /// </summary>
        public IAddress Address { get;}

        /// <summary>
        /// The transport mapping over which the message has been received.
        /// </summary>
        public ITransportMapping<IAddress> Transport { get; }

        /// <summary>
        /// The received message as a <see cref="BERInputStream"/>
        /// </summary>
        public BERInputStream Message { get; }

        /// <summary>
        /// The SNMP4J internal error status caused by the message.
        /// </summary>
        public int Error { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFailureEventArgs"/> class.
        /// </summary>
        /// <param name="sourceAddress">
        /// The address from where the failed message has been received.
        /// </param>
        /// <param name="transport">
        /// The <see cref="ITransportMapping{A}"/> with which the message has been
        /// received.
        /// </param>
        /// <param name="error">
        /// The JunoSnmp MP error status caused by the message
        /// </param>
        /// <param name="message">
        /// The message as received, at the position where processing the message stopped
        /// </param>
        public AuthenticationFailureEventArgs(IAddress sourceAddress,
                                          ITransportMapping<IAddress> transport,
                                          int error,
                                          BERInputStream message)
        {
            this.Address = sourceAddress;
            this.Transport = transport;
            this.Error = error;
            this.Message = message;
        }
    }
}
