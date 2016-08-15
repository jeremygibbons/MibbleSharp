// <copyright file="IBERSerializable.cs" company="None">
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

namespace JunoSnmp.ASN1
{
    using System.IO;

    /// <summary>
    /// The interface <code>BERSerializable</code> has to be implemented by
    /// any data type class that needs to be serialized using the Basic Encoding
    /// Rules(BER) that provides encoding rules for ASN.1 data types.
    /// </summary>
    public interface IBERSerializable
    {
        /// <summary>
        /// Gets the length of this <code>BERSerializable</code> object
        /// in bytes when encoded according to the Basic Encoding Rules(BER).
        /// </summary>
        int BERLength { get; }
        
        /// <summary>
        /// Gets the length of the payload of this <code>BERSerializable</code> object
        /// in bytes when encoded according to the Basic Encoding Rules(BER).
        /// </summary>
        int BERPayloadLength { get; }
        
        /// <summary>
        /// Decodes a <c>IVariable</c> from an <c>InputStream</c>.
        /// </summary>
        /// <param name="inputStream">
        /// An <code>InputStream</code> containing a BER encoded byte stream.
        /// </param>
        /// <exception cref="IOException">
        /// If the stream could not be decoded by using BER rules
        /// </exception>
        void DecodeBER(BERInputStream inputStream);
        
        /// <summary>
        /// Encodes a <c>IVariable</c> to an <c>OutputStream</c>.
        /// </summary>
        /// <param name="outputStream">A stream to write to</param>
        /// <exception cref="IOException">If an error occurs writing to the stream</exception>
        void EncodeBER(Stream outputStream);
    }
}
