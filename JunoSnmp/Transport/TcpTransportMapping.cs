// <copyright file="TcpTransportMapping.cs" company="None">
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
    using System.Threading.Tasks;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <code>TcpTransportMapping</code> is the abstract base class for
    /// TCP transport mappings.
    /// </summary>
    public abstract class TcpTransportMapping : 
        AbstractTransportMapping<TcpAddress>, 
        IConnectionOrientedTransportMapping<TcpAddress>
    {
        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected TcpAddress tcpAddress;

        public event ConnectionStateChangeHandler OnConnectionStateChange;

        public TcpTransportMapping(TcpAddress tcpAddress)
        {
            this.tcpAddress = tcpAddress;
        }

        public override Type SupportedAddressClass
        {
            get
            {
                return typeof(TcpAddress);
            }

        }

        /**
         * Returns the transport address that is used by this transport mapping for
         * sending and receiving messages.
         * @return
         *    the <code>Address</code> used by this transport mapping. The returned
         *    instance must not be modified!
         */
        public TcpAddress Address
        {
            get
            {
                return tcpAddress;
            }
        }

        public override TcpAddress ListenAddress
        {
            get
            {
                return tcpAddress;
            }
        }

        public abstract override void SendMessage(TcpAddress address, byte[] message,
                                         TransportStateReference tmStateReference);

        public abstract override Task Listen();

        public abstract override void Close();

        /**
         * Returns the <code>MessageLengthDecoder</code> used by this transport
         * mapping.
         * @return
         *    a MessageLengthDecoder instance.
         * @since 1.7
         */

        /**
         * Sets the <code>MessageLengthDecoder</code> that decodes the total
         * message length from the header of a message.
         *
         * @param messageLengthDecoder
         *    a MessageLengthDecoder instance.
         * @since 1.7
         */
        public abstract MessageLengthDecoder MessageLengthDecoder { get; set; }

        /**
         * Sets the connection timeout. This timeout specifies the time a connection
         * may be idle before it is closed.
         * @param connectionTimeout
         *    the idle timeout in milliseconds. A zero or negative value will disable
         *    any timeout and connections opened by this transport mapping will stay
         *    opened until they are explicitly closed.
         * @since 1.7
         */
        public abstract void SetConnectionTimeout(long connectionTimeout);
        public abstract bool Close(TcpAddress remoteAddress);
    }
}
