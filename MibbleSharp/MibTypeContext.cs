//
// MibTypeContext.cs
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

using System;
using MibbleSharp.Snmp;
using MibbleSharp.Type;
using MibbleSharp.Value;

namespace MibbleSharp
{
    /**
     * A MIB type context. This class attempts to resolve all symbols as
     * defined enumeration values in the contained MIB type.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.8
     */
    class MibTypeContext : MibContext
    {

    /**
     * The MIB symbol, value or type.
     */
    private Object context;

    /**
     * Creates a new MIB type context.
     *
     * @param context        the MIB symbol, value or type
     */
    public MibTypeContext(Object context)
    {
        this.context = context;
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
        MibContext ctx = null;

        if (context is ValueReference) {
            context = ((ValueReference)context).getSymbol();
        }
        if (context is MibTypeSymbol) {
            context = ((MibTypeSymbol)context).getType();
        }
        if (context is MibValueSymbol) {
            context = ((MibValueSymbol)context).getType();
        }
        if (context is SnmpObjectType) {
            context = ((SnmpObjectType)context).getSyntax();
        }
        if (context is TypeReference) {
            context = ((TypeReference)context).getSymbol();
            return FindSymbol(name, expanded);
        }
        if (context is MibContext) {
            ctx = (MibContext)context;
        }
        return (ctx == null) ? null : ctx.FindSymbol(name, expanded);
    }

    /**
     * Returns a string representation of this object.
     *
     * @return a string representation of this object
     */
    public override string ToString()
    {
        return "<type context>";
    }
}

}
