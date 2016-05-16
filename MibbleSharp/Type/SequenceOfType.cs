//
// SequenceOfType.cs
// 
// This work is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; either version 2 of the License,
// or (at your option) any later version.
//
// This work is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
// USA
// 
// Original Java code Copyright (c) 2004-2016 Per Cederberg. All
// rights reserved.
// C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//

using System.Text;

namespace MibbleSharp.Type
{

    /**
     * An sequence of a MIB type. In some other languages this is known
     * as an array.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.6
     * @since    2.0
     */
    public class SequenceOfType : MibType
    {

    /**
     * The base type.
     */
    private MibType baseType;

    /**
     * The additional type constraint.
     */
    private Constraint constraint = null;

    /**
     * Creates a new sequence of a MIB type.
     *
     * @param base           the sequence element type
     */
    public SequenceOfType(MibType baseType) : this(true, baseType, null)
        {
        
    }

    /**
     * Creates a new sequence of a MIB type.
     *
     * @param base           the sequence element type
     * @param constraint     the sequence constraint
     */
    public SequenceOfType(MibType baseType, Constraint constraint) : this(true, baseType, constraint)
        {
       
    }

    /**
     * Creates a new sequence of a MIB type.
     *
     * @param primitive      the primitive type flag
     * @param base           the sequence element type
     * @param constraint     the sequence constraint
     */
    private SequenceOfType(bool primitive,
                           MibType baseType,
                           Constraint constraint)
            : base("SEQUENCE", primitive)
    {

        this.baseType = baseType;
        this.constraint = constraint;
        setTag(true, MibTypeTag.SEQUENCE);
    }

    /**
     * Initializes the MIB type. This will remove all levels of
     * indirection present, such as references to types or values. No
     * information is lost by this operation. This method may modify
     * this object as a side-effect, and will return the basic
     * type.<p>
     *
     * <strong>NOTE:</strong> This is an internal method that should
     * only be called by the MIB loader.
     *
     * @param symbol         the MIB symbol containing this type
     * @param log            the MIB loader log
     *
     * @return the basic MIB type
     *
     * @throws MibException if an error was encountered during the
     *             initialization
     *
     * @since 2.2
     */
    public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
    {

        baseType = baseType.Initialize(symbol, log);
        if (baseType != null && constraint != null) {
            constraint.Initialize(this, log);
        }
        return this;
    }

    /**
     * Creates a type reference to this type. The type reference is
     * normally an identical type, but with the primitive flag set to
     * false. Only certain types support being referenced, and the
     * default implementation of this method throws an exception.<p>
     *
     * <strong>NOTE:</strong> This is an internal method that should
     * only be called by the MIB loader.
     *
     * @return the MIB type reference
     *
     * @since 2.2
     */
    public override MibType CreateReference()
    {
        SequenceOfType type = new SequenceOfType(false, baseType, constraint);

        type.setTag(true, getTag());
        return type;
    }

    /**
     * Creates a constrained type reference to this type. The type
     * reference is normally an identical type, but with the
     * primitive flag set to false. Only certain types support being
     * referenced, and the default implementation of this method
     * throws an exception.<p>
     *
     * <strong>NOTE:</strong> This is an internal method that should
     * only be called by the MIB loader.
     *
     * @param constraint     the type constraint
     *
     * @return the MIB type reference
     *
     * @since 2.2
     */
    public override MibType CreateReference(Constraint constraint)
    {
        SequenceOfType type = new SequenceOfType(false, baseType, constraint);

        type.setTag(true, getTag());
        return type;
    }

    /**
     * Checks if this type has any constraint.
     *
     * @return true if this type has any constraint, or
     *         false otherwise
     *
     * @since 2.2
     */
    public bool HasConstraint()
    {
        return constraint != null;
    }

    /**
     * Checks if the specified value is compatible with this type. No
     * values are considered compatible with this type, and this
     * method will therefore always return false.
     *
     * @param value          the value to check
     *
     * @return true if the value is compatible, or
     *         false otherwise
     */
    public override bool IsCompatible(MibValue value)
    {
        return false;
    }

    /**
     * Returns the optional type constraint. The type constraint for
     * a sequence of type will typically be a size constraint.
     *
     * @return the type constraint, or
     *         null if no constraint has been set
     *
     * @since 2.2
     */
    public Constraint GetConstraint()
    {
        return constraint;
    }

    /**
     * Returns the sequence element type. This is the type of each
     * individual element in the sequence.
     *
     * @return the sequence element type
     *
     * @since 2.2
     */
    public MibType getElementType()
    {
        return baseType;
    }

    /**
     * Returns a string representation of this object.
     *
     * @return a string representation of this object
     */
    public string toString()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(base.ToString());
        builder.Append(" ");
        if (constraint != null)
        {
            builder.Append("(");
            builder.Append(constraint.ToString());
            builder.Append(") ");
        }
        builder.Append("OF ");
        builder.Append(baseType);
        return builder.ToString();
    }
}

}
