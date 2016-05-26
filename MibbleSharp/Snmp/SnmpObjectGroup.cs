// <copyright file="SnmpObjectGroup.cs" company="None">
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
    /// The SNMP object group macro type. This macro type was added
    /// to SMIv2 and is defined in RFC 2580.
    /// <see href="http://www.ietf.org/rfc/rfc2580.txt">RFC 2580 <c>(SNMPv2-CONF)</c></see>
    /// </summary>
    public class SnmpObjectGroup : SnmpType
    {
        /// <summary>
        /// The value objects.
        /// </summary>
        private IList<MibValue> objects;

        /// <summary>
        /// The object group status.
        /// </summary>
        private SnmpStatus status;

        /// <summary>
        /// The object group reference.
        /// </summary>
        private string reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpObjectGroup"/> class.
        /// </summary>
        /// <param name="objects">The value objects</param>
        /// <param name="status">The object group status</param>
        /// <param name="description">The object group description</param>
        /// <param name="reference">The object group reference</param>
        public SnmpObjectGroup(
            IList<MibValue> objects,
            SnmpStatus status,
            string description,
            string reference)
            : base("OBJECT-GROUP", description)
        {
            this.objects = objects;
            this.status = status;
            this.reference = reference;
        }

        /// <summary>
        /// Gets the value objects. The returned list will consist of
        /// MibValue instances.
        /// </summary>
        /// <see cref="MibValue"/>
        public IEnumerable<MibValue> Objects
        {
            get
            {
                return this.objects;
            }
        }

        /// <summary>
        /// Gets the object group status.
        /// </summary>
        public SnmpStatus Status
        {
            get
            {
                return this.status;
            }
        }

        /// <summary>
        /// Gets the object group reference.
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
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            if (!(symbol is MibValueSymbol))
            {
                throw new MibException(
                    symbol.Location,
                    "only values can have the " + Name + " type");
            }

            this.objects = this.objects.Select(o => o.Initialize(log, null)).ToList();
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
            builder.Append("\n  Objects: ");
            builder.Append(this.objects);
            builder.Append("\n  Status: ");
            builder.Append(this.status);
            builder.Append("\n  Description: ");
            builder.Append(this.GetDescription("               "));

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