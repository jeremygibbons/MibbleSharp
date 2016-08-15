﻿// <copyright file="AuthHMAC384SHA512.cs" company="None">
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
    /// The class <c>AuthHMAC256SHA384</c> implements the usmHMAC256SHA3846AuthProtocol
    /// defined by RFC 7630.
    /// </summary>
    public class AuthHMAC384SHA512 : AuthSHA2
    {
        public static readonly OID Id = new OID(new long[] { 1, 3, 6, 1, 6, 3, 10, 1, 1, 7 });
        private static readonly int HashLengthH384S512 = 64;
        private static readonly int AuthCodeLength = 48;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHMAC384SHA512"/> class,
        /// i.e. a <c>usmHMAC256SHA3846AuthProtocol</c> implementation.
        /// </summary>
        public AuthHMAC384SHA512() : base("SHA-512", Id, HashLengthH384S512, AuthCodeLength)
        {
        }
    }
}
