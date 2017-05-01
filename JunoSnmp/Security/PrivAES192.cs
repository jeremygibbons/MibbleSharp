// <copyright file="PrivAES192.cs" company="None">
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

namespace JunoSnmp.Security
{
    using JunoSnmp.SMI;
    using JunoSnmp.MP;
    using JunoSnmp.Security.NonStandard;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Encryption class for AES 192.
    /// </summary>
    public class PrivAES192 : PrivAES, INonStandardSecurityProtocol
    {
        /// <summary>
        /// Unique ID of this privacy protocol.
        /// </summary>
        public static readonly OID Oid = new OID(SnmpConstants.oosnmpUsmAesCfb192Protocol);

        private OID oid;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PrivAES192() : base(24)
        {
        }

        /// <summary>
        /// Gets the OID uniquely identifying the privacy protocol.
        /// </summary>
        public override OID ID
        {
            get
            {
                return (oid == null) ? DefaultID : oid;
            }
        }

        public OID DefaultID
        {
            get
            {
                return (OID)Oid.Clone();
            }
        }

        public void SetID(OID newOID)
        {
            oid = new OID(newOID);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
