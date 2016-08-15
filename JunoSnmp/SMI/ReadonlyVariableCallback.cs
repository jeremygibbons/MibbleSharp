// <copyright file="ReadonlyVariableCallback.cs" company="None">
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
    /// This abstract class helps to implement a <see cref="IVariantVariableCallback"/>
    /// for a read-only Variable.
    /// </summary>
    public abstract class ReadonlyVariableCallback : IVariantVariableCallback
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadonlyVariableCallback"/> class.
        /// </summary>
        public ReadonlyVariableCallback()
        {
        }

        /// <summary>
        /// The supplied variable needs to be updated because it is about
        /// to be read.
        /// </summary>
        /// <param name="variable">The <see cref="VariantVariable"/> that needs updating</param>
        public abstract void UpdateVariable(VariantVariable variable);

        /// <summary>
        /// The supplied variable's value has been updated.
        /// </summary>
        /// <param name="variable">The <see cref="VariantVariable"/> that has been updated</param>
        public void VariableUpdated(VariantVariable variable)
        {
        }
    }
}
