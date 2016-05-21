//
// ElementType.cs
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
     * A compound element MIB type. This typs is used inside various
     * compound types, storing a reference to the type and an optional
     * name.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.2
     * @since    2.0
     */
    public class ElementType : MibType
    {
    /**
     * The element type.
     */
    private MibType type;

    /**
     * Creates a new element type.
     *
     * @param name           the optional element name
     * @param type           the element type
     */
    public ElementType(string name, MibType type) : base("", false)
        {
        this.Name = name;
        this.type = type;
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

        type = type.Initialize(symbol, log);
        return this;
    }

    /**
     * Checks if the specified value is compatible with this type.
     * The value is considered compatible with this type, if it is
     * compatible with the underlying type.
     *
     * @param value          the value to check
     *
     * @return true if the value is compatible, or
     *         false otherwise
     */
    public override bool IsCompatible(MibValue value)
    {
        return type.IsCompatible(value);
    }

    /**
     * Returns the referenced MIB type.
     *
     * @return the referenced MIB type
     *
     * @since 2.2
     */
    public MibType getType()
    {
        return type;
    }

    /**
     * Returns a string representation of this object.
     *
     * @return a string representation of this object
     */
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(base.ToString());
        if (this.Name != null)
        {
            builder.Append(this.Name);
            builder.Append(" ");
        }
        builder.Append(type.ToString());
        return builder.ToString();
    }
}

}
