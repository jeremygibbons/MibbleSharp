// <copyright file="Row.cs" company="None">
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
//    C# conversion Copyright (c) 2017 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Util.Table
{
    using System.Collections.Generic;
    using JunoSnmp.SMI;

    internal class Row : List<VariableBinding>
    {
        private readonly OID index;

        public Row(OID index) : base()
        {
            this.index = index;
        }

        public OID RowIndex
        {
            get
            {
                return index;
            }
        }

        public int NumComplete
        {
            get
            {
                return base.Count;
            }

            set
            {
                if(value > this.NumComplete)
                {
                    //add nulls to extend the list
                    base.AddRange(new VariableBinding[value - this.NumComplete]);
                }
                else if(value < this.NumComplete)
                {
                    base.RemoveRange(value, this.NumComplete - value);
                }
                
            }
        }
    }
}
