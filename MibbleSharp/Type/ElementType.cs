// <copyright file="ElementType.cs" company="None">
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

namespace MibbleSharp.Type
{
    using System.Text;

    /// <summary>
    /// A compound element MIB type. This type is used inside various
    /// compound types, storing a reference to the type and an optional
    /// name.
    /// </summary>
    public class ElementType : MibType
    {
        /// <summary>
        /// The element type.
        /// </summary>
        private MibType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementType"/> class.
        /// </summary>
        /// <param name="name">The optional element name</param>
        /// <param name="type">The element type</param>
        public ElementType(string name, MibType type) : base(string.Empty, false)
        {
            this.Name = name;
            this.type = type;
        }

        /// <summary>Gets the referenced MIB type.</summary>
        /// <returns>The referenced MIB type</returns>
        public MibType Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Initializes the MIB type. This will remove all levels of
        /// indirection present, such as references to types or values. No
        /// information is lost by this operation. This method may modify
        /// this object as a side-effect, and will return the basic
        /// type.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="symbol">The MIB symbol containing this type</param>
        /// <param name="log">The MIB loader log</param>
        /// <returns>The basic MIB type</returns>
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            this.type = this.type.Initialize(symbol, log);
            return this;
        }

        /// <summary>
        /// Checks if the specified value is compatible with this type.
        /// The value is considered compatible with this type, if it is
        /// compatible with the underlying type.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>true if the value is compatible, false otherwise</returns>
        public override bool IsCompatible(MibValue value)
        {
            return this.type.IsCompatible(value);
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(base.ToString());

            if (this.Name != null)
            {
                builder.Append(this.Name);
                builder.Append(" ");
            }

            builder.Append(this.type.ToString());
            return builder.ToString();
        }
    }
}