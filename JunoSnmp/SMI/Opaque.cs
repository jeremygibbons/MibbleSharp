// <copyright file="Opaque.cs" company="None">
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
    using System.IO;
    using JunoSnmp.ASN1;

    /// <summary>
    /// The <c>Opaque</c> class represents the SMI type Opaque which is used
    /// to transparently exchange BER encoded values.
    /// </summary>
    public class Opaque : OctetString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Opaque"/> class.
        /// </summary>
        public Opaque() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Opaque"/> class from an array of bytes
        /// </summary>
        /// <param name="bytes">An initial value as an array of bytes</param>
        public Opaque(byte[] bytes) : base(bytes)
        {
        }

        /// <summary>
        /// Gets the syntax for this <see cref="IVariable"/>
        /// </summary>
        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxOpaque;
            }
        }

        /// <summary>
        /// Encodes this <see cref="Opaque"/> to a BER output stream
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeString(outputStream, BER.OPAQUE, this.GetValue());
        }

        /// <summary>
        /// Decodes the value of this <see cref="Opaque"/> from a BER input stream
        /// </summary>
        /// <param name="inputStream">The stream to read from</param>
        public override void DecodeBER(BERInputStream inputStream)
        {
            byte[] v = BER.DecodeString(inputStream, out BER.MutableByte type);
            if (type.Value != (BER.Asn1Application | 0x04))
            {
                throw new IOException("Wrong type encountered when decoding OctetString: " +
                                      type.Value);
            }

            this.SetValue(v);
        }

        /// <summary>
        /// Sets the values of this variable from a given <see cref="OctetString"/>
        /// </summary>
        /// <param name="value">The Octet String to set values from</param>
        public void SetValue(OctetString value)
        {
            this.SetValue(new byte[0]);
            this.Append(value);
        }

        /// <summary>
        /// Gets a string representation of this variable
        /// </summary>
        /// <returns>A string representation of this variable, as a series of hexadecimal values</returns>
        public override string ToString()
        {
            return base.ToHexString();
        }

        /// <summary>
        /// Creates a copy of this <see cref="Opaque"/>
        /// </summary>
        /// <returns>A new copy of this variable</returns>
        public override object Clone()
        {
            return new Opaque(base.GetValue());
        }
    }
}
