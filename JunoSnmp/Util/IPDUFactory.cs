// <copyright file="PDUFactory.cs" company="None">
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

namespace JunoSnmp.Util
{
    using JunoSnmp.MP;
   
    /// <summary>
    /// The interface for PDU Factories
    /// </summary>
    public interface IPDUFactory
    {
        /// <summary>
        /// Creates a <see cref="PDU"/> instance for the supplied target. The created
        /// PDU has to be compliant to the SNMP version defined by the supplied target.
        /// For example, a SNMPv3 target requires a ScopedPDU instance.
        /// </summary>
        /// <param name="target">
        /// The <c>ITarget</c> where the PDU to be created will be sent.
        /// </param>
        /// <returns>
        /// A PDU instance that is compatible with the supplied target.
        /// </returns>
        PDU CreatePDU(ITarget target);

        /// <summary>
        /// Creates a {@link PDU} instance that is compatible with the given SNMP version
        /// (message processing model).
        /// </summary>
        /// <param name="messageProcessingModel">
        /// A <see cref="MessageProcessingModel"/> instance.
        /// </param>
        /// <returns>
        /// A <see cref="PDU"/> instance that is compatible with
        /// the given SNMP version (message processing model)
        /// </returns>
        PDU CreatePDU(MessageProcessingModel messageProcessingModel);
    }
}
