// <copyright file="INonStandardSecurityProtocol.cs" company="None">
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

namespace JunoSnmp.Security.NonStandard
{
    using JunoSnmp.SMI;

    /// <summary>
    /// With the <c>INonStandardSecurityProtocol</c> interface you can modify
    /// the ID of a non-standard security protocol to match the ID that is used
    /// by that protocol in your environment.
    /// </summary>
    public interface INonStandardSecurityProtocol
    {
        /**
         * Assign a new ID to a non-standard security protocol instance.
         *
         * @param newOID
         *    the new security protcol ID for the security protocol class called.
         */
        void SetID(OID newOID);

        /**
         * Gets the default ID for this non-standard privacy protocol.
         * @return
         *    the default ID as defined by the OOSNMP-USM-MIB.
         */
        OID DefaultID { get; }
    }
}
