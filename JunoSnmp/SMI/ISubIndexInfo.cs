// <copyright file="SubIndexInfo.cs" company="None">
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

namespace JunoSnmp.SMI
{
    /// <summary>
    /// The <see cref="ISubIndexInfo"/> interface represents the meta information of a SMI INDEX 
    /// clause element (= sub-index) which are relevant for converting an OID index value to an 
    /// INDEX object and vice versa.
    /// </summary>
    public interface ISubIndexInfo
    {
        /// <summary>
        /// Gets a value indicating whether the sub-index represented by this index info has an implied length or not.
        /// </summary>
        bool HasImpliedLength { get; }

        /// <summary>
        /// Gets the minimum length in bytes of the sub-index. If min and max length are equal, then this sub-index
        /// is a fixed length length sub - index.
        /// </summary>
        int MinLength { get; }

        /// <summary>
        /// Gets the maximum length in bytes of the sub-index. If min and max length are equal, then this sub-index
        /// is a fixed length length sub - index.
        /// </summary>
        int MaxLength { get; }

        /// <summary>
        /// Gets the SNMP syntax value of the sub-index' base syntax.
        /// </summary>
        int SnmpSyntax { get; }
    }
}
