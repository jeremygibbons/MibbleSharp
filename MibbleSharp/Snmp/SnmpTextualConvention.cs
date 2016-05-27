// <copyright file="SnmpTextualConvention.cs" company="None">
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

namespace MibbleSharp.Snmp
{
    using System.Collections.Generic;
    using System.Text;
    using MibbleSharp.Type;
    
    /// <summary>
    /// The SNMP textual convention macro type. This macro type was added
    /// to SMIv2 and is defined in RFC 2579.
    /// </summary>
    /// <see href="http://www.ietf.org/rfc/rfc2579.txt">RFC 2579 (SNMPv2-TC)</see>
    public class SnmpTextualConvention : SnmpType, IMibContext
    {
        /// <summary>
        /// The display hint.
        /// </summary>
        private string displayHint;

        /// <summary>
        /// The type status.
        /// </summary>
        private SnmpStatus status;

        /// <summary>
        /// The type reference.
        /// </summary>
        private string reference;

        /// <summary>
        /// The type syntax.
        /// </summary>
        private MibType syntax;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpTextualConvention"/> class.
        /// </summary>
        /// <param name="displayHint">The display hint, or null</param>
        /// <param name="status">The type status</param>
        /// <param name="description">The type description</param>
        /// <param name="reference">The type reference, or null</param>
        /// <param name="syntax">The type syntax</param>
        public SnmpTextualConvention(
            string displayHint,
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

        /// <summary>
        /// Gets the display hint
        /// </summary>
        public string DisplayHint
        {
            get
            {
                return this.displayHint;
            }
        }

        /// <summary>
        /// Gets the type status.
        /// </summary>
        public SnmpStatus Status
        {
            get
            {
                return this.status;
            }
        }
        
        /// <summary>
        /// Gets the type reference
        /// </summary>
        public string Reference
        {
            get
            {
                return this.reference;
            }
        }
        
        /// <summary>
        /// Gets the type syntax
        /// </summary>
        public MibType Syntax
        {
            get
            {
                return this.syntax;
            }
        }

        /// <summary>
        /// Finds the first SNMP textual convention reference for a type. If the
        /// type specified is a textual convention, it will be returned directly.
        /// </summary>
        /// <param name="type">The MIB type</param>
        /// <returns>The SNMP Textual Convention reference if found, null if not</returns>
        public static SnmpTextualConvention FindReference(MibType type)
        {
            MibTypeSymbol sym;

            if (type is SnmpObjectType)
            {
                type = ((SnmpObjectType)type).Syntax;
            }

            if (type is SnmpTextualConvention)
            {
                return (SnmpTextualConvention)type;
            }

            sym = type.ReferenceSymbol;

            return (sym == null) ? null : SnmpTextualConvention.FindReference(sym.Type);
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
        /// <exception cref="MibException">
        /// If an error was encountered during the initialization
        /// </exception>
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            this.syntax = this.syntax.Initialize(symbol, log);
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
            return this.syntax.CreateReference();
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
        /// <returns>The MIB Type reference</returns>
        public override MibType CreateReference(IConstraint constraint)
        {
            return this.syntax.CreateReference(constraint);
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
        /// <param name="values">The type value symbols</param>
        /// <returns>The MIB type reference</returns>
        public override MibType CreateReference(IList<MibValueSymbol> values)
        {
            return this.syntax.CreateReference(values);
        }

        /// <summary>
        /// Checks if the specified value is compatible with this type. No
        /// value is compatible with this type, so this method always
        /// returns false.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public override bool IsCompatible(MibValue value)
        {
            return false;
        }        
        
        /// <summary>
        /// Searches for a named MIB symbol. This method may search outside
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
        /// <returns>The MIB symbol, or null if not found</returns>
        public MibSymbol FindSymbol(string name, bool expanded)
        {
            if (this.syntax is IMibContext)
            {
                return ((IMibContext)this.syntax).FindSymbol(name, expanded);
            }

            return null;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(base.ToString());
            builder.Append(" (");

            if (this.displayHint != null)
            {
                builder.Append("\n  Display-Hint: ");
                builder.Append(this.displayHint);
            }

            builder.Append("\n  Status: ");
            builder.Append(this.status);

            builder.Append("\n  Description: ");
            builder.Append(this.GetDescription("               "));

            if (this.reference != null)
            {
                builder.Append("\n  Reference: ");
                builder.Append(this.reference);
            }

            builder.Append("\n  Syntax: ");
            builder.Append(this.syntax);

            builder.Append("\n)");
            return builder.ToString();
        }
    }
}
