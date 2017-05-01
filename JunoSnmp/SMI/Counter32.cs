// <copyright file="Counter32.cs" company="None">
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
    /// The <c>Counter32</c> class allows all the functionality of unsigned
    /// integers but is recognized as a distinct SMI type, which is used for
    /// monotonically increasing values that wrap around at 2^32-1 (4294967295).
    /// </summary>
    public class Counter32 : UnsignedInteger32, IEquatable<Counter32>
    {
        public static readonly long MaxCounter32Value = 4294967295L;

        /// <summary>
        /// Initializes a new instance of the <see cref="Counter32"/> class with a zero value.
        /// </summary>
        public Counter32()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Counter32"/> class with a given value
        /// </summary>
        /// <param name="value">The initial value</param>
        public Counter32(long value) : base(value)
        {
        }

        /// <summary>
        /// Gets the syntax for this <see cref="IVariable"/>
        /// </summary>
        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxCounter32;
            }
        }

        /// <summary>
        /// Tests if this <see cref="Counter32"/> is equal to another object.
        /// </summary>
        /// <param name="o">An object to test against</param>
        /// <returns>
        /// True if both objects are <see cref="Counter32"/>s and have the same value,
        /// False if not.
        /// </returns>
        public override bool Equals(object o)
        {
            return (o is Counter32 c) ?  this.Equals(c) : false;
        }

        /// <summary>
        /// Tests if this <see cref="Counter32"/> is equal to another Counter32.
        /// </summary>
        /// <param name="o">An object to test against</param>
        /// <returns>
        /// True if both objects have the same value,
        /// False if not.
        /// </returns>
        public bool Equals(Counter32 o)
        {
            return o.GetValue() == this.GetValue();
        }

        /// <summary>
        /// Gets a hashcode for this object
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        /// <remarks>Implemented here because Equals is also overridden</remarks>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Encodes this <see cref="Counter32"/> to a BER output stream
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeUnsignedInteger(outputStream, BER.COUNTER32, this.GetValue());
        }

        /// <summary>
        /// Reads a value for this <see cref="Counter32"/> from a BER input stream
        /// </summary>
        /// <param name="inputStream">The input stream to read from</param>
        public override void DecodeBER(BERInputStream inputStream)
        {
            long newValue = BER.DecodeUnsignedInteger(inputStream, out byte type);

            if (type != BER.COUNTER32)
            {
                throw new IOException("Wrong type encountered when decoding Counter: " +
                                      type);
            }

            this.SetValue(newValue);
        }

        /// <summary>
        /// Creates a copy of this <see cref="Counter32"/>
        /// </summary>
        /// <returns>A new copy of this counter</returns>
        public override object Clone()
        {
            return new Counter32(value);
        }

        /// <summary>
        /// Increment the value of the counter by one. If the current value is
        /// 2^32-1 (4294967295) then value will be set to zero.
        /// </summary>
        public void Increment()
        {
            if (this.value < Counter32.MaxCounter32Value)
            {
                this.value++;
            }
            else
            {
                this.value = 0;
            }
        }
        
        /// <summary>
        /// Increment the value by a given amount
        /// </summary>
        /// <param name="increment">A positive increment value, as a long</param>
        /// <returns>The updated value of the counter</returns>
        public long Increment(long increment)
        {
            if (increment > 0)
            {
                if (this.value + increment < Counter32.MaxCounter32Value)
                {
                    this.value += increment;
                }
                else
                {
                    this.value = increment - (Counter32.MaxCounter32Value - this.value);
                }
            }
            else if (increment < 0)
            {
                throw new ArgumentException("Negative increments not allowed for counters: " + increment);
            }

            return this.value;
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
            throw new NotSupportedException();
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
            throw new NotSupportedException();
        }
    }
}
