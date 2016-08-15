﻿// <copyright file="SecurityLevel.cs" company="None">
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
    /// <summary>
    /// The <code>SecurityLevel</code> interface contains enumerated values
    /// for the different security levels.
    /// </summary>
    public enum SecurityLevel
    {
        /// <summary>
        /// The undefined value
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// No authentication and no encryption.
        /// Anyone can create and read messages with this security level
        /// </summary>
        NoAuthNoPriv = 1,

        /// <summary>
        /// Authentication and no encryption.
        /// Only the one with the right authentication key can create messages
        /// with this security level, but anyone can read the contents of
        /// the message.
        /// </summary>
        AuthNoPriv = 2,

        /// <summary>
        /// Authentication and encryption.
        /// Only the one with the right authentication key can create messages
        /// with this security level, and only the one with the right
        /// encryption/decryption key can read the contents of the message.
        /// </summary>
        AuthPriv = 3
    }
}
