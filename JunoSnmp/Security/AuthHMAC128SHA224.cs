// <copyright file="AuthHMAC128SHA224.cs" company="None">
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

    /// <summary>
    /// The class <c>AuthHMAC128SHA224</c> implements the usmHMAC128SHA224AuthProtocol
    /// defined by RFC 7630.
    /// </summary>
    public class AuthHMAC128SHA224 : AuthSHA2
    {
        public static readonly OID Id = new OID(new long[] { 1, 3, 6, 1, 6, 3, 10, 1, 1, 4 });
        private static readonly int HashLengthH128S224 = 28;
        private static readonly int AuthCodeLength = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHMAC128SHA224"/> class,
        /// i.e. a usmHMAC128SHA224AuthProtocol implementation.
        /// </summary>
        public AuthHMAC128SHA224() : base("SHA-224", Id, HashLengthH128S224, AuthCodeLength)
        {
        }
    }
}
