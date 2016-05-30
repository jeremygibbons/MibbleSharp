// <copyright file="BitSetValue.cs" company="None">
//    <para>
//    This work is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published
//    by the Free Software Foundation; either version 2 of the License,
//    or (at your option) any later version.</para>
//    <para>
//    This work is distributed in the hope that it will be useful, but
//    WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    General Public License for more details.</para>
//    <para>
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
//    USA</para>
//    Original Java code Copyright (c) 2004-2016 Per Cederberg. All
//    rights reserved.
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
// </copyright>

namespace MibbleSharp.Value
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Text;
    
    /// <summary>
    /// A bit set MIB value.
    /// </summary>
    public class BitSetValue : MibValue
    {
        /// <summary>
        /// The bit set value.
        /// </summary>
        private BitArray value;

        /// <summary>
        /// The additional value references.
        /// </summary>
        private IList<ValueReference> references;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitSetValue"/> class.
        /// </summary>
        /// <param name="value">The bit set value</param>
        public BitSetValue(BitArray value) : this(value, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitSetValue"/> class.
        /// </summary>
        /// <param name="value">The bit set value</param>
        /// <param name="references">The additional referenced bit values</param>
        public BitSetValue(BitArray value, IList<ValueReference> references) : base("BIT STRING")
        {
            this.value = value;
            this.references = references;
        }

        /// <summary>
        /// Gets all the bits in this bit set as individual number
        /// values.
        /// </summary> 
        public IList<NumberValue> Bits
        {
            get
            {
                IList<NumberValue> components = new List<NumberValue>();

                foreach (bool b in this.value)
                {
                    components.Add(new NumberValue((BigInteger)(b ? 1 : 0)));
                }

                return components;
            }
        }

        /// <summary>
        /// Initializes the MIB value. This will remove all levels of
        /// indirection present, such as references to other values. No
        /// value information is lost by this operation. This method may
        /// modify this object as a side-effect, and will return the basic
        /// value.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should only be called by 
        /// the MIB loader.
        /// </remarks>
        /// <param name="log">The MIB loader log</param>
        /// <param name="type">The value type</param>
        /// <returns>The basic MIB value</returns>
        /// <exception cref="MibException">If an error occurs during initialization</exception>
        public override MibValue Initialize(MibLoaderLog log, MibType type)
        {
            if (this.references != null)
            {
                foreach (ValueReference vref in this.references)
                {
                    this.Initialize(log, type, vref);
                }

                this.references = null;
            }

            return this;
        }

        /// <summary>
        /// Creates a value reference to this value. The value reference
        /// is normally an identical value. Only certain values support
        /// being referenced, and the default implementation of this
        /// method throws an exception.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        /// <returns>The MIB value reference</returns>
        public override MibValue CreateReference()
        {
            return new BitSetValue(this.value, this.references);
        }

        /// <summary>
        /// Clears and prepares this value for garbage collection. This
        /// method will recursively clear any associated types or values,
        /// making sure that no data structures references this object.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        public override void Clear()
        {
            base.Clear();
            this.value = null;
            this.references = null;
        }

        /// <summary>
        /// Checks if this object equals another object. This method will
        /// compare the string representations for equality.
        /// </summary>
        /// <param name="obj">The object to compare with</param>
        /// <returns>True if the objects are equal, false if not</returns>
        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj.ToString());
        }

        /// <summary>
        /// Returns a hash code for this object.
        /// </summary>
        /// <returns>
        /// A hash code for this object
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this value.
        /// </summary>
        /// <returns>
        /// A string representation of this value
        /// </returns>
        public override string ToString()
        {
            return this.value.ToString();
        }

        /// <summary>
        /// Compares this object to a MibValue
        /// </summary>
        /// <param name="other">The MIB Value to compare against</param>
        /// <returns>
        /// Zero (0) if the objects are equal, an non-zero integer value if not
        /// </returns>
        public override int CompareTo(MibValue other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an ASN.1 representation of this value. The string will
        /// contain named references to any values that can be found in the
        /// specified list.
        /// </summary>
        /// <param name="values">The defined symbol values</param>
        /// <returns>An ASN.1 representation of this value</returns>
        public string ToAsn1String(MibValueSymbol[] values)
        {
            StringBuilder builder = new StringBuilder();

            foreach (bool b in this.value)
            {
                if (b)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }

                    builder.Append(this.ToAsn1String(b, values));
                }
            }

            if (builder.Length > 0)
            {
                return "{ " + builder.ToString() + " }";
            }
            else
            {
                return "{}";
            }
        }

        /// <summary>
        /// Returns an ASN.1 representation of a bit number. The value
        /// name will be returned if found in the specified array.
        /// </summary>
        /// <param name="bit">The bit number</param>
        /// <param name="values">The defined bit names</param>
        /// <returns>
        /// The ASN.1 representation of the bit number
        /// </returns>
        private string ToAsn1String(bool bit, MibValueSymbol[] values)
        {
            foreach (MibValueSymbol s in values)
            {
                if (s.Value.Equals(bit))
                {
                    return s.Name;
                }
            }

            return bit.ToString();
        }

        /// <summary>
        /// Initializes a the MIB value from a value reference. This will
        /// resolve the reference, and set the bit corresponding to the
        /// value.
        /// </summary>
        /// <param name="log">The MIB loader log</param>
        /// <param name="type">The value type</param>
        /// <param name="vref">The value reference to resolve</param>
        /// <exception cref="MibException">
        /// If an error occurred during initialization
        /// </exception> 
        private void Initialize(MibLoaderLog log, MibType type, ValueReference vref)
        {
            MibValue val = vref.Initialize(log, type);

            NumberValue nv = val as NumberValue;

            if (nv != null)
            {
                this.value.Set((int)nv.Value, true);
            }
            else
            {
                throw new MibException(
                    vref.Location,
                    "referenced value is not a number");
            }
        }
    }
}