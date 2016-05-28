// <copyright file="StringType.cs" company="None">
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
    using System.Text;
    using MibbleSharp.Value;

    /// <summary>
    /// A string MIB type.
    /// </summary>
    public class StringType : MibType
    {
        /// <summary>
        /// The additional type constraint.
        /// </summary>
        private IConstraint constraint = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringType"/> class.
        /// </summary>
        public StringType() : this(true, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringType"/> class.
        /// </summary>
        /// <param name="constraint">The additional type constraint</param>
        public StringType(IConstraint constraint) : this(true, constraint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringType"/> class.
        /// </summary>
        /// <param name="primitive">The primitive type flag</param>
        /// <param name="constraint">The type constraint, or null</param>
        private StringType(bool primitive, IConstraint constraint)
                : base("OCTET STRING", primitive)
        {
            this.constraint = constraint;
        }

        /// <summary>
        /// Gets the optional type constraint. The type constraint for
        /// a string will typically be a size constraint.
        /// </summary>
        public IConstraint Constraint
        {
            get
            {
                return this.constraint;
            }
        } 
        
        /// <summary>
        /// Gets a value indicating whether this type has a constraint
        /// </summary>
        public bool HasConstraint
        {
            get
            {
                return this.constraint != null;
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
            this.SetTag(true, MibTypeTag.OctetString);
            if (this.constraint != null)
            {
                this.constraint.Initialize(this, log);
            }

            return this;
        }
        
        /// <summary>
        /// Creates a type reference to this type. The type reference is
        /// normally an identical type, but with the primitive flag set to
        /// false. Only certain types support being referenced, and the
        /// default implementation of this method throws an exception.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <returns>The MIB type reference</returns>
        public override MibType CreateReference()
        {
            StringType type = new StringType(false, this.constraint);
            type.SetTag(true, this.Tag);
            return type;
        }

        /// <summary>
        /// Creates a constrained type reference to this type. The type
        /// reference is normally an identical type, but with the
        /// primitive flag set to false. Only certain types support being
        /// referenced, and the default implementation of this method
        /// throws an exception.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="constraint">The type constraint</param>
        /// <returns>The MIB type reference</returns>
        public override MibType CreateReference(IConstraint constraint)
        {
            StringType type = new StringType(false, constraint);

            type.SetTag(true, this.Tag);
            return type;
        }

        /// <summary>
        /// Checks if the specified value is compatible with this type.  A
        /// value is compatible if it is a string value that is compatible
        /// with the constraints.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, or
        /// false otherwise
        /// </returns>
        public override bool IsCompatible(MibValue value)
        {
            return this.IsCompatibleType(value)
                && (this.constraint == null || this.constraint.IsCompatible(value));
        }

        /// <summary>
        /// Returns a string representation of this type
        /// </summary>
        /// <returns>A string representation of this type.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(base.ToString());
            if (this.constraint != null)
            {
                builder.Append(" (");
                builder.Append(this.constraint.ToString());
                builder.Append(")");
            }

            return builder.ToString();
        } 
        
        /// <summary>
        /// Checks if the specified value is compatible with this type. A
        /// value is compatible if it is a string value.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, false if not</returns>
        private bool IsCompatibleType(MibValue value)
        {
            return value is StringValue;
        }
    }
}
