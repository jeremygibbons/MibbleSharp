// <copyright file="MessageLengthDecoder.cs" company="None">
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

namespace JunoSnmp.Transport
{
    using System.IO;
    /// <summary>
    /// The <c>MessageLengthDecoder</c> needs to be implemented for connection
    /// oriented transport mappings, because those transport mappings have no message
    /// boundaries.To determine the message length, the message header is decoded
    /// in a protocol specific way.
    /// </summary>
    public interface MessageLengthDecoder
    {
        /// Gets the minimum length of the header to be decoded. Typically this
        /// is a constant value.
        /// </summary>
        int MinHeaderLength { get; }
        
        /// <summary>
        /// Returns the total message length to read (including header) and
        /// the actual header length.
        /// </summary>
        /// <param name="ms">A <c>MemoryStream</c> with a minimum length of <see cref="MinHeaderLength"/></param>
        /// <returns>A <see cref="MessageLength"/> object containing the message length information</returns>
        MessageLength GetMessageLength(MemoryStream ms);
    }
}
