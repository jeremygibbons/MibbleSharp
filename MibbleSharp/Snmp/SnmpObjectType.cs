//
// SnmpObjectType.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MibbleSharp.Type;
using MibbleSharp.Value;

namespace MibbleSharp.Snmp
{
    /**
     * The SNMP object type macro type. This macro type was present in
     * SMIv1, but was somewhat extended in SMIv2. It is defined in the
     * RFC:s 1155, 1212 and 2578.
     *
     * @see <a href="http://www.ietf.org/rfc/rfc1155.txt">RFC 1155 (RFC1155-SMI)</a>
     * @see <a href="http://www.ietf.org/rfc/rfc1212.txt">RFC 1212 (RFC-1212)</a>
     * @see <a href="http://www.ietf.org/rfc/rfc2578.txt">RFC 2578 (SNMPv2-SMI)</a>
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class SnmpObjectType : SnmpType
    {

        /**
         * The type syntax.
         */
        private MibType syntax;

        /**
         * The units description.
         */
        private string units;

        /**
         * The access mode.
         */
        private SnmpAccess access;

        /**
         * The type status.
         */
        private SnmpStatus status;

        /**
         * The type reference.
         */
        private string reference;

        /**
         * The list of index values or types.
         */
        private IList<SnmpIndex> index;

        /**
         * The index augments value.
         */
        private MibValue augments;

        /**
         * The default value.
         */
        private MibValue defaultValue;

        /**
         * Creates a new SNMP object type.
         *
         * @param syntax         the object type syntax
         * @param units          the units description, or null
         * @param access         the access mode
         * @param status         the type status
         * @param description    the type description, or null
         * @param reference      the type reference, or null
         * @param index          the list of index objects
         * @param defaultValue   the default value, or null
         */
        public SnmpObjectType(MibType syntax,
                              string units,
                              SnmpAccess access,
                              SnmpStatus status,
                              string description,
                              string reference,
                              IList<SnmpIndex> index,
                              MibValue defaultValue)
                : base("OBJECT-TYPE", description)
        {
            this.syntax = syntax;
            this.units = units;
            this.access = access;
            this.status = status;
            this.reference = reference;
            this.index = index;
            this.augments = null;
            this.defaultValue = defaultValue;
        }

        /**
         * Creates a new SNMP object type.
         *
         * @param syntax         the object type syntax
         * @param units          the units description, or null
         * @param access         the access mode
         * @param status         the type status
         * @param description    the type description, or null
         * @param reference      the type reference, or null
         * @param augments       the index augments value
         * @param defaultValue   the default value, or null
         */
        public SnmpObjectType(MibType syntax,
                              string units,
                              SnmpAccess access,
                              SnmpStatus status,
                              string description,
                              string reference,
                              MibValue augments,
                              MibValue defaultValue)
                : base("OBJECT-TYPE", description)
        {
            this.syntax = syntax;
            this.units = units;
            this.access = access;
            this.status = status;
            this.reference = reference;
            this.index = new List<SnmpIndex>();
            this.augments = augments;
            this.defaultValue = defaultValue;
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

            if (!(symbol is MibValueSymbol))
            {
                throw new MibException(symbol.Location,
                                       "only values can have the " +
                                       Name + " type");
            }
            syntax = syntax.Initialize(symbol, log);

            checkType((MibValueSymbol)symbol, log, syntax);

            foreach (SnmpIndex si in index)
            {
                si.Initialize(symbol, log);
            }

            if (augments != null)
            {
                augments = augments.Initialize(log, syntax);
            }

            if (defaultValue != null)
            {
                defaultValue = defaultValue.Initialize(log, syntax);
            }

            return this;
        }

        /**
         * Validates a MIB type. This will check any sequences and make
         * sure their elements are present in the MIB file. If they are
         * not, new symbols will be added to the MIB.
         *
         * @param symbol         the MIB symbol containing this type
         * @param log            the MIB loader log
         * @param type           the MIB type to check
         *
         * @throws MibException if an error was encountered during the
         *             validation
         *
         * @since 2.2
         */
        private void checkType(MibValueSymbol symbol,
                               MibLoaderLog log,
                               MibType type)
        {

            SequenceOfType sequence;

            if (type is SequenceOfType)
            {
                sequence = (SequenceOfType)type;
                checkType(symbol, log, sequence.getElementType());
            }
            else if (type is SequenceType)
            {
                IList<ElementType> elems = ((SequenceType)type).getAllElements().ToList();
                for (int i = 0; i < elems.Count(); i++)
                {
                    checkElement(symbol, log, elems[i], i + 1);
                }
            }
        }

