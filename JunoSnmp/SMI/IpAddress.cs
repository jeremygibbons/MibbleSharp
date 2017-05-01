// <copyright file="IpAddress.cs" company="None">
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
    /// The <c>IpAddress</c> class represents an IPv4 address SNMP variable.
    /// </summary>
    public class IpAddress : SMIAddress, IEquatable<IpAddress>
    {
        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private static readonly byte[] IpAnyAddressBytes = { 0, 0, 0, 0 };

        public static readonly System.Net.IPAddress IpAnyAddress = CreateAnyAddress();

        private System.Net.IPAddress ipAddress;

        /// <summary>
        /// Creates a <c>0.0.0.0</c> IP address.
        /// </summary>
        public IpAddress()
        {
            this.ipAddress = IpAnyAddress;
        }
        
        /// <summary>
        /// Creates an SMI <c>IPAddress</c> from an <see cref="System.Net.IPAddress"/>
        /// </summary>
        /// <param name="address">An address instance (not necessarily an IPv4 address)</param>
        public IpAddress(System.Net.IPAddress address)
        {
            if (address == null)
            {
                throw new ArgumentNullException();
            }

            this.ipAddress = address;
        }
        
        /// <summary>
        /// Create an IP address from an address string.
        /// </summary>
        /// <param name="address">A string containing an IP Address</param>
        /// <see cref="IAddress.ParseAddress(string)"/>
        public IpAddress(string address)
        {
            if (!this.ParseAddress(address))
            {
                throw new ArgumentException(address);
            }
        }

        /**
         * Create an IP address from a raw IP address. The argument is in
         * network byte order: the highest order byte of the address is in first
         * element of the supplied byte array.
         * @param addressBytes
         *    the raw IP address in network byte order.
         * @since 1.10.2
         */
        public IpAddress(byte[] addressBytes)
        {
            try
            {
                this.ipAddress = new IPAddress(addressBytes);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Invalid address: " + ex.Message);
            }
        }

        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxIpAddress;
            }
        }

        public override bool IsValid
        {
            get
            {
                return (this.ipAddress != null);
            }

        }

        public override string ToString()
        {
            if (this.ipAddress != null)
            {
                return ipAddress.ToString();
            }

            return "0.0.0.0";
        }

        public override int GetHashCode()
        {
            if (this.ipAddress != null)
            {
                return this.ipAddress.GetHashCode();
            }

            return 0;
        }

        /**
         * Parses an IP address string and returns the corresponding
         * <code>IpAddress</code> instance.
         * @param address
         *    an IP address string which may be a host name or a numerical IP address.
         * @return
         *    an <code>IpAddress</code> instance or <code>null</code> if
         *    <code>address</code> cannot not be parsed.
         * @see Address#parseAddress(String address)
         */
        public static IAddress Parse(string address)
        {
            try
            {
                IPAddress addr = Dns.GetHostAddresses(address)[0];
                return new IpAddress(addr);
            }
            catch (ArgumentException ex)
            {

                log.Error("Unable to parse IpAddress from: " + address, ex);
                return null;
            }
        }

        public override bool ParseAddress(string address)
        {
            try
            {
                this.ipAddress = Dns.GetHostAddresses(address)[0];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override int CompareTo(IVariable o)
        {
            IpAddress b = o as IpAddress;
            if(b == null)
            {
                throw new ArgumentException("Argument is not an IpAddress object: " + o);
            }

            OctetString a = new OctetString(this.ipAddress.GetAddressBytes());
            return a.CompareTo(new OctetString(b.GetSystemNetIpAddress().GetAddressBytes()));
        }

        public override bool Equals(object o)
        {
            return (o is IpAddress) && (CompareTo((IpAddress)o) == 0);
        }

        public bool Equals(IpAddress ip)
        {
            return this.CompareTo(ip) == 0;
        }

        public override void DecodeBER(BERInputStream inputStream)
        {
            BER.MutableByte type;
            byte[] value = BER.DecodeString(inputStream, out type);
            if (type.Value != BER.IPADDRESS)
            {
                throw new IOException("Wrong type encountered when decoding Counter: " +
                                      type.Value);
            }

            if (value.Length != 4)
            {
                throw new IOException("IpAddress encoding error, wrong length: " +
                                      value.Length);
            }
            
            this.ipAddress = new IPAddress(value);
        }

        public override void EncodeBER(Stream outputStream)
        {
            byte[] address = new byte[4];
            // TODO: Check IPv6 support
            /*
            if (this.ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                Inet6Address v6Addr = (Inet6Address)inetAddress;
                if (6Addr.isIPv4CompatibleAddress())
                {
                    byte[] v6Bytes = this.ipAddress.GetAddressBytes();
                    System.Array.Copy(v6Bytes, v6Bytes.Length - 5, address, 0, 4);
                }
            }
            else
            {
                System.Array.Copy(this.ipAddress.GetAddressBytes(), 0, address, 0, 4);
            }
            */
            System.Array.Copy(this.ipAddress.GetAddressBytes(), 0, address, 0, 4);
            BER.EncodeString(outputStream, BER.IPADDRESS, address);
        }

        public override int BERLength
        {
            get
            {
                return 6;
            }
        }

        public void SetAddress(byte[] rawValue)
        {
            this.ipAddress = new IPAddress(rawValue);
        }

        public void SetSystemNetIpAddress(System.Net.IPAddress ipAddress)
        {
            this.ipAddress = ipAddress;
        }

        public System.Net.IPAddress GetSystemNetIpAddress()
        {
            return this.ipAddress;
        }

        private static System.Net.IPAddress CreateAnyAddress()
        {
            try
            {
                return new System.Net.IPAddress(IpAddress.IpAnyAddressBytes);
            }
            catch (Exception ex)
            {
                log.Error("Unable to create any IpAddress: " + ex.Message, ex);
            }
            return null;
        }

        public override object Clone()
        {
            return new IpAddress(this.ipAddress);
        }

        public override int IntValue
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long LongValue
        {
            get
            {
                throw new NotSupportedException();
            }
            
        }

        public override OID ToSubIndex(bool impliedLength)
        {
            byte[] address = new byte[4];
            System.Array.Copy(this.ipAddress.GetAddressBytes(), 0, address, 0, 4);
            OID subIndex = new OID(new long[4]);
            for (int i = 0; i < address.Length; i++)
            {
                subIndex[i] = address[i] & 0xFF;
            }

            return subIndex;
        }

        public override void FromSubIndex(OID subIndex, bool impliedLength)
        {
            byte[] rawValue = new byte[4];

            for (int i = 0; i < rawValue.Length; i++)
            {
                rawValue[i] = (byte)(subIndex[i] & 0xFF);
            }

            this.SetAddress(rawValue);
            
        }

        public override void SetValue(string value)
        {
            if (!this.ParseAddress(value))
            {
                throw new ArgumentException(value + " cannot be parsed by " + this.GetType().Name);
            }
        }

        public override void SetValue(byte[] value)
        {
            this.SetAddress(value);
        }
    }
}
