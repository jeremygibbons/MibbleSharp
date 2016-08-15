// <copyright file="AuthSHA2.cs" company="None">
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
    using JunoSnmp.SMI;

    /// <summary>
    /// The <code>SHA-2</code> class implements the Secure Hash Authentication 2.
    /// </summary>
    public class AuthSHA2 : AuthGeneric
    {
        /// <summary>
        /// The object identifier that identifies this authentication protocol.
        /// </summary> 
        private OID protocolID;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthSHA2"/> class,with the specified hash length.
        /// </summary>
        /// <param name="protocolName">The SHA protocol name, i.e. "SHA-256"</param>
        /// <param name="protocolOID">The OID of the protocol as defined in RFC 7630</param>
        /// <param name="hashLength">The hash length</param>
        /// <param name="authenticationCodeLength">The length of the authentication hash output in bytes</param>
        public AuthSHA2(string protocolName, OID protocolOID, int hashLength, int authenticationCodeLength)
                : base(protocolName, hashLength, authenticationCodeLength)
        {
            this.protocolID = protocolOID;
        }

        /// <summary>
        /// Gets the OID uniquely identifying the privacy protocol.
        /// </summary>
        public override OID ID
        {
            get
            {
                return (OID)this.protocolID.Clone();
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
