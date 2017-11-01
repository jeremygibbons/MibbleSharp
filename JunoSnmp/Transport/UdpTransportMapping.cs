// <copyright file="UdpTransportMapping.cs" company="None">
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
    /// The <code>UdpTransportMapping</code> is the abstract base class for
    /// UDP transport mappings.
    /// </summary>
    public abstract class UdpTransportMapping : AbstractTransportMapping<UdpAddress>
    {

        protected UdpAddress udpAddress;

        protected UdpTransportMapping(UdpAddress udpAddress)
        {
            this.udpAddress = udpAddress;
        }

        public override Type SupportedAddressClass
        {
            get
            {
                return typeof(UdpAddress);
            }
        }

        /// <summary>
        /// Gets the transport address that is configured for this transport mapping for
        /// sending and receiving messages.
        /// </summary>
        public UdpAddress Address
        {
            get
            {
                return udpAddress;
            }
        }

        public override UdpAddress ListenAddress
        {
            get
            {
                return udpAddress;
            }
        }

        public abstract override Task Listen();

        public abstract override void Close();

        public abstract override void SendMessage(
            UdpAddress targetAddress, 
            byte[] message,
            TransportStateReference tmStateReference);
    }
}
