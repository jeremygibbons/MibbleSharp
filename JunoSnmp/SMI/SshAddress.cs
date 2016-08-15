// <copyright file="SshAddress.cs" company="None">
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
    /// The <c>SshAddress</c> represents a SSH transport addresses as defined
    /// by RFC 5592 SnmpSSHAddress textual convention.
    /// </summary>
    public class SshAddress : TcpAddress
    {
        private static readonly log4net.ILog log = log4net.LogManager
           .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string addressURI;
        private string userName;

        public SshAddress(string addressURI)
        {
            this.addressURI = addressURI;
            this.ParseAddress(addressURI);
        }

        public SshAddress(System.Net.IPAddress ipAddress, int port) : base(ipAddress, port)
        {
            addressURI = "" + ipAddress.ToString() + ':' + port;
        }

        public SshAddress(System.Net.IPAddress ipAddress, int port, string userName) : base(ipAddress, port)
        {
            this.userName = userName;
            this.addressURI = userName + '@' + ipAddress.ToString() + ':' + port;
        }

        public string AddressURI
        {
            get
            {
                return this.addressURI;
            }
        }

        public string UserName
        {
            get
            {
                return userName;
            }
        }

        public override bool ParseAddress(string address)
        {
            try
            {
                string addressString = address;
                string portString = null;
                string userName = null;
                int lastColon = address.LastIndexOf(':');
                if ((lastColon >= 0) && (lastColon + 1 < address.Length))
                {
                    addressString = address.Substring(0, lastColon);
                    portString = address.Substring(lastColon + 1);
                }

                int firstAtPos = addressString.IndexOf('@');
                if ((firstAtPos > 0) && (firstAtPos + 1 < addressString.Length))
                {
                    userName = addressString.Substring(0, firstAtPos);
                    addressString = addressString.Substring(firstAtPos + 1);
                }

                try
                {
                    this.SetSystemNetIpAddress(System.Net.Dns.GetHostAddresses(addressString)[0]);
                    this.port = int.Parse(portString);
                    this.userName = userName;
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error("Failed to parse address '" + address + "' as SSH address: " + ex.Message, ex);
                return false;
            }
        }

        public override bool Equals(object o)
        {
            SshAddress sa = o as SshAddress;

            if (sa == null)
            {
                return false;
            }

            if (base.Equals(sa)
                && this.GetSystemNetIpAddress().Equals(sa.GetSystemNetIpAddress())
                && this.addressURI.Equals(sa.AddressURI)
                && (this.userName == null || this.userName.Equals(sa.UserName)))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 31 * result + addressURI.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            return "SshAddress[" +
                "addressURI='" + this.addressURI + '\'' +
                ']';
        }
    }
}
