// <copyright file="SequenceOfType.cs" company="None">
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

    /// <summary>
    /// A sequence of a MIB type. In some other languages this is known as an array.
    /// </summary>
    public class SequenceOfType : MibType
    {
        /// <summary>
        /// The base type.
        /// </summary>
        private MibType baseType;

        /// <summary>
        /// The additional type constraint.
        /// </summary>
        private IConstraint constraint = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceOfType"/> class.
        /// </summary>
        /// <param name="baseType">The sequence element type</param>         
        public SequenceOfType(MibType baseType) : this(true, baseType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceOfType"/> class.
        /// </summary>
        /// <param name="baseType">The sequence element type</param>
        /// <param name="constraint">The sequence constraint</param>
        public SequenceOfType(MibType baseType, IConstraint constraint) : this(true, baseType, constraint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceOfType"/> class.
        /// </summary>
        /// <param name="primitive">The primitive type flag</param>
        /// <param name="baseType">The sequence element type</param>
        /// <param name="constraint">The sequence constraint</param>
        private SequenceOfType(
            bool primitive,
            MibType baseType,
            IConstraint constraint)
            : base("SEQUENCE", primitive)
        {
            this.baseType = baseType;
            this.constraint = constraint;
        }

        /// <summary>
        /// Gets a value indicating whether this type has any constraint.
        /// </summary>
        public bool HasConstraint
        {
            get
            {
                return this.constraint != null;
            }
        }

        /// <summary>
        /// Gets the optional type constraint. The type constraint for
        /// a sequence of type will typically be a size constraint.
        /// </summary>
        /// <returns>
        /// the type constraint, or
        /// null if no constraint has been set
        /// </returns>
        public IConstraint Constraint
        {
            get
            {
                return this.constraint;
            }
        }

        /// <summary>
        /// Gets the sequence element type. This is the type of each
        /// individual element in the sequence.
        /// </summary>
        public MibType ElementType
        {
            get
            {
                return this.baseType;
            }
        }

        /// <summary>
        /// Initializes the MIB type. This will remove all levels of
        /// indirection present, such as references to types or values. No
        /// information is lost by this operation. This method may modify
        /// this object as a side-effect, and will return the basic
        /// type.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        /// <param name="symbol">The MIB symbol containing this type</param>
        /// <param name="log">The MIB loader log</param>
        /// <returns>The MIB type</returns>
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            this.SetTag(true, MibTypeTag.Sequence);

            this.baseType = this.baseType.Initialize(symbol, log);
            if (this.baseType != null && this.constraint != null)
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
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        /// <returns>The MIB type reference</returns>
        public override MibType CreateReference()
        {
            SequenceOfType type = new SequenceOfType(false, this.baseType, this.constraint);

            type.SetTag(true, this.Tag);
            return type;
        }

        /// <summary>
        /// Creates a constrained type reference to this type. The type
        /// reference is normally an identical type, but with the
        /// primitive flag set to false. Only certain types support being
        /// referenced, and the default implementation of this method
        /// throws an exception.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        /// <param name="constraint">The type constraint</param>
        /// <returns>The MIB type reference</returns>
        public override MibType CreateReference(IConstraint constraint)
        {
            SequenceOfType type = new SequenceOfType(false, this.baseType, this.constraint);
            type.SetTag(true, this.Tag);
            return type;
        }

        /// <summary>
        /// Checks if the specified value is compatible with this type. No
        /// values are considered compatible with this type, and this
        /// method will therefore always return false.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public override bool IsCompatible(MibValue value)
        {
            return false;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>
        /// A string representation of this object
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(base.ToString());
            builder.Append(" ");

            if (this.constraint != null)
            {
                builder.Append("(");
                builder.Append(this.constraint.ToString());
                builder.Append(") ");
            }

            builder.Append("OF ");
            builder.Append(this.baseType);
            return builder.ToString();
        }
    }
}