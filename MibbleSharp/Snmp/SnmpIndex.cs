// <copyright file="SnmpIndex.cs" company="None">
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

namespace MibbleSharp.Snmp
{
    using System;
    using System.Text;

    /// <summary>
    /// An SNMP index object. This declaration is used inside an object
    /// type index declaration.An index contains either a type or a
    /// value. Indices based on values may be implied.
    /// </summary>
    /// <see cref="SnmpObjectType"/>
    public class SnmpIndex
    {
        /// <summary>The implied flag</summary>
        private bool implied;

        /// <summary>
        /// The index value, or null.
        /// </summary>
        private MibValue value;

        /// <summary>
        /// The index type, or null.
        /// </summary>
        private MibType type;

        /// <summary>Initializes a new instance of the <see cref="SnmpIndex"/> class.
        /// Exactly one of the value or type arguments are supposed to be non-null.
        /// </summary> 
        /// <param name="implied">The implied flag</param>
        /// <param name="value">The index value, or null</param>
        /// <param name="type">The index type, or null</param>
        public SnmpIndex(bool implied, MibValue value, MibType type)
        {
            this.implied = implied;
            this.value = value;
            this.type = type;
        }

        /// <summary>
        /// Gets a value indicating whether this index is an implied value. If this is true, the
        /// index also represents a value index.
        /// </summary>
        public bool Implied
        {
            get
            {
                return this.implied;
            }
        }

        /// <summary>
        /// Gets the index value
        /// </summary>
        /// <see cref="MibValue"/>
        public MibValue Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Gets the index type
        /// </summary>
        /// <see cref="MibType"/>
        public MibType Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Initializes the object. This will remove all levels of
        /// indirection present, such as references to other types and
        /// values.No information is lost by this operation.This method
        /// may modify this object as a side-effect, and will be called by
        /// the MIB loader.
        /// </summary>
        /// <param name="symbol">the MIB symbol containing this object</param>
        /// <param name="log">the MIB loader log</param>
        /// <exception cref="MibException">
        /// If an error was encountered during the initialization
        /// </exception>
        public void Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            if (this.value != null)
            {
                this.value = this.value.Initialize(log, null);
            }

            if (this.type != null)
            {
                this.type = this.type.Initialize(symbol, log);
            }
        }

        /// <summary>Returns a string representation of this object.</summary>
        /// <returns>a string representation of this object</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (this.implied)
            {
                builder.Append("IMPLIED ");
            }

            if (this.type != null)
            {
                builder.Append(this.type);
            }
            else
            {
                builder.Append(this.value);
            }

            return builder.ToString();
        }
    }
}
