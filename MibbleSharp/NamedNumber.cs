// <copyright file="NamedNumber.cs" company="None">
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

namespace MibbleSharp
{
    using System.Numerics;
    using MibbleSharp.Value;

    /// <summary>
    /// A named number. This class is used for storing intermediate values
    /// during the parsing.
    /// </summary>
    public class NamedNumber
    {
        /// <summary>
        /// The value name
        /// </summary>
        private string name = null;

        /// <summary>
        /// The numeric value
        /// </summary>
        private BigInteger? number = null;

        /// <summary>
        /// The value reference
        /// </summary>
        private ValueReference reference = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedNumber"/> class.
        /// </summary>
        /// <param name="number">The numeric value</param>
        public NamedNumber(BigInteger number) : this(null, number)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedNumber"/> class.
        /// </summary>
        /// <param name="name">The value name</param>
        /// <param name="number">The numeric value</param>
        public NamedNumber(string name, BigInteger number)
        {
            this.name = name;
            this.number = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedNumber"/> class.
        /// </summary>
        /// <param name="reference">A reference to the represented value</param>
        public NamedNumber(ValueReference reference) : this(null, reference)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedNumber"/> class.
        /// </summary>
        /// <param name="name">The named number's name</param>
        /// <param name="reference">A reference to the value</param>
        public NamedNumber(string name, ValueReference reference)
        {
            this.name = name;
            this.reference = reference;
        }

        /// <summary>
        /// Gets a value indicating whether this named number has a name component
        /// </summary>
        public bool HasName
        {
            get
            {
                return this.name != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this named number has a number component
        /// </summary>
        public bool HasNumber
        {
            get
            {
                return this.number != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this named number has a value reference
        /// </summary>
        public bool HasReference
        {
            get
            {
                return this.reference != null;
            }
        }

        /// <summary>
        /// Gets the name of this value
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the value of this number
        /// </summary>
        public BigInteger Number
        {
            get
            {
                if (this.number == null)
                {
                    return 0;
                }
                    
                return (BigInteger)this.number;
            }
        }

        /// <summary>
        /// Gets an integer value of the number
        /// </summary>
        /// <exception cref="System.OverflowException">
        /// If the number is larger than can fit in an <c>Int32</c>
        /// </exception>
        public int IntValue
        {
            get
            {
                if (this.number > int.MaxValue)
                {
                    throw new System.OverflowException();
                }
                    
                return (int)this.number;
            }
        }

        /// <summary>
        /// Gets the value reference
        /// </summary>
        public ValueReference Reference
        {
            get
            {
                return this.reference;
            }
        }
    }
}