// <copyright file="MibValueSymbol.cs" company="None">
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
    using System.Text;
    using MibbleSharp.Snmp;
    using MibbleSharp.Type;
    using MibbleSharp.Value;

    /// <summary>
    /// A MIB value symbol. This class holds information relevant to a MIB
    /// value assignment, i.e.a type and a value.Normally the value is
    /// an object identifier.
    /// </summary>
    public class MibValueSymbol : MibSymbol
    {
        /// <summary>The symbol type.</summary>
        private MibType type;

        /// <summary>The symbol value.</summary>
        private MibValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="MibValueSymbol"/> class.
        /// NOTE: This is an internal constructor that
        /// should only be called by the MIB loader.
        /// </summary>
        /// <param name="location">The symbol location</param>
        /// <param name="mib">The symbol MIB name</param>
        /// <param name="name">The symbol name</param>
        /// <param name="type">The symbol type</param>
        /// <param name="value">The symbol value</param>
        public MibValueSymbol(
            FileLocation location,
            Mib mib,
            string name,
            MibType type,
            MibValue value)
            : base(location, mib, name)
        {
            this.type = type;
            this.value = value;
        }

        /// <summary>Gets a value indicating whether this symbol corresponds to a scalar. A symbol is
        /// considered a scalar if it has an <c>SnmpObjectType</c> type and does
        /// not represent or reside within a table.
        /// </summary>
        /// <see cref="IsTable"/>
        /// <see cref="IsTableRow"/>
        /// <see cref="IsTableColumn"/>
        /// <see cref="Snmp.SnmpObjectType"/>
        public bool IsScalar
        {
            get
            {
                return this.type is SnmpObjectType
                && !this.IsTable && !this.IsTableRow && !this.IsTableColumn;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this symbol corresponds to a table. A symbol is
        /// considered a table if it has an <c>SnmpObjectType</c> type with
        /// SEQUENCE OF syntax.
        /// </summary>
        /// <see cref="IsScalar"/>
        /// <see cref="IsTableRow"/>
        /// <see cref="IsTableColumn"/>
        /// <see cref="Snmp.SnmpObjectType"/>
        public bool IsTable
        {
            get
            {
                MibType syntax;

                if (this.type is SnmpObjectType)
                {
                    syntax = ((SnmpObjectType)this.type).getSyntax();
                    return syntax is SequenceOfType;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this symbol corresponds to a table row (or entry). A
        /// symbol is considered a table row if it has an <c>SnmpObjectType</c>
        /// type with SEQUENCE syntax.
        /// </summary>
        /// <see cref="IsScalar"/>
        /// <see cref="IsTableRow"/>
        /// <see cref="IsTableColumn"/>
        /// <see cref="Snmp.SnmpObjectType"/>
        public bool IsTableRow
        {
            get
            {
                MibType syntax;

                if (this.type is SnmpObjectType)
                {
                    syntax = ((SnmpObjectType)this.type).getSyntax();
                    return syntax is SequenceType;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this symbol corresponds to a table column. A symbol
        /// is considered a table column if it has an <c>SnmpObjectType</c> type
        /// and a parent symbol that is a table row.
        /// </summary>
        /// <see cref="IsScalar"/>
        /// <see cref="isTable"/>
        /// <see cref="IsTableRow"/>
        /// <see cref="Snmp.SnmpObjectType"/>
        public bool IsTableColumn
        {
            get
            {
                MibValueSymbol par = this.Parent;

                return this.type is SnmpObjectType
                && par != null
                && par.IsTableRow;
            }
        }

        /// <summary>
        /// Gets the symbol type
        /// </summary>
        public MibType Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Gets the symbol value
        /// </summary>
        public MibValue Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Gets the parent symbol in the OID tree. This is a
        /// convenience method for value symbols that have object
        /// identifier values. 
        /// </summary>
        public MibValueSymbol Parent
        {
            get
            {
                ObjectIdentifierValue oid;

                if (this.value is ObjectIdentifierValue)
                {
                    oid = ((ObjectIdentifierValue)this.value).getParent();
                    if (oid != null)
                    {
                        return oid.getSymbol();
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the number of child symbols in the OID tree. This is a
        /// convenience method for value symbols that have object
        /// identifier values. 
        /// </summary>
        public int ChildCount
        {
            get
            {
                ObjectIdentifierValue oiv = this.value as ObjectIdentifierValue;
                if (oiv == null)
                {
                    return 0;
                }
                
                return oiv.getChildCount();
            }
        }

        /// <summary>
        /// Gets all child symbols in the OID tree. This is a
        /// convenience method for value symbols that have object
        /// identifier values. 
        /// </summary>
        public MibValueSymbol[] Children
        {
            get
            {
                MibValueSymbol[] children;

                ObjectIdentifierValue oid = this.value as ObjectIdentifierValue;

                if (oid == null)
                {
                    return new MibValueSymbol[0];
                }

                children = new MibValueSymbol[oid.getChildCount()];

                for (int i = 0; i < oid.getChildCount(); i++)
                {
                    children[i] = oid.getChild(i).getSymbol();
                }

                return children;
            }
        }

        /// <summary>
        /// Returns a specific child symbol in the OID tree. This is a
        /// convenience method for value symbols that have object
        /// identifier values. 
        /// </summary>
        /// <param name="index">The child position</param>
        /// <returns>
        /// The child symbol in the OID tree, or
        /// null if not found or not applicable
        /// </returns>
        /// <see cref="ObjectIdentifierValue"/>
        public MibValueSymbol GetChild(int index)
        {
            ObjectIdentifierValue child;

            ObjectIdentifierValue oiv = this.value as ObjectIdentifierValue;

            if (oiv == null)
            {
                return null;
            }

            child = oiv.getChild(index);
            if (child != null)
            {
                return child.getSymbol();
            }

            return null;
        }

        /// <summary>
        /// Initializes the MIB symbol. This will remove all levels of
        /// indirection present, such as references to types or values. No
        /// information is lost by this operation. This method may modify
        /// this object as a side-effect.
        /// </summary>
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// <param name="log">The MIB loader log</param>
        /// <exception cref="MibException">
        /// If an error was encountered during the initialization
        /// </exception>
        public override void Initialize(MibLoaderLog log)
        {
            ObjectIdentifierValue oid;

            if (this.type != null)
            {
                try
                {
                    this.type = this.type.Initialize(this, log);
                }
                catch (MibException e)
                {
                    log.AddError(e.Location, e.Message);
                    this.type = null;
                }
            }

            if (this.value != null)
            {
                try
                {
                    this.value = this.value.Initialize(log, this.type);
                }
                catch (MibException e)
                {
                    log.AddError(e.Location, e.Message);
                    this.value = null;
                }
            }

            if (this.type != null && this.value != null && !this.type.IsCompatible(this.value))
            {
                log.AddError(this.Location, "value is not compatible with type");
            }

            if (this.value is ObjectIdentifierValue)
            {
                oid = (ObjectIdentifierValue)this.value;
                if (oid.getSymbol() == null)
                {
                    oid.setSymbol(this);
                }
            }
        }

        /// <summary>
        /// Clears and prepares this MIB symbol for garbage collection.
        /// This method will recursively clear any associated types or
        /// values, making sure that no data structures references this
        /// symbol.
        /// </summary>
        public override void Clear()
        {
            this.type = null;
            if (this.value != null)
            {
                this.value.Clear();
            }

            this.value = null;
        }

        /// <summary>
        /// Get a string representation of this symbol
        /// </summary>
        /// <returns>A string representation of this symbol</returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("VALUE ");
            buffer.Append(this.Name);
            buffer.Append(" ");
            buffer.Append(this.Type);
            buffer.Append("\n    ::= ");
            buffer.Append(this.Value);
            return buffer.ToString();
        }
    }
}