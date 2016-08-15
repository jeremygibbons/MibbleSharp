// <copyright file="TransportIpAddress.cs" company="None">
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
    /// The <c>TransportIpAddress</c> is the abstract base class for all
    /// transport addresses on top of IP network addresses.
    /// </summary>
    public abstract class TransportIpAddress : IpAddress
    {
        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected int port = 0;

        public int Port
        {
            get
            {
                return this.port;
            }

            set
            {
                if ((value < 0) || (value > 65535))
                {
                    throw new ArgumentException("Illegal port specified: " + value);
                }

                this.port = value;
            }

        }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && (this.port >= 0) && (this.port <= 65535);
            }
        }

        public override int CompareTo(IVariable o)
        {
            int result = base.CompareTo(o);
            if (result == 0)
            {
                return (port - ((TransportIpAddress)o).Port);
            }

            return result;
        }

        public override bool Equals(object o)
        {
            return (o is TransportIpAddress) && (base.Equals(o) && ((TransportIpAddress)o).Port == this.port);
        }

        public override bool ParseAddress(string address)
        {
            try
            {
                string[] tokens = address.Split('/');

                string addr = tokens[0];
                string portnum = tokens[1];

                if (base.ParseAddress(addr))
                {
                    this.port = int.Parse(portnum);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
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

        public override string ToString()
        {
            return base.ToString() + "/" + port;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ 2 + port;
        }

        /**
         * Sets this transport address from an OcetString containing the address
         * value in format as specified by the TRANSPORT-ADDRESS-MIB.
         * @param transportAddress
         *    an OctetString containing the IP address bytes and the two port bytes
         *    in network byte order.
         * @throws UnknownHostException
         *    if the address is invalid.
         */
        public void SetTransportAddress(OctetString transportAddress)
        {
            OctetString inetAddr = transportAddress.Substring(0, transportAddress.Length - 2);
            byte[] addr = inetAddr.GetValue();
            if ((addr.Length == 8) || (addr.Length == 20))
            {
                // address with zone (scope) index
                byte[] ipaddr = new byte[addr.Length - 4];
                System.Array.Copy(addr, 0, ipaddr, 0, ipaddr.Length);
                int sz = ipaddr.Length;
                int scope = ((addr[sz] << 24) +
                             ((addr[sz + 1] & 0xff) << 16) +
                             ((addr[sz + 2] & 0xFF) << 8) +
                             (addr[sz + 3] & 0xFF));
                try
                {
                    this.SetSystemNetIpAddress(new IPAddress(ipaddr, scope));
                }
                catch (Exception ex)
                {
                    log.Warn("Problem setting scope for IPv6 address, " +
                                "ignoring scope ID for " + transportAddress + ": " + ex.Message);
                    this.SetSystemNetIpAddress(new System.Net.IPAddress(ipaddr));
                }
            }
            else
            {
                this.SetSystemNetIpAddress(new System.Net.IPAddress(addr));
            }

            port = ((transportAddress[transportAddress.Length - 2] & 0xFF) << 8);
            port += (transportAddress[transportAddress.Length - 1] & 0xFF);
        }

        /**
         * Returns the address value as a byte array.
         * @return
         *    a byte array with IP address bytes and two additional bytes containing
         *    the port in network byte order. If the address is a zoned (scoped) IP
         *    address, four additional bytes with the scope ID are returned between
         *    address and port bytes.
         * @since 1.5
         */
        public byte[] GetValue()
        {
            byte[] addr = this.GetSystemNetIpAddress().GetAddressBytes();
            int scopeSize = 0;
            long scopeID = 0;
            if (this.GetSystemNetIpAddress().AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                scopeID = this.GetSystemNetIpAddress().ScopeId;
                scopeSize = 4;
            }

            byte[] retval = new byte[addr.Length + 2 + scopeSize];
            System.Array.Copy(addr, 0, retval, 0, addr.Length);
            int offset = addr.Length;
            if (scopeSize > 0)
            {
                retval[offset++] = (byte)((scopeID & 0xFF000000) >> 24);
                retval[offset++] = (byte)((scopeID & 0x00FF0000) >> 16);
                retval[offset++] = (byte)((scopeID & 0x0000FF00) >> 8);
                retval[offset++] = (byte)(scopeID & 0x000000FF);
            }

            retval[offset++] = (byte)((port >> 8) & 0xFF);
            retval[offset] = (byte)(port & 0xFF);

            return retval;
        }

        public override void DecodeBER(BERInputStream inputStream)

        {
            OctetString os = new OctetString();
            os.DecodeBER(inputStream);
            try
            {
                this.SetTransportAddress(os);
            }
            catch (Exception ex)
            {
                string txt = "Wrong encoding of TransportAddress";
                log.Error(txt);
                throw new IOException(txt + ": " + ex.Message);
            }
        }

        public override void EncodeBER(Stream outputStream)
        {
            OctetString os = new OctetString(this.GetValue());
            os.EncodeBER(outputStream);
        }

        public override int BERLength
        {
            get
            {
                return this.GetValue().Length;
            }
        }

        public override int BERPayloadLength
        {
            get
            {
                return this.BERLength;
            }
        }

        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxOctetString;
            }
        }
    }

}
