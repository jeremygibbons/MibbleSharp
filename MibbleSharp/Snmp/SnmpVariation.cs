//
// SnmpVariation.cs
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
using System.Linq;
using System.Text;
using MibbleSharp.Value;

namespace MibbleSharp.Snmp
{
    /**
     * An SNMP module variation value. This declaration is used inside a
     * module support declaration.
     *
     * @see SnmpModuleSupport
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class SnmpVariation
    {

        /**
         * The variation value.
         */
        private MibValue value;

        /**
         * The value syntax.
         */
        private MibType syntax;

        /**
         * The value write syntax.
         */
        private MibType writeSyntax;

        /**
         * The access mode.
         */
        private SnmpAccess access;

        /**
         * The cell values required for creation.
         */
        private IList<MibValue> requiredCells;

        /**
         * The default value.
         */
        private MibValue defaultValue;

        /**
         * The variation description.
         */
        private string description;

        /**
         * Creates a new SNMP module variation.
         *
         * @param value          the variation value
         * @param syntax         the value syntax, or null
         * @param writeSyntax    the value write syntax, or null
         * @param access         the access mode, or null
         * @param requiredCells  the cell values required for creation
         * @param defaultValue   the default value, or null
         * @param description    the variation description
         */
        public SnmpVariation(MibValue value,
                             MibType syntax,
                             MibType writeSyntax,
                             SnmpAccess access,
                             IList<MibValue> requiredCells,
                             MibValue defaultValue,
                             string description)
        {

            this.value = value;
            this.syntax = syntax;
            this.writeSyntax = writeSyntax;
            this.access = access;
            this.requiredCells = requiredCells;
            this.defaultValue = defaultValue;
            this.description = description;
        }

        /**
         * Initializes the object. This will remove all levels of
         * indirection present, such as references to other types, and
         * returns the basic type. No type information is lost by this
         * operation. This method may modify this object as a
         * side-effect, and will be called by the MIB loader.
         *
         * @param log            the MIB loader log
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        public void Initialize(MibLoaderLog log)
        {
            MibType type = null;

            value = value.Initialize(log, null);
            if (BaseSymbol != null)
            {
                // TODO: use utility function to retrieve correct base type here
                type = BaseSymbol.Type;
                if (type is SnmpTextualConvention)
                {
                    type = ((SnmpTextualConvention)type).getSyntax();
                }
                if (type is SnmpObjectType)
                {
                    type = ((SnmpObjectType)type).getSyntax();
                }
            }
            if (syntax != null)
            {
                syntax = syntax.Initialize(null, log);
            }
            if (writeSyntax != null)
            {
                writeSyntax = writeSyntax.Initialize(null, log);
            }
            requiredCells = requiredCells.Select(rc => rc.Initialize(log, type)).ToList();
            if (defaultValue != null)
            {
                defaultValue = defaultValue.Initialize(log, type);
            }
        }

        /**
         * Returns the base symbol that this variation applies to.
         *
         * @return the base symbol that this variation applies to
         *
         * @since 2.8
         */
        public MibValueSymbol BaseSymbol
        {
            get
            {
                if (value is ObjectIdentifierValue)
                {
                    return ((ObjectIdentifierValue)value).getSymbol();
                }
                else
                {
                    return null;
                }
            }
        }

        /**
         * Returns the value.
         *
         * @return the value
         */
        public MibValue getValue()
        {
            return value;
        }

        /**
         * Returns the value syntax.
         *
         * @return the value syntax, or
         *         null if not set
         */
        public MibType Syntax
        {
            get
            {
                return syntax;
            }
        }

        /**
         * Returns the value write syntax.
         *
         * @return the value write syntax, or
         *         null if not set
         */
        public MibType WriteSyntax
        {
            get
            {
                return writeSyntax;
            }
        }

        /**
         * Returns the access mode.
         *
         * @return the access mode, or
         *         null if not set
         */
        public SnmpAccess Access
        {
            get
            {
                return access;
            }
        }

        /**
         * Returns cell values required for creation. The returned list
         * will consist of MibValue instances.
         *
         * @return cell values required for creation
         *
         * @see net.percederberg.mibble.MibValue
         */
        public IList<MibValue> RequiredCells
        {
            get
            {
                return requiredCells;
            }
        }

        /**
         * Returns the default value.
         *
         * @return the default value, or
         *         null if no default value has been set
         */
        public MibValue DefaultValue
        {
            get
            {
                return defaultValue;
            }
        }

        /**
         * Returns the variation description.
         *
         * @return the variation description
         */
        public string Description
        {
            get
            {
                return description;
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

            builder.Append(value);
            if (syntax != null)
            {
                builder.Append("\n      Syntax: ");
                builder.Append(syntax);
            }
            if (writeSyntax != null)
            {
                builder.Append("\n      Write-Syntax: ");
                builder.Append(writeSyntax);
            }
            if (access != null)
            {
                builder.Append("\n      Access: ");
                builder.Append(access);
            }
            if (requiredCells.Count > 0)
            {
                builder.Append("\n      Creation-Requires: ");
                builder.Append(requiredCells);
            }
            if (defaultValue != null)
            {
                builder.Append("\n      Default Value: ");
                builder.Append(defaultValue);
            }
            builder.Append("\n      Description: ");
            builder.Append(description);
            return builder.ToString();
        }
    }

}
