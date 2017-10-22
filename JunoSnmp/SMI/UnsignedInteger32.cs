// <copyright file="UnsignedInteger32.cs" company="None">
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
//    Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.SMI
{
    using System;
    using System.IO;
    using JunoSnmp.ASN1;

    /// <summary>
    /// UnsignedInteger32 type is an SNMP type that represents unsigned 32bit
    /// integer values(0 to 4294967295).
    /// </summary>
    public class UnsignedInteger32 : AbstractVariable, IAssignableFrom<long>, IAssignableFrom<string>, IEquatable<UnsignedInteger32>
    {
        protected long value = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsignedInteger32"/> class with zero value.
        /// </summary>
        public UnsignedInteger32()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsignedInteger32"/> class with a given value.
        /// </summary>
        /// <param name="value">
        /// A <c>long</c> value which must not be less than 0 or greater than 2^32-1
        /// </param>
        /// <exception cref="ArgumentException">If the value is not in the allowable range</exception>
        public UnsignedInteger32(long value)
        {
            this.SetValue(value);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsignedInteger32"/> class with a given signed int value.
        /// Negative values are interpreted in a two's complement fashion as values between 2^31-1 and 2^32-1.
        /// </summary>
        /// <param name="signedIntValue">A signed int value</param>
        public UnsignedInteger32(int signedIntValue)
        {
            this.SetValue(signedIntValue & 0xFFFFFFFFL);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsignedInteger32"/> class with a given signed byte value.
        /// Negative values are interpreted in a two's complement fashion as values between 2^7-1 and 2^8-1.
        /// </summary>
        /// <param name="signedByteValue">A signed byte value</param>
        public UnsignedInteger32(byte signedByteValue)
        {
            this.SetValue(signedByteValue & 0xFF);
        }

        /// <summary>
        /// Gets the syntax for this <see cref="IVariable"/>
        /// </summary>
        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxUnsignedInteger32;
            }
        }

        /// <summary>
        /// Encodes this <see cref="UnsignedInteger32"/> to a BER output stream
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeUnsignedInteger(outputStream, BER.GAUGE, value);
        }

        /// <summary>
        /// Decodes the value of this <see cref="UnsignedInteger32"/> from a BER input stream
        /// </summary>
        /// <param name="inputStream">The stream to read from</param>
        public override void DecodeBER(BERInputStream inputStream)
        {
            long newValue = BER.DecodeUnsignedInteger(inputStream, out byte type);
            if (type != BER.GAUGE)
            {
                throw new IOException("Wrong type encountered when decoding Gauge: " +
                                      type);
            }

            this.SetValue(newValue);
        }

        /// <summary>
        /// Gets a hashcode for this object
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        public override int GetHashCode()
        {
            return (int)value;
        }

        /// <summary>
        /// Gets the BER encoding length for this variable
        /// </summary>
        public override int BERLength
        {
            get
            {
                if (value < 0x80L)
                {
                    return 3;
                }
                else if (value < 0x8000L)
                {
                    return 4;
                }
                else if (value < 0x800000L)
                {
                    return 5;
                }
                else if (value < 0x80000000L)
                {
                    return 6;
                }

                return 7;
            }
        }

        /// <summary>
        /// Tests this <see cref="UnsignedInteger32"/> for equality against another object
        /// </summary>
        /// <param name="o">Another object to compare against</param>
        /// <returns>
        /// True if <c>o</c> is an <see cref="UnsignedInteger32"/> with the same value, false if not
        /// </returns>
        public override bool Equals(object o)
        {
            UnsignedInteger32 ui = o as UnsignedInteger32;
            if (ui == null)
            {
                return false;
            }

            return this.Equals(ui);
        }

        public bool Equals(UnsignedInteger32 other)
        {
            return other.value == this.value;
        }

        public override int CompareTo(IVariable o)
        {
            long diff = (value - ((UnsignedInteger32)o).GetValue());

            if (diff < 0)
            {
                return -1;
            }
            else if (diff > 0)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Gets a string representation of thie variable
        /// </summary>
        /// <returns>A string representing this variable (in this case its value)</returns>
        public override string ToString()
        {
            return value.ToString();
        }

        /// <summary>
        /// Sets the value of this variable from a string
        /// </summary>
        /// <param name="val">A string containing a <c>long</c></param>
        public virtual void SetValue(string val)
        {
            SetValue(long.Parse(val));
        }

        /// <summary>
        /// Sets the value of this variable from a <c>long</c>
        /// </summary>
        /// <param name="val">The value</param>
        /// <exception cref="ArgumentException">If the value is not in the allowed range</exception>
        public void SetValue(long val)
        {
            if ((val < 0) || (val > 4294967295L))
            {
                throw new ArgumentException(
                    "Argument must be an unsigned 32bit value");
            }

            this.value = val;
        }

        /// <summary>
        /// Gets the value of this <see cref="UnsignedInteger32"/>
        /// </summary>
        /// <returns>The value of this variable as a <c>long</c></returns>
        public long GetValue()
        {
            return value;
        }

        /// <summary>
        /// Creates a copy of this <see cref="UnsignedInteger32"/>
        /// </summary>
        /// <returns>A new copy of this variable</returns>
        public override object Clone()
        {
            return new UnsignedInteger32(value);
        }

        /// <summary>
        /// Gets this variable's value as an <c>int</c>
        /// </summary>
        public override int IntValue
        {
            get
            {
                return (int)this.GetValue();
            }
        }

        /// <summary>
        /// Gets this variable's value as a <c>long</c>
        /// </summary>
        public override long LongValue
        {
            get
            {
                return this.GetValue();
            }
        }

        public override OID ToSubIndex(bool impliedLength)
        {
            return new OID(new long[] { this.LongValue });
        }

        /// <summary>
        /// Sets the value of this <c>IVariable</c> from the supplied (sub-)index.
        /// </summary>
        /// <param name="subIndex">The sub-index OID</param>
        /// <param name="impliedLength">
        /// Specifies if the sub-index has an implied length. This parameter applies
        /// to variable length variables only(e.g. <see cref="OctetString"/>
        /// and <see cref="OID"/>). For other variables it has no effect.
        /// </param>
        /// <exception cref="NotSupportedException">If this variable cannot be set from a sub index</exception>
        public override void FromSubIndex(OID subIndex, bool impliedLength)
        {
            this.SetValue(subIndex[0]);
        }
    }
}
