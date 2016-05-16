//
// SnmpTextualConvention.cs
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

using System.Collections.Generic;
using System.Text;
using MibbleSharp.Type;

namespace MibbleSharp.Snmp
{
    /**
     * The SNMP textual convention macro type. This macro type was added
     * to SMIv2 and is defined in RFC 2579.
     *
     * @see <a href="http://www.ietf.org/rfc/rfc2579.txt">RFC 2579 (SNMPv2-TC)</a>
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.7
     * @since    2.0
     */
    public class SnmpTextualConvention : SnmpType, MibContext
    {

    /**
     * The display hint.
     */
    private string displayHint;

    /**
     * The type status.
     */
    private SnmpStatus status;

    /**
     * The type reference.
     */
    private string reference;

    /**
     * The type syntax.
     */
    private MibType syntax;

    /**
     * Finds the first SNMP textual convention reference for a type. If the
     * type specified is a textual convention, it will be returned directly.
     *
     * @param type           the MIB type
     *
     * @return the SNMP textual convention reference, or
     *         null if none was found
     *
     * @since 2.7
     */
    public static SnmpTextualConvention findReference(MibType type)
    {
        MibTypeSymbol sym;

        if (type is SnmpObjectType) {
            type = ((SnmpObjectType)type).getSyntax();
        }
        if (type is SnmpTextualConvention) {
            return (SnmpTextualConvention)type;
        }
            sym = type.ReferenceSymbol;
        return (sym == null) ? null : findReference(sym.getType());
    }

    /**
     * Creates a new SNMP textual convention.
     *
     * @param displayHint    the display hint, or null
     * @param status         the type status
     * @param description    the type description
     * @param reference      the type reference, or null
     * @param syntax         the type syntax
     */
    public SnmpTextualConvention(string displayHint,
                                 SnmpStatus status,
                                 string description,
                                 string reference,
                                 MibType syntax)
            : base("TEXTUAL-CONVENTION", description)
        {        
        this.displayHint = displayHint;
        this.status = status;
        this.reference = reference;
        this.syntax = syntax;
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

        syntax = syntax.Initialize(symbol, log);
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
     * @throws UnsupportedOperationException if a type reference
     *             couldn't be created
     *
     * @since 2.2
     */
    public override MibType CreateReference()
    {

        return syntax.CreateReference();
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
     * @throws UnsupportedOperationException if a type reference
     *             couldn't be created with constraints
     *
     * @since 2.2
     */
    public override MibType CreateReference(Constraint constraint)
    {

        return syntax.CreateReference(constraint);
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
     * @param values         the type value symbols
     *
     * @return the MIB type reference
     *
     * @throws UnsupportedOperationException if a type reference
     *             couldn't be created with value constraints
     *
     * @since 2.2
     */
    public override MibType CreateReference(IList<MibValueSymbol> values)
    {

        return syntax.CreateReference(values);
    }

    /**
     * Checks if the specified value is compatible with this type. No
     * value is compatible with this type, so this method always
     * returns false.
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
     * Returns the display hint.
     *
     * @return the display hint, or
     *         null if no display hint has been set
     */
    public string getDisplayHint()
    {
        return displayHint;
    }

    /**
     * Returns the type status.
     *
     * @return the type status
     */
    public SnmpStatus getStatus()
    {
        return status;
    }

    /**
     * Returns the type reference.
     *
     * @return the type reference, or
     *         null if no reference has been set
     */
    public string getReference()
    {
        return reference;
    }

    /**
     * Returns the type syntax.
     *
     * @return the type syntax
     */
    public MibType getSyntax()
    {
        return syntax;
    }

    /**
     * Searches for a named MIB symbol. This method may search outside
     * the normal (or strict) scope, thereby allowing a form of
     * relaxed search. Note that the results from the normal and
     * expanded search may not be identical, due to the context
     * chaining and the same symbol name appearing in various
     * contexts.<p>
     *
     * <strong>NOTE:</strong> This is an internal method that should
     * only be called by the MIB loader.
     *
     * @param name           the symbol name
     * @param expanded       the expanded scope flag
     *
     * @return the MIB symbol, or null if not found
     *
     * @since 2.4
     */
    public MibSymbol FindSymbol(string name, bool expanded)
    {
        if (syntax is MibContext) {
            return ((MibContext)syntax).FindSymbol(name, expanded);
        } else {
            return null;
        }
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
        builder.Append(" (");
        if (displayHint != null)
        {
            builder.Append("\n  Display-Hint: ");
            builder.Append(displayHint);
        }
        builder.Append("\n  Status: ");
        builder.Append(status);
        builder.Append("\n  Description: ");
        builder.Append(getDescription("               "));
        if (reference != null)
        {
            builder.Append("\n  Reference: ");
            builder.Append(reference);
        }
        builder.Append("\n  Syntax: ");
        builder.Append(syntax);
        builder.Append("\n)");
        return builder.ToString();
    }
}

}
