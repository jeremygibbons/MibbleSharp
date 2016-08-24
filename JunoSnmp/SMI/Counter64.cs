// <copyright file="Counter64.cs" company="None">
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
    using System.Numerics;
    using JunoSnmp.ASN1;

    /// <summary>
    /// The <c>Counter64</c> class represents a 64 bit unsigned integer type.
    /// It is used for monotonically increasing values that wrap around at
    /// 2^64-1 (18446744073709551615). The unsigned 64 bit value is represented as a signed
    /// 64 bit long value internally
    /// </summary>
    public class Counter64 : AbstractVariable, IAssignableFrom<long>, IAssignableFrom<string>
    {

        private long value = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Counter64"/> class with 0 value.
        /// </summary>
        public Counter64()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Counter64"/> class with a given initial value.
        /// </summary>
        /// <param name="value">The initial value</param>
        public Counter64(long value)
        {
            this.SetValue(value);
        }

        /// <summary>
        /// Gets the Counter64 value as a signed integer.
        /// </summary>
        /// <remarks>This will truncate the value to a signed int range</remarks>
        public override int IntValue
        {
            get
            {
                return (int)this.GetValue();
            }

        }

        /// <summary>
        /// Gets the Counter64 value as a signed long
        /// </summary>
        /// <remarks>This will return the internal signed 64 bit value</remarks>
        public override long LongValue
        {
            get
            {
                return this.GetValue();
            }
        }

        /// <summary>
        /// Gets the syntax of this <see cref="IVariable"/>
        /// </summary>
        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxCounter64;
            }
        }

        /// <summary>
        /// Gets the length of the BER encoding of this variable
        /// </summary>
        public override int BERLength
        {
            get
            {
                if (this.value < 0L)
                {
                    return 11;
                }

                if (this.value < 0x80000000L)
                {
                    if (this.value < 0x8000L)
                    {
                        return (this.value < 0x80L) ? 3 : 4;
                    }

                    return (this.value < 0x800000L) ? 5 : 6;
                }

                if (this.value < 0x800000000000L)
                {
                    return (this.value < 0x8000000000L) ? 7 : 8;
                }

                return (this.value < 0x80000000000000L) ? 9 : 10;
            }

        }

        /// <summary>
        /// Encodes this <see cref="Counter64"/> to a BER Stream
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeUnsignedInt64(outputStream, BER.COUNTER64, value);
        }

        /// <summary>
        /// Decodes the <see cref="Counter64"/> value from a BER Stream
        /// </summary>
        /// <param name="inputStream">The stream to read from</param>
        public override void DecodeBER(BERInputStream inputStream)
        {
            BER.MutableByte type = new BER.MutableByte();
            long newValue = BER.DecodeUnsignedInt64(inputStream, out type);
            if (type.Value != BER.COUNTER64)
            {
                throw new IOException("Wrong type encountered when decoding Counter64: " +
                                      type.Value);
            }

            this.SetValue(newValue);
        }

        public override int GetHashCode()
        {
            return (int)this.value;
        }


        public override bool Equals(object o)
        {
            return (o is Counter64) && ((Counter64)o).value == this.value;
        }

        public override int CompareTo(IVariable o)
        {
            long other = ((Counter64)o).value;
            for (int i = 63; i >= 0; i--)
            {
                if (((this.value >> i) & 1) !=
                    ((other >> i) & 1))
                {
                    if (((this.value >> i) & 1) != 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }

        public override string ToString()
        {
            if ((this.value > 0) && (this.value < long.MaxValue))
            {
                return this.value.ToString();
            }

            ulong u64 = unchecked((ulong)this.value);
            return u64.ToString();
        }

        public void SetValue(string value)
        {
            this.value = long.Parse(value);
        }

        public void SetValue(long value)
        {
            this.value = value;
        }

        public long GetValue()
        {
            return this.value;
        }

        public override object Clone()
        {
            return new Counter64(this.value);
        }

        /// <summary>
        /// Increment the value of the counter by one. If the current value is
        /// 2^63-1 (9223372036854775807) then value will be set to -2^63. Nevertheless,
        /// the BER encoded value of this counter will always be unsigned!
        /// </summary>
        public void Increment()
        {
            this.value++;
        }

        /**
         * Increment the value by more than one in one step.
         * @param increment
         *   an increment value greater than zero.
         * @return
         *   the current value of the counter.
         * @since 2.4.2
         */
        public long Increment(long increment)
        {
            if (increment < 0)
            {
                throw new ArgumentException("Counter64 allows only positive increments: " + increment);
            }

            return this.value += increment;
        }

        public override OID ToSubIndex(bool impliedLength)
        {
            throw new NotSupportedException();
        }

        public override void FromSubIndex(OID subIndex, bool impliedLength)
        {
            throw new NotSupportedException();
        }
    }
}
