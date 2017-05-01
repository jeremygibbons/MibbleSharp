// <copyright file="SecureTarget.cs" company="None">
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
//    Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp
{
    using System.Runtime.Serialization;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <code>SecureTarget</code> is an security model independent abstract class
    /// for all targets supporting secure SNMP communication.
    /// </summary>
    public abstract class SecureTarget : AbstractTarget, ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureTarget"/> class.
        /// </summary>
        protected SecureTarget()
        {
        }
        
        /// <summary>
        /// Creates a SNMPv3 secure target with an address and security name.
        /// </summary>
        /// <param name="address">
        /// a <code>OctetString</code> instance representing the security name
        /// of the USM user used to access the target.
        /// </param>
        /// <param name="securityName">
        /// an <code>Address</code> instance denoting the transport address of the
        /// target.
        /// </param>
        protected SecureTarget(IAddress address, OctetString securityName) : base(address, securityName)
        {
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return "SecureTarget[" + ToStringAbstractTarget() + ']';
        }
    }
}
