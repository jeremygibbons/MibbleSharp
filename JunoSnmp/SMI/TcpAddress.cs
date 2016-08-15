// <copyright file="TcpAddress.cs" company="None">
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

    /// <summary>
    /// The <c>TcpAddress</c> represents TCP/IP transport addresses.
    /// </summary>
    public class TcpAddress : TransportIpAddress
    {

        private static readonly log4net.ILog log = log4net.LogManager
           .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpAddress"/> class.
        /// </summary>
        public TcpAddress() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpAddress"/> class with a given IP and port
        /// </summary>
        /// <param name="ipAddress">The IP Address for this address object</param>
        /// <param name="port">The TCP port number</param>
        public TcpAddress(System.Net.IPAddress ipAddress, int port)
        {
            this.SetSystemNetIpAddress(ipAddress);
            this.Port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpAddress"/> class with a given port
        /// </summary>
        /// <param name="port">The TCP port number</param>
        public TcpAddress(int port) : base()
        {
            this.Port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpAddress"/> class from a string
        /// </summary>
        /// <param name="address">A string containing an address</param>
        public TcpAddress(string address)
        {
            if (!this.ParseAddress(address))
            {
                throw new ArgumentException(address);
            }
        }

        /// <summary>
        /// Parses the address from the supplied string representation.
        /// </summary>
        /// <param name="address">A string representation of this address</param>
        /// <returns>
        /// True if the address could be successfully parsed and assigned to this address object,
        /// False if not.
        /// </returns>
        public new static IAddress Parse(string address)
        {
            try
            {
                TcpAddress a = new TcpAddress();
                if (a.ParseAddress(address))
                {
                    return a;
                }
            }
            catch (Exception ex)
            {
                logger.error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets hashcode for this object
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Tests this object for equality with another object
        /// </summary>
        /// <param name="o">The other object</param>
        /// <returns>
        /// True if both are of type <see cref="TcpAddress"/> and have the same address and port values
        /// </returns>
        public override bool Equals(object o)
        {
            return (o is TcpAddress) && base.Equals(o);
        }
    }
}
