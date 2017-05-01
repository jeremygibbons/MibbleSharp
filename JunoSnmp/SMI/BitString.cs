// <copyright file="BitString.cs" company="None">
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
    using System;
    using System.IO;
    using JunoSnmp.ASN1;

    /// <summary>
    /// The <see cref="BitString"/> class represents the obsolete SMI type
    /// BIT STRING which has been defined in RFC 1442 (an SNMPv2 draft) but
    /// which has been obsoleteted by RFC 1902 and RFC 2578. This type is
    /// provided for compatibility only and should not be used for new
    /// applications.
    /// </summary>
    public class BitString : OctetString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitString"/> class.
        /// </summary>
        /// <remarks>
        /// The Bit String type was defined in RFC 1442 and rendered obsolete by RFC 2578. Use
        /// <see cref="OctetString"/> (i.e. BITS syntax) instead.
        /// </remarks>
        public BitString()
        {
        }

        /// <summary>
        /// Gets the syntax for this variable
        /// </summary>
        public override int Syntax
        {
            get
            {
                return BER.Asn1BitString;
            }
        }

        /// <summary>
        /// Encodes this bit string to a BER output stream
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeString(outputStream, BER.BITSTRING, this.GetValue());
        }

        /// <summary>
        /// Decodes the value of this bit string from a BER input stream
        /// </summary>
        /// <param name="inputStream">The stream to read the value from</param>
        public override void DecodeBER(BERInputStream inputStream)
        {
            byte[] v = BER.DecodeString(inputStream, out byte type);
            if (type != BER.BITSTRING)
            {
                throw new IOException("Wrong type encountered when decoding BitString: " +
                                      type);
            }

            this.SetValue(v);
        }

        /// <summary>
        /// Create a copy of this <see cref="BitString"/>
        /// </summary>
        /// <returns>A new copy of this <see cref="BitString"/> with the same value</returns>
        public override object Clone()
        {
            BitString clone = new BitString();
            clone.SetValue(base.GetValue());
            return clone;
        }
    }
}
