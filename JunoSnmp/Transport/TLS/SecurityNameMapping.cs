// <copyright file="SecurityNameMapping.cs" company="None">
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

namespace JunoSnmp.Transport.TLS
{
    using System;
    using JunoSnmp.SMI;
    /// <summary>
    /// The <c>SecurityNameMapping</c> maps a X509 certificate identified by its
    /// fingerprint to a security name based on a mapping defined by <see cref="CertMappingType"/>.
    /// </summary>
    public class SecurityNameMapping
    {

        public enum CertMappingType
        {
            Specified,
            SANRFC822Name,
            SANDNSName,
            SANIpAddress,
            SANAny,
            CommonName
        }

        private OctetString fingerprint;
        private OctetString data;
        private CertMappingType type;
        private OctetString securityName;

        public SecurityNameMapping(OctetString fingerprint, OctetString data, CertMappingType type,
                                   OctetString securityName)
        {
            this.fingerprint = fingerprint;
            this.data = data;
            this.type = type;
            this.securityName = securityName;
        }

        public OctetString getFingerprint()
        {
            return fingerprint;
        }

        public OctetString getData()
        {
            return data;
        }

        public CertMappingType getType()
        {
            return type;
        }

        public OctetString getSecurityName()
        {
            return securityName;
        }

      public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || this.GetType() != o.GetType()) return false;

            SecurityNameMapping that = o as SecurityNameMapping;

            if (data != null ? !data.Equals(that.data) : that.data != null) return false;
            if (fingerprint != null ? !fingerprint.Equals(that.fingerprint) : that.fingerprint != null) return false;
            if (type != that.type) return false;

            return true;
        }

      public override int GetHashCode()
        {
            int result = fingerprint != null ? fingerprint.GetHashCode() : 0;
            result = 31 * result + (data != null ? data.GetHashCode() : 0);
            result = 31 * result + type.GetHashCode();
            return result;
        }

      public override string ToString()
        {
            return "SecurityNameMapping{" +
                "fingerprint=" + fingerprint +
                ", data=" + data +
                ", type=" + type +
                ", securityName=" + securityName +
                '}';
        }
    }
}
