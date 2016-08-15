// <copyright file="ISecurityParameters.cs" company="None">
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
    using JunoSnmp.ASN1;

    /// <summary>
    /// The <see cref="ISecurityParameters"/> interface represents the security
    /// parameters in a SNMPv3 message.
    /// </summary>
    public interface ISecurityParameters : IBERSerializable
    {
        /// <summary>
        /// Gets or sets the byte position of the first byte (counted from zero) of the
        /// security parameters in the whole message.
        /// </summary>
        int SecurityParametersPosition { get; set; }
        
        /// <summary>
        /// Gets the maximum length of the BER encoded representation of this
        /// <c>SecurityParameters</c> instance.
        /// </summary>
        /// <param name="securityLevel">The security level to be used.</param>
        int GetBERMaxLength(int securityLevel);
    }
}
