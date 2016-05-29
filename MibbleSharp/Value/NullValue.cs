// <copyright file="NullValue.cs" company="None">
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
    
    /// <summary>
    /// A null MIB value.
    /// </summary>
    public class NullValue : MibValue
    {
        /// <summary>
        /// The one and only null value instance.
        /// </summary>
        public static readonly NullValue NULL = new NullValue();

        /// <summary>
        /// Prevents a default instance of the <see cref="NullValue"/> class from
        /// being created, as only the NULL static instance should be used.
        /// </summary>
        private NullValue() : base("NULL")
        {
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
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <returns>The NULL value</returns>
        public override MibValue CreateReference()
        {
            return NullValue.NULL;
        }
        
        /// <summary>
        /// Checks if this object equals another object. This method will
        /// compare the string representations for equality.
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if the objects are equal, false if not</returns>
        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj.ToString());
        }

        /// <summary>
        /// Returns a hash code for this object.
        /// </summary>
        /// <returns>A hash code for this object.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Returns a Java null representation of this value.
        /// </summary>
        /// <returns>A Java null representation of this value.</returns>
        public object ToObject()
        {
            return null;
        }

        /// <summary>
        /// Returns a string representation of this value.
        /// </summary>
        /// <returns>A string representation of this value.</returns>
        public override string ToString()
        {
            return "NULL";
        }

        /// <summary>
        /// Compares the value to another MibValue
        /// </summary>
        /// <param name="other">The MibValue to compare against</param>
        /// <returns>
        /// 0 if the objects are equal, a positive or negative integer 
        /// if they are different
        /// </returns>
        public override int CompareTo(MibValue other)
        {
            throw new NotImplementedException();
        }
    }
}
