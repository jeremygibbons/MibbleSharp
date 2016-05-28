// <copyright file="ValueConstraint.cs" company="None">
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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
//    USA</para>
//    Original Java code Copyright (c) 2004-2016 Per Cederberg. All
//    rights reserved.
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
// </copyright>

namespace MibbleSharp.Type
{
    using MibbleSharp.Value;

    /// <summary>
    /// A MIB type value constraint. This class represents a single value
    /// in a set of value constraints.
    /// </summary>
    public class ValueConstraint : IConstraint
    {
        /// <summary>
        /// The constraint location. This value is reset to null once the
        /// constraint has been initialized. 
        /// </summary>
        private FileLocation location;

        /// <summary>
        /// The constraint value.
        /// </summary>
        private MibValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConstraint"/> class.
        /// </summary>
        /// <param name="location">The constraint location</param>
        /// <param name="value">The constraint value</param>
        public ValueConstraint(FileLocation location, MibValue value)
        {
            this.location = location;
            this.value = value;
        }

        /// <summary>
        /// Gets the constraint value
        /// </summary>
        public MibValue Value
        {
            get
            {
                return this.value;
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
        /// <param name="type">The MIB type</param>
        /// <param name="log">The MIB loader log</param>
        /// <exception cref="MibException">
        /// If an error is encountered during initialization
        /// </exception>
        public void Initialize(MibType type, MibLoaderLog log)
        {
            string message;

            this.value = this.value.Initialize(log, type);
            if (this.location != null && !this.IsCompatible(type))
            {
                message = "Value constraint not compatible with this type";
                log.AddWarning(this.location, message);
            }

            this.location = null;
        }

        /// <summary>
        /// Checks if the specified type is compatible with this
        /// constraint.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if the type is compatible, false if not</returns>
        public bool IsCompatible(MibType type)
        {
            return type == null || this.value == null || type.IsCompatible(this.value);
        }

        /// <summary>
        /// Checks if the specified value is compatible with this
        /// constraint.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, or
        /// false otherwise
        /// </returns>
        public bool IsCompatible(MibValue value)
        {
            string str1 = this.value.ToString();
            string str2 = value.ToString();

            if (this.value is NumberValue
                && value is NumberValue)
            {
                return str1.Equals(str2);
            }
            else if (this.value is StringValue
                && value is StringValue)
            {
                return str1.Equals(str2);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a string representation of this type.
        /// </summary>
        /// <returns>A string representation of this type</returns>
        public override string ToString()
        {
            return this.value.ToString();
        }
    }
}