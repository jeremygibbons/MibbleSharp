// <copyright file="ValueRangeConstraint.cs" company="None">
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
    using System.Numerics;
    using System.Text;
    using MibbleSharp.Value;

    /// <summary>
    /// A MIB type value range constraint. This class represents a value
    /// range in a set of value constraints.
    /// </summary>
    public class ValueRangeConstraint : IConstraint
    {
        /// <summary>
        /// The constraint location. This value is reset to null once the
        /// constraint has been initialized.
        /// </summary>
        private FileLocation location;

        /// <summary>
        /// The lower bound value.
        /// </summary>
        private NumberValue lower;

        /// <summary>
        /// /The upper bound value.
        /// </summary>
        private NumberValue upper;

        /// <summary>
        /// The strict lower bound flag.
        /// </summary>
        private bool strictLower;

        /// <summary>
        /// The strict upper bound flag.
        /// </summary>
        private bool strictUpper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueRangeConstraint"/> class.
        /// </summary>
        /// <param name="location">The constraint location</param>
        /// <param name="lower">The lower bound or null for minimum</param>
        /// <param name="strictLower">The strict lower bound (less than) flag</param>
        /// <param name="upper">The upper bound, or null for maximum</param>
        /// <param name="strictUpper">The strict upper bound (greater than) flag</param>
        public ValueRangeConstraint(
            FileLocation location,
            NumberValue lower,
            bool strictLower,
            NumberValue upper,
            bool strictUpper)
        {
            this.location = location;
            this.lower = lower;
            this.upper = upper;
            this.strictLower = strictLower;
            this.strictUpper = strictUpper;
        }
        
        /// <summary>
        /// Gets the lower bound value
        /// </summary>
        public MibValue LowerBound
        {
            get
            {
                return this.lower;
            }
        }
                
       /// <summary>
       /// Gets the upper bound value
       /// </summary>
        public MibValue UpperBound
        {
            get
            {
                return this.upper;
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

            if (this.lower != null)
            {
                this.lower.Initialize(log, type);
            }

            if (this.upper != null)
            {
                this.upper.Initialize(log, type);
            }

            if (this.location != null && !this.IsCompatible(type))
            {
                message = "Value range constraint not compatible with " +
                          "this type";
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
            return (type == null || this.lower == null || type.IsCompatible(this.lower))
                && (type == null || this.upper == null || type.IsCompatible(this.upper));
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
            return (this.lower == null || this.IsLessThan(this.strictLower, this.lower, value))
                && (this.upper == null || this.IsLessThan(this.strictUpper, value, this.upper));
        }

        /// <summary>
        /// Checks if the specified value is compatible with this
        /// constraint.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public bool IsCompatible(BigInteger value)
        {
            return (this.lower == null || this.IsLessThan(this.strictLower, this.lower.value, value))
                && (this.upper == null || this.IsLessThan(this.strictUpper, value, this.upper.value));
        }

        /// <summary>
        /// Returns a string representation of this type.
        /// </summary>
        /// <returns>A string representation of this type</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (this.lower == null)
            {
                builder.Append("MIN");
            }
            else
            {
                builder.Append(this.lower);
            }

            if (this.strictLower)
            {
                builder.Append("<");
            }

            builder.Append("..");

            if (this.strictUpper)
            {
                builder.Append("<");
            }

            if (this.upper == null)
            {
                builder.Append("MAX");
            }
            else
            {
                builder.Append(this.upper);
            }

            return builder.ToString();
        }
        
        /// <summary>
        /// Checks if one MIB value is less than another.
        /// </summary>
        /// <param name="strict">The strict less than flag</param>
        /// <param name="value1">The first value</param>
        /// <param name="value2">The second value</param>
        /// <returns>
        /// True if the first value is less than the second,
        /// false if not
        /// </returns>
        private bool IsLessThan(
            bool strict,
            MibValue value1,
            MibValue value2)
        {
            NumberValue nv1 = value1 as NumberValue;
            NumberValue nv2 = value2 as NumberValue;

            if (value1 != null && value2 != null)
            {
                return this.IsLessThan(
                    strict,
                    nv1.value,
                    nv2.value);
            }

            StringValue s1 = value1 as StringValue;
            StringValue s2 = value2 as StringValue;

            if (s1 != null && s2 != null)
            {
                return this.IsLessThan(
                    strict,
                    s1.ToString(),
                    s2.ToString());
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a number is less than another.
        /// </summary>
        /// <param name="strict">The strict less than flag</param>
        /// <param name="value1">The first value</param>
        /// <param name="value2">The second value</param>
        /// <returns>
        /// True if the first value is less than the second,
        /// false if not
        /// </returns>
        private bool IsLessThan(
            bool strict,
            BigInteger value1,
            BigInteger value2)
        {
            return strict ? (value1 < value2) : (value1 <= value2);
        }
        
        /// <summary>
        /// Checks if a string is less than another.
        /// </summary>
        /// <param name="strict">The strict less than flag</param>
        /// <param name="value1">The first value</param>
        /// <param name="value2">The second value</param>
        /// <returns>
        /// True if the first value is less than the second,
        /// false if not
        /// </returns>
        private bool IsLessThan(
            bool strict,
            string value1,
            string value2)
        {
            if (strict)
            {
                return value1.CompareTo(value2) < 0;
            }
            else
            {
                return value1.CompareTo(value2) <= 0;
            }
        }
    }
}