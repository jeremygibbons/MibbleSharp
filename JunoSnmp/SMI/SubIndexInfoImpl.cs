// <copyright file="SubIndexInfoImpl.cs" company="None">
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
    /// The <see cref="SubIndexInfoImpl"/> class represents the meta information of a SMI INDEX clause element (= sub-index)
    /// which are relevant for converting an OID index value to an INDEX object and vice versa.
    /// </summary>
    public class SubIndexInfoImpl : ISubIndexInfo
    {
        private readonly bool impliedLength;
        private readonly int minLength;
        private readonly int maxLength;
        private readonly int snmpSyntax;

        /**
         * Create a sub index information object.
         * @param impliedLength
         *    indicates if the sub-index value has an implied variable length (must apply to the last variable length
         *    sub-index only).
         * @param minLength
         *    the minimum length in bytes of the sub-index value.
         * @param maxLength
       *      the maximum length in bytes of the sub-index value.
         * @param snmpSyntax
         *    the BER syntax of the sub-index object type's base syntax.
         */
        public SubIndexInfoImpl(bool impliedLength, int minLength, int maxLength, int snmpSyntax)
        {
            this.impliedLength = impliedLength;
            this.maxLength = maxLength;
            this.minLength = minLength;
            this.snmpSyntax = snmpSyntax;
        }

        public bool HasImpliedLength
        {
            get
            {
                return impliedLength;
            }
        }

        public int MinLength
        {
            get
            {
                return this.minLength;
            }
        }

        public int MaxLength
        {
            get
            {
                return this.maxLength;
            }
        }

        public int SnmpSyntax
        {
            get
            {
                return this.snmpSyntax;
            }
        }
    }
}