        /**
         * Validates an element type. This will check that the element
         * is present in the MIB file. If it is not, a new symbol will be
         * added to the MIB.
         *
         * @param symbol         the MIB symbol containing this type
         * @param log            the MIB loader log
         * @param element        the MIB element type to check
         * @param pos            the MIB element position
         *
         * @throws MibException if an error was encountered during the
         *             validation
         *
         * @since 2.2
         */
        private void checkElement(MibValueSymbol symbol,
                                  MibLoaderLog log,
                                  ElementType element,
                                  int pos)
        {

            Mib mib = symbol.Mib;
            MibSymbol elementSymbol;
            string name;
            MibType type;
            ObjectIdentifierValue value;

            elementSymbol = mib.GetSymbol(element.Name);
            if (elementSymbol == null)
            {
                if (element.Name != null)
                {
                    name = pos + " '" + element.Name + "'";
                }
                else
                {
                    name = pos.ToString();
                }
                log.AddWarning(symbol.Location,
                               "sequence element " + name + " is undefined " +
                               "in MIB, a default symbol will be created");
                name = element.Name;
                if (name == null)
                {
                    name = symbol.Name + "." + pos;
                }
                type = new SnmpObjectType(element.getType(),
                                          null,
                                          SnmpAccess.ReadOnly,
                                          SnmpStatus.CURRENT,
                                          "AUTOMATICALLY CREATED SYMBOL",
                                          null,
                                          new List<SnmpIndex>(),
                                          null);
                value = (ObjectIdentifierValue)symbol.Value;
                value = new ObjectIdentifierValue(symbol.Location,
                                                  value,
                                                  element.Name,
                                                  pos);
                elementSymbol = new MibValueSymbol(symbol.Location,
                                                   mib,
                                                   name,
                                                   type,
                                                   value);
                elementSymbol.Initialize(log);
            }
            else if (elementSymbol is MibTypeSymbol)
            {
                if (element.Name != null)
                {
                    name = pos + " '" + element.Name + "'";
                }
                else
                {
                    name = pos.ToString();
                }
                throw new MibException(symbol.Location,
                                       "sequence element " + name +
                                       " does not refer to a value, but " +
                                       "to a type");
            }
        }

        /**
         * Checks if the specified value is compatible with this type. A
         * value is compatible if and only if it is an object identifier
         * value.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        public override bool IsCompatible(MibValue value)
        {
            return value is ObjectIdentifierValue;
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
         * Returns the units description.
         *
         * @return the units description, or
         *         null if no units has been set
         */
        public string getUnits()
        {
            return units;
        }

        /**
         * Returns the access mode.
         *
         * @return the access mode
         */
        public SnmpAccess getAccess()
        {
            return access;
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
         * Returns the list of indices. The returned list will consist of
         * SnmpIndex instances. Note that the semantics of this method
         * changed in version 2.6, as the returned list previously
         * contained type and value objects.
         *
         * @return the list of SNMP index objects, or
         *         an empty list if no indices are defined
         *
         * @see SnmpIndex
         *
         * @since 2.6
         */
        public IList<SnmpIndex> getIndex()
        {
            return index;
        }

        /**
         * Returns the augmented index value.
         *
         * @return the augmented index value, or
         *         null if no augments index is used
         */
        public MibValue getAugments()
        {
            return augments;
        }

        /**
         * Returns the default value.
         *
         * @return the default value, or
         *         null if no default value has been set
         */
        public MibValue getDefaultValue()
        {
            return defaultValue;
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
            builder.Append("\n  Syntax: ");
            builder.Append(syntax);
            if (units != null)
            {
                builder.Append("\n  Units: ");
                builder.Append(units);
            }
            builder.Append("\n  Access: ");
            builder.Append(access);
            builder.Append("\n  Status: ");
            builder.Append(status);
            if (UnformattedDescription!= null)
            {
                builder.Append("\n  Description: ");
                builder.Append(GetDescription("               "));
            }
            if (reference != null)
            {
                builder.Append("\n  Reference: ");
                builder.Append(reference);
            }
            if (index.Count > 0)
            {
                builder.Append("\n  Index: ");
                builder.Append(index);
            }
            if (augments != null)
            {
                builder.Append("\n  Augments: ");
                builder.Append(augments);
            }
            if (defaultValue != null)
            {
                builder.Append("\n  Default Value: ");
                builder.Append(defaultValue);
            }
            builder.Append("\n)");
            return builder.ToString();
        }
    }

}
