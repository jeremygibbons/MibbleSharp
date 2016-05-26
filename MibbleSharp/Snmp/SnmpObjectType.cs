// <copyright file="SnmpObjectType.cs" company="None">
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
    using System.Linq;
    using System.Text;
    using MibbleSharp.Type;
    using MibbleSharp.Value;

    /// <summary>
    /// The SNMP object type macro type. This macro type was present in
    /// SMIv1, but was somewhat extended in SMIv2. It is defined in the
    /// RFC:s 1155, 1212 and 2578.
    /// <see href="http://www.ietf.org/rfc/rfc1155.txt">RFC 1155 (RFC1155-SMI)</see>
    /// <see href="http://www.ietf.org/rfc/rfc1212.txt">RFC 1212 (RFC-1212)</see>
    /// <see href="http://www.ietf.org/rfc/rfc2578.txt">RFC 2578 (<c>SNMPv2-SMI</c>)</see>
    /// </summary>
    public class SnmpObjectType : SnmpType
    {
        /// <summary>The type syntax.</summary>
        private MibType syntax;

        /// <summary>
        /// The units description.
        /// </summary>
        private string units;

        /// <summary>
        /// The access mode.
        /// </summary>
        private SnmpAccess access;

        /// <summary>
        /// The type status.
        /// </summary>
        private SnmpStatus status;

        /// <summary>
        /// The type reference.
        /// </summary>
        private string reference;

        /// <summary>
        /// The list of index values or types.
        /// </summary>
        private IList<SnmpIndex> index;

        /// <summary>
        /// The index augments value.
        /// </summary>
        private MibValue augments;

        /// <summary>
        /// The default value.
        /// </summary>
        private MibValue defaultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpObjectType"/> class.
        /// </summary>
        /// <param name="syntax">The object type syntax</param>
        /// <param name="units">The units description, or null</param>
        /// <param name="access">The access mode</param>
        /// <param name="status">The type status</param>
        /// <param name="description">The type description, or null</param>
        /// <param name="reference">The type reference, or null</param>
        /// <param name="index">The list of index objects</param>
        /// <param name="defaultValue">The default value, or null</param>
        public SnmpObjectType(
            MibType syntax,
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpObjectType"/> class.
        /// </summary>
        /// <param name="syntax">The object type syntax</param>
        /// <param name="units">The units description, or null</param>
        /// <param name="access">The access mode</param>
        /// <param name="status">The type status</param>
        /// <param name="description">The type description, or null</param>
        /// <param name="reference">The type reference, or null</param>
        /// <param name="augments">The index augments value</param>
        /// <param name="defaultValue">The default value, or null</param>
        public SnmpObjectType(
            MibType syntax,
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
        /// Gets the units description
        /// </summary>
        public string Units
        {
            get
            {
                return this.units;
            }
        }

        /// <summary>
        /// Gets the type access
        /// </summary>
        public SnmpAccess Access
        {
            get
            {
                return this.access;
            }
        }

        /// <summary>
        /// Gets the type status
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
        /// Gets the list of indices. The returned list will consist of
        /// <c>SnmpIndex</c> instances. Note that the semantics of this method
        /// changed in version 2.6, as the returned list previously
        /// contained type and value objects.
        /// </summary>
        public IList<SnmpIndex> Index
        {
            get
            {
                return this.index;
            }
        }

        /// <summary>
        /// Gets the augmented index value
        /// </summary>
        public MibValue Augments
        {
            get
            {
                return this.augments;
            }
        }

        /// <summary>
        /// Gets the default value
        /// </summary>
        public MibValue DefaultValue
        {
            get
            {
                return this.defaultValue;
            }
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
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            if (!(symbol is MibValueSymbol))
            {
                throw new MibException(
                    symbol.Location,
                    "only values can have the " + Name + " type");
            }

            this.syntax = this.syntax.Initialize(symbol, log);

            SnmpObjectType.CheckType((MibValueSymbol)symbol, log, this.syntax);

            foreach (SnmpIndex si in this.index)
            {
                si.Initialize(symbol, log);
            }

            if (this.augments != null)
            {
                this.augments = this.augments.Initialize(log, this.syntax);
            }

            if (this.defaultValue != null)
            {
                this.defaultValue = this.defaultValue.Initialize(log, this.syntax);
            }

            return this;
        }

        /// <summary>
        /// Checks if the specified value is compatible with this type. A
        /// value is compatible if and only if it is an object identifier
        /// value.
        /// </summary>
        /// <param name="value">The value to check for compatibility</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public override bool IsCompatible(MibValue value)
        {
            return value is ObjectIdentifierValue;
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

            builder.Append("\n  Syntax: ");
            builder.Append(this.syntax);

            if (this.units != null)
            {
                builder.Append("\n  Units: ");
                builder.Append(this.units);
            }

            builder.Append("\n  Access: ");
            builder.Append(this.access);

            builder.Append("\n  Status: ");
            builder.Append(this.status);

            if (this.UnformattedDescription != null)
            {
                builder.Append("\n  Description: ");
                builder.Append(this.GetDescription("               "));
            }

            if (this.reference != null)
            {
                builder.Append("\n  Reference: ");
                builder.Append(this.reference);
            }

            if (this.index.Count > 0)
            {
                builder.Append("\n  Index: ");
                builder.Append(this.index);
            }

            if (this.augments != null)
            {
                builder.Append("\n  Augments: ");
                builder.Append(this.augments);
            }

            if (this.defaultValue != null)
            {
                builder.Append("\n  Default Value: ");
                builder.Append(this.defaultValue);
            }

            builder.Append("\n)");
            return builder.ToString();
        }

        /// <summary>
        /// Validates a MIB type. This will check any sequences and make
        /// sure their elements are present in the MIB file. If they are
        /// not, new symbols will be added to the MIB.
        /// </summary>
        /// <param name="symbol">The MIB symbol containing this type</param>
        /// <param name="log">The MIB loader log</param>
        /// <param name="type">The MIB type to check</param>
        /// <exception cref="MibException">
        /// If an error was encountered during the validation
        /// </exception>
        private static void CheckType(
            MibValueSymbol symbol,
            MibLoaderLog log,
            MibType type)
        {
            SequenceOfType sequence;

            if (type is SequenceOfType)
            {
                sequence = (SequenceOfType)type;
                SnmpObjectType.CheckType(symbol, log, sequence.getElementType());
            }
            else if (type is SequenceType)
            {
                IList<ElementType> elems = ((SequenceType)type).getAllElements().ToList();
                for (int i = 0; i < elems.Count(); i++)
                {
                    SnmpObjectType.CheckElement(symbol, log, elems[i], i + 1);
                }
            }
        }

        /// <summary>
        /// Validates an element type. This will check that the element
        /// is present in the MIB file. If it is not, a new symbol will be
        /// added to the MIB.
        /// </summary>
        /// <param name="symbol">The MIB symbol containing this type</param>
        /// <param name="log">The MIB Loader log</param>
        /// <param name="element">The MIB element type to check</param>
        /// <param name="pos">The MIB element position</param>
        /// <exception cref="MibException">
        /// If an error was encountered during the validation
        /// </exception>
        private static void CheckElement(
            MibValueSymbol symbol,
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

                string msg = "sequence element " + name + " is undefined " +
                               "in MIB, a default symbol will be created";
                log.AddWarning(symbol.Location, msg);

                name = element.Name;

                if (name == null)
                {
                    name = symbol.Name + "." + pos;
                }

                type = new SnmpObjectType(
                    element.getType(),
                    null,
                    SnmpAccess.ReadOnly,
                    SnmpStatus.CURRENT,
                    "AUTOMATICALLY CREATED SYMBOL",
                    null,
                    new List<SnmpIndex>(),
                    null);

                value = (ObjectIdentifierValue)symbol.Value;
                value = new ObjectIdentifierValue(
                    symbol.Location,
                    value,
                    element.Name,
                    pos);

                elementSymbol = new MibValueSymbol(
                    symbol.Location,
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

                throw new MibException(
                    symbol.Location,
                    "sequence element " + name + " does not refer to a value, but to a type");
            }
        }
    }
}