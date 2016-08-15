// <copyright file="IAddress.cs" company="None">
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
    /// The <code>Address</code> interface serves as a base class for all SNMP
    /// transport addresses.
    /// </summary>
    public interface IAddress :
        /// Allows for setting the address value from the supplied string. The string must match
        ///the format required for the Address instance implementing this interface.
        /// Otherwise an <see cref="System.ArgumentException"/> runtime exception is thrown.
        IAssignableFrom<string>,
        IAssignableFrom<byte[]>
    {
        /// <summary>Gets a value indicating whether this address is valid</summary>
        bool IsValid { get; }
        
        /// <summary>
        /// Parses the address from the supplied string representation.
        /// </summary>
        /// <param name="address">A string representation of this address</param>
        /// <returns>
        /// True if the address could be successfully parsed and assigned to this address object,
        /// False if not.
        /// </returns>
        bool ParseAddress(string address);
    }
}
