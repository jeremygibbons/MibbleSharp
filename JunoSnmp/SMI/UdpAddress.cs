// <copyright file="UdpAddress.cs" company="None">
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

namespace JunoSnmp.SMI
{
    using System;
    using System.IO;
    using System.Net;
    using JunoSnmp.ASN1;

    /// <summary>
    /// The <c>UdpAddress</c> represents UDP/IP transport addresses.
    /// </summary>
    public class UdpAddress : TransportIpAddress
    {

        public UdpAddress()
        {
        }

        public UdpAddress(System.Net.IPAddress ipAddress, int port)
        {
            this.SetSystemNetIpAddress(ipAddress);
            this.Port = port;
        }

        public UdpAddress(int port)
        {
            this.Port = port;
        }

        public UdpAddress(string address)
        {
            if (!this.ParseAddress(address))
            {
                throw new ArgumentException(address);
            }
        }

        public new static IAddress Parse(string address)
        {
            UdpAddress a = new UdpAddress();
            if (a.ParseAddress(address))
            {
                return a;
            }

            return null;
        }

        public override bool Equals(object o)
        {
            return (o is UdpAddress) && base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
