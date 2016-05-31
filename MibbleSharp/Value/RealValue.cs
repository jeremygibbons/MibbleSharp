// <copyright file="RealValue.cs" company="None">
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
    using MibbleSharp.Type;

    /// <summary>
    /// A real MIB value, used to represent special values such as
    /// plus and minus infinity
    /// </summary>
    public class RealValue : MibValue
    {
        /// <summary>
        /// The numeric real value, stored as a double as the only values 
        /// being used are special double values (infinity)
        /// </summary>
        private double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RealValue"/> class.
        /// </summary>
        /// <param name="value">The real value</param>
        public RealValue(double value) : base("Real")
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value of the RealValue as a double
        /// </summary>
        public double Value
        {
            get
            {
                return this.value;
            }
        }
        
        /// <summary>
        /// Initializes the MIB value. This will remove all levels of
        /// indirection present, such as references to other values. No
        /// value information is lost by this operation. This method may
        /// modify this object as a side-effect, and will return the basic
        /// value.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="log">The Mib Loader log</param>
        /// <param name="type">The value type</param>
        /// <returns>The MIB value</returns>
        public override MibValue Initialize(MibLoaderLog log, MibType type)
        {
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
            return new RealValue(this.value);
        }

        /// <summary>
        /// Compare this value to another MIB Value
        /// </summary>
        /// <param name="other">The MIB Value to compare against</param>
        /// <returns>
        /// Zero (0) if the objects are equal, an non-zero integer value if not
        /// </returns>
        public override int CompareTo(MibValue other)
        {
            NumberValue nv = other as NumberValue;
            if (nv != null)
            {
                return this.CompareToNumber((double)nv.Value);
            }

            return this.ToString().CompareTo(other);
        }

        /// <summary>
        /// Checks if this object equals another object. This method will
        /// compare the string representations for equality.
        /// </summary>
        /// <param name="obj">The object to compare with</param>
        /// <returns>True if the objects are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            MibValue mv = obj as MibValue;
            if (mv == null)
            {
                return false;
            }

            return this.CompareTo(mv) == 0;
        }

        /// <summary>
        /// Returns a hash code for this object.
        /// </summary>
        /// <returns>
        /// Returns a hash code for this object
        /// </returns>
        public override int GetHashCode()
        {
            return (int)this.value;
        }

        /// <summary>
        /// Returns a string representation of this value.
        /// </summary>
        /// <returns>A string representation of this value</returns>
        public override string ToString()
        {
            return this.value.ToString();
        }

        /// <summary>
        /// Returns the number of bytes required by the specified type and
        /// initial value size. If the type has no size requirement
        /// specified, a value of one (1) will always be returned. If the
        /// type size constraint allows for zero length, a zero might also
        /// be returned.
        /// </summary>
        /// <param name="type">The MIB value type</param>
        /// <param name="initialBytes">The initial number of bytes used</param>
        /// <returns>The number of bytes required</returns>
        protected static int GetByteSize(MibType type, int initialBytes)
        {
            IConstraint c = null;
            int res = -1;

            if (type is StringType)
            {
                c = ((StringType)type).Constraint;
            }

            if (c is SizeConstraint)
            {
                res = ((SizeConstraint)c).NextValue(initialBytes);
            }

            if (res < 0)
            {
                res = 1;
            }

            return res;
        } 
        
        /// <summary>
        /// Compares this object with the specified number for order.
        /// </summary>
        /// <param name="num">The number to compare to</param>
        /// <returns>
        /// Less than zero if this number is less than the specified,
        /// Zero if the numbers are equal, or
        /// Greater than zero otherwise
        /// </returns>
        private int CompareToNumber(double num)
        {
            return (int)(this.value - num);
        }
    }
}
