// <copyright file="BinaryNumberValue.cs" company="None">
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
    using System.Numerics;
    using System.Text;
    using MibbleSharp.Util;

    /// <summary>
    /// A binary numeric MIB value.
    /// </summary>
    public class BinaryNumberValue : NumberValue
    {
        /// <summary>
        /// The minimum number of bits to print.
        /// </summary>
        private int minLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryNumberValue"/> class. 
        /// A default minimum print length of one(1) will be used.
        /// </summary>
        /// <param name="value">The numeric value</param>
        public BinaryNumberValue(BigInteger value) : this(value, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryNumberValue"/> class.
        /// </summary>
        /// <param name="value">The numeric value</param>
        /// <param name="minLength">The minimum print length</param>
        public BinaryNumberValue(BigInteger value, int minLength) : base(value)
        {
            this.minLength = minLength;
        }
         
        /// <summary>
        /// Gets a binary representation of this value.
        /// </summary>
        private string ToBinaryString
        {
            get
            {
                return this.Value.ToBinaryString();
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
        public override MibValue Initialize(MibLoaderLog log, MibType type)
        {
            int bytes = (this.minLength / 8) + ((this.minLength % 8 > 0) ? 1 : 0);
            int length = NumberValue.GetByteSize(type, bytes) * 8;
            if (length > this.minLength)
            {
                this.minLength = length;
            }

            return this;
        }

        /// <summary>
        /// Returns a string representation of this value.
        /// </summary>
        /// <returns>
        /// A string representation of this value.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string val;

            builder.Append("'");
            val = this.ToBinaryString;

            if (val.Equals("0"))
            {
                val = string.Empty;
            }

            for (int i = val.Length; i < this.minLength; i++)
            {
                builder.Append("0");
            }

            builder.Append(val);

            builder.Append("'B");

            return builder.ToString();
        }
    }
}