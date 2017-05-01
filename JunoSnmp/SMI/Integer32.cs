// <copyright file="Integer32.cs" company="None">
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
    /// The <c>Integer32</c> represents 32bit signed integer values for SNMP.
    /// </summary>
    public class Integer32 : AbstractVariable, IAssignableFrom<int>, IAssignableFrom<string>, IEquatable<Integer32>
    {

        private int value = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Integer32"/> class with a zero value.
        /// </summary>
        public Integer32()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Integer32"/> class with a supplied value.
        /// </summary>
        /// <param name="val">An integer value</param>
        public Integer32(int val)
        {
            this.SetValue(val);
        }

        /// <summary>
        /// Encodes this Integer32 to a BER stream
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeInteger(outputStream, BER.INTEGER, value);
        }

        /// <summary>
        /// Set the value of this <see cref="Integer32"/>from a BER input stream
        /// </summary>
        /// <param name="inputStream">The input stream to read from</param>
        public override void DecodeBER(BERInputStream inputStream)
        {
            int newValue = BER.DecodeInteger(inputStream, out byte type);

            if (type != BER.INTEGER)
            {
                throw new IOException("Wrong type encountered when decoding Counter: " + type);
            }

            this.SetValue(newValue);
        }

        /// <summary>
        /// Gets the syntax of this variable
        /// </summary>
        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxInteger;
            }
        }

        /// <summary>
        /// Gets a hashcode for this object
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        public override int GetHashCode()
        {
            return this.value;
        }

        /// <summary>
        /// Gets the length of the BER encoding of this <see cref="Integer32"/>
        /// </summary>
        public override int BERLength
        {
            get
            {
                if ((this.value < 0x80) && (this.value >= -0x80))
                {
                    return 3;
                }
                else if ((this.value < 0x8000) && (this.value >= -0x8000))
                {
                    return 4;
                }
                else if ((this.value < 0x800000) && (this.value >= -0x800000))
                {
                    return 5;
                }

                return 6;
            }
        }

        /// <summary>
        /// Checks whether this object and another are equal
        /// </summary>
        /// <param name="o">The object to compare with</param>
        /// <returns>True if the object are equal-valued Integer32 objects, false if not</returns>
        public override bool Equals(object o)
        {
            return (o is Integer32) && (((Integer32)o).value == this.value);
        }

        /// <summary>
        /// Checks whether this object and another Integer32 object are equal
        /// </summary>
        /// <param name="other">The Integer32 to compare with</param>
        /// <returns>True if the object are equal-valued Integer32 objects, false if not</returns>
        public bool Equals(Integer32 other)
        {
            return other.value == this.value;
        }

        /// <summary>
        /// Compares this object with another IVariable
        /// </summary>
        /// <param name="o">The object to compare against</param>
        /// <returns>
        /// If <c>o</c> is an Integer32, the value is 0 if both are equal, 
        /// a positive int if this object's value is greater than <c>o</c>'s,
        /// a negative int if not
        /// </returns>
        public override int CompareTo(IVariable o)
        {
            return value - ((Integer32)o).value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public void SetValue(string value)
        {
            this.value = int.Parse(value);
        }

        /**
         * Sets the value of this integer.
         * @param value
         *    an integer value.
         */
        public void SetValue(int value)
        {
            this.value = value;
        }

        /**
         * Gets the value.
         * @return
         *    an integer.
         */
        public int GetValue()
        {
            return this.value;
        }

        public override object Clone()
        {
            return new Integer32(value);
        }

        public override int IntValue
        {
            get
            {
                return this.GetValue();
            }
        }

        public override long LongValue
        {
            get
            {
                return this.GetValue();
            }
        }

        /// <summary>
        /// Converts the value of this <c>Variable</c> to a (sub-)index value
        /// </summary>
        /// <param name="impliedLength">
        /// Specifies if the sub-index has an implied length. This parameter applies
        /// to variable length variables only(e.g. { @link OctetString}
        /// and <see cref="OID"/>. For other variables it has no effect.
        /// </param>
        /// <returns>An OID that represents this value as a sub index</returns>
        /// <exception cref="NotSupportedException">If this variable cannot be used as a sub index</exception>
        public override OID ToSubIndex(bool impliedLength)
        {
            return new OID(new long[] { this.value });
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
            this.SetValue((int)subIndex[0]);
        }
    }
}
