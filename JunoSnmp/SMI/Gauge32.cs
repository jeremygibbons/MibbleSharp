// <copyright file="Gauge32.cs" company="None">
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
    /// The <see cref="Gauge32"/> variable type. It is in practice indistinguishable from the generic
    /// <see cref="UnsignedInteger32"/> type.
    /// </summary>
    public class Gauge32 : UnsignedInteger32
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gauge32"/> class with 0 value.
        /// </summary>
        public Gauge32() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gauge32"/> class with a given value.
        /// </summary>
        /// <param name="value">The initial value</param>
        public Gauge32(long value) : base(value)
        {
        }

        /// <summary>
        /// Gets the syntax for this <see cref="IVariable"/>
        /// </summary>
        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxGauge32;
            }
        }

        /// <summary>
        /// Creates a copy of this <see cref="Gauge32"/>
        /// </summary>
        /// <returns>A new copy of this variable</returns>
        public override object Clone()
        {
            return new Gauge32(this.value);
        }
    }
}
