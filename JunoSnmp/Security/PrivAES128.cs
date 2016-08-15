// <copyright file="PrivAES128.cs" company="None">
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
    /// Encryption class for AES 128.
    /// </summary>
    public class PrivAES128 : PrivAES
    {
        /// <summary>
        /// Unique ID of this privacy protocol.
        /// </summary>
        public static readonly OID Oid = new OID("1.3.6.1.6.3.10.1.2.4");

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivAES128"/> class.
        /// </summary>
        public PrivAES128() : base(16)
        {
        }

        /// <summary>
        /// Gets the OID uniquely identifying the privacy protocol.
        /// </summary>
        /// <returns></returns>
        public override OID ID
        {
            get
            {
                return (OID)Oid.Clone();
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
