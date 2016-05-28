// <copyright file="MibTypeContext.cs" company="None">
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

namespace MibbleSharp
{
    using System;
    using MibbleSharp.Snmp;
    using MibbleSharp.Type;
    using MibbleSharp.Value;

    /// <summary>
    /// A MIB type context. This class attempts to resolve all symbols as
    /// defined enumeration values in the contained MIB type.
    /// </summary>
    public class MibTypeContext : IMibContext
    {
        /// <summary>The MIB symbol, value or type.</summary>
        private object context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MibTypeContext"/> class.
        /// </summary>
        /// <param name="context">The MIB Symbol, Value or Type</param>
        public MibTypeContext(object context)
        {
            this.context = context;
        }

        /// <summary>Searches for a named MIB symbol. This method may search outside
        /// the normal (or strict) scope, thereby allowing a form of
        /// relaxed search. Note that the results from the normal and
        /// expanded search may not be identical, due to the context
        /// chaining and the same symbol name appearing in various
        /// contexts.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="name">The symbol name</param>
        /// <param name="expanded">The expanded scope flag</param>
        /// <returns>The MIB symbol, or null if the symbol is not found</returns>
        public MibSymbol FindSymbol(string name, bool expanded)
        {
            IMibContext ctx = null;

            if (this.context is ValueReference)
            {
                this.context = ((ValueReference)this.context).Symbol;
            }

            if (this.context is MibTypeSymbol)
            {
                this.context = ((MibTypeSymbol)this.context).Type;
            }

            if (this.context is MibValueSymbol)
            {
                this.context = ((MibValueSymbol)this.context).Type;
            }

            if (this.context is SnmpObjectType)
            {
                this.context = ((SnmpObjectType)this.context).Syntax;
            }

            if (this.context is TypeReference)
            {
                this.context = ((TypeReference)this.context).Symbol;
                return this.FindSymbol(name, expanded);
            }

            if (this.context is IMibContext)
            {
                ctx = (IMibContext)this.context;
            }

            return (ctx == null) ? null : ctx.FindSymbol(name, expanded);
        }

        /// <summary>Returns a string representation of this object.</summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return "<type context>";
        }
    }
}
