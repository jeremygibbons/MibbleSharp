// <copyright file="AuthMD5.cs" company="None">
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
    using System;
    using System.Runtime.Serialization;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;

    /// <summary>
    /// The AuthMD5 class implements the MD5 authentication protocol.
    /// </summary>
    public class AuthMD5 : AuthGeneric
    {
        private static readonly OID Id = new OID(SnmpConstants.usmHMACMD5AuthProtocol);

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthMD5"/> class.
        /// </summary>
        public AuthMD5() : base("MD5", 16)
        {
        }

        /// <summary>
        /// Gets the OID uniquely identifying the privacy protocol.
        /// </summary>
        public override OID ID
        {
            get
            {
                return (OID)Id.Clone();
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
