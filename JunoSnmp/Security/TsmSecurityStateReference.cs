// <copyright file="TsmSecurityStateReference.cs" company="None">
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
    /// The <c>TsmSecurityStateReference</c> holds cached security data
    /// for the <see cref="TSM"/> security model.
    /// </summary>
    public class TsmSecurityStateReference : ISecurityStateReference
    {

        private TransportStateReference tmStateReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="TsmSecurityStateReference"/> class.
        /// </summary>
        public TsmSecurityStateReference()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="TransportStateReference"/> information
        /// </summary>
        public TransportStateReference TmStateReference
        {
            get
            {
                return this.tmStateReference;
            }

            set
            {
                this.tmStateReference = value;
            }
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return "TsmSecurityStateReference[" +
                "tmStateReference=" + tmStateReference +
                ']';
        }
    }
}
