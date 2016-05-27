// <copyright file="SnmpTrapType.cs" company="None">
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
    using MibbleSharp.Value;

    /// <summary>
    /// The SNMP trap type macro. This macro type is only present in
    /// SMIv1 and is defined in RFC 1215. In SMIv2 and later, the
    /// notification type macro should be used instead.
    /// </summary>
    /// <see cref="SnmpNotificationType"/>
    /// <see href="http://www.ietf.org/rfc/rfc1215.txt"> RFC 1215 (RFC-1215)</see>
    public class SnmpTrapType : SnmpType
    {
        /// <summary>
        /// The enterprise value
        /// </summary>
        private MibValue enterprise;

        /// <summary>
        /// The list of MIB values.
        /// </summary>
        private IList<MibValue> variables;

        /// <summary>
        /// The type reference.
        /// </summary>
        private string reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpTrapType"/> class.
        /// </summary>
        /// <param name="enterprise">the enterprise value</param>
        /// <param name="variables">The list of MIB values</param>
        /// <param name="description">The type description or null</param>
        /// <param name="reference">The type reference or null</param>
        public SnmpTrapType(
            MibValue enterprise,
            IList<MibValue> variables,
            string description,
            string reference)
            : base("TRAP-TYPE", description)
        {
            this.enterprise = enterprise;
            this.variables = variables;
            this.reference = reference;
        }

        /// <summary>
        /// Gets the enterprise
        /// </summary>
        public MibValue Enterprise
        {
            get
            {
                return this.enterprise;
            }
        }

        /// <summary>
        /// Gets the list of MIB variables
        /// </summary>
        /// <see cref="MibValue"/>
        public IList<MibValue> Variables
        {
            get
            {
                return this.variables;
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
            if (!(symbol is MibValueSymbol))
            {
                throw new MibException(
                    symbol.Location,
                    "only values can have the " + Name + " type");
            }

            this.enterprise = this.enterprise.Initialize(log, null);
            this.variables = this.variables.Select(v => v.Initialize(log, null)).ToList();
            return this;
        }

        /// <summary>
        /// Checks if the specified value is compatible with this type. A
        /// value is compatible if and only if it is an integer number value
        /// </summary>
        /// <param name="value">The value to check for compatibility</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public override bool IsCompatible(MibValue value)
        {
            return value is NumberValue;
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

            builder.Append("\n  Enterprise: ");
            builder.Append(this.enterprise);

            builder.Append("\n  Variables: ");
            builder.Append(this.variables);

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

            builder.Append("\n)");
            return builder.ToString();
        }
    }
}
