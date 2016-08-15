// <copyright file="ScopedPDU.cs" company="None">
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

namespace JunoSnmp
{
    using System;
    using System.IO;
    using System.Text;
    using JunoSnmp.ASN1;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <c>ScopedPDU</c> class represents a SNMPv3 scoped PDU.
    /// </summary>
    public class ScopedPDU : PDU
    {
        private OctetString contextEngineID = new OctetString();
        private OctetString contextName = new OctetString();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedPDU"/> class, empty.
        /// </summary>
        public ScopedPDU()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedPDU"/> class, copying its values from
        /// another <see cref="ScopedPDU"/>.
        /// </summary>
        /// <param name="other">A scoped PDU to copy values from</param>
        public ScopedPDU(ScopedPDU other) : base(other)
        {
            this.contextEngineID = (OctetString)other.contextEngineID.Clone();
            this.contextName = (OctetString)other.contextName.Clone();
        }
        
        /// <summary>
        /// Gets or sets the context engine ID of this scoped PDU.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the passed in ID is null</exception>
        public OctetString ContextEngineID
        {
            get
            {
                return this.contextEngineID;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Context engine ID must not be null");
                }

                this.contextEngineID = value;
            }
        }

        /// <summary>
        /// Gets or sets the context name of this scoped PDU.
        /// </summary>
        public OctetString ContextName
        {
            get
            {
                return this.contextName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Context name must not be null");
                }

                this.contextName = value;
            }
        }

        /// <summary>
        /// Gets the length of the BER encoding for this object
        /// </summary>
        public override int BERLength
        {
            get
            {
                int length = this.BERPayloadLength;
                length += 1 + BER.GetBERLengthOfLength(length);
                return length;
            }
        }

        /// <summary>
        /// Gets the length of the BER encoding of this object's payload
        /// </summary>
        public override int BERPayloadLength
        {
            get
            {
                int length = base.BERLength;
                int cid = (contextEngineID == null) ? 0 : contextEngineID.Length;
                int cn = (contextName == null) ? 0 : contextName.Length;
                length += BER.GetBERLengthOfLength(cid) + 1
                    + cid + BER.GetBERLengthOfLength(cn) + 1 + cn;
                return length;
            }
        }

        /// <summary>
        /// Writes this object in BER format to an output stream
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeHeader(outputStream, BER.SEQUENCE, this.BERPayloadLength);
            contextEngineID.EncodeBER(outputStream);
            contextName.EncodeBER(outputStream);
            base.EncodeBER(outputStream);
        }

        /// <summary>
        /// Create and return a copy of this object
        /// </summary>
        /// <returns>A fresh copy of this <see cref="ScopedPDU"/></returns>
        public override object Clone()
        {
            return new ScopedPDU(this);
        }
        
        /// <summary>
        /// Decodes a <see cref="ScopedPDU"/> from a BER input stream.
        /// </summary>
        public override void DecodeBER(BERInputStream inputStream)
        {
            BER.MutableByte mutableByte = new BER.MutableByte();
            int length = BER.DecodeHeader(inputStream, out mutableByte);
            long startPos = inputStream.Position;
            contextEngineID.DecodeBER(inputStream);
            contextName.DecodeBER(inputStream);
            base.DecodeBER(inputStream);
            if (BER.CheckSequenceLengthFlag)
            {
                BER.CheckSequenceLength(
                    length,
                    (int)(inputStream.Position - startPos),
                    this);
            }
        }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object</returns>
        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(PDU.GetTypeString(type));
            buf.Append("[{contextEngineID=" + contextEngineID + ", contextName=" + contextName + "}, requestID=");
            buf.Append(requestID);
            buf.Append(", errorStatus=");
            buf.Append(errorStatus);
            buf.Append(", errorIndex=");
            buf.Append(errorIndex);
            buf.Append(", VBS[");
            for (int i = 0; i < variableBindings.Count; i++)
            {
                buf.Append(variableBindings[i]);
                if (i + 1 < variableBindings.Count)
                {
                    buf.Append("; ");
                }
            }

            buf.Append("]]");
            return buf.ToString();
        }

        /// <summary>
        /// Tests this object for equality with another object
        /// </summary>
        /// <param name="obj">The object to be compared with</param>
        /// <returns>True if the objects are equal, false if not</returns>
        public override bool Equals(object obj)
        {
            if (obj is ScopedPDU)
            {
                ScopedPDU o = (ScopedPDU)obj;
                return base.Equals(obj) &&
                  AbstractVariable.Equal(contextEngineID, o.contextEngineID) &&
                  AbstractVariable.Equal(contextName, o.contextName);
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Gets a hashcode for this object
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
