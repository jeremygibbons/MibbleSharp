// <copyright file="SnmpModuleIdentity.cs" company="None">
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
    using MibbleSharp.Value;

    /// <summary>
    /// The SNMP module identity macro type. This macro type was added
    /// to SMIv2 and is defined in RFC 2578.
    /// <see href="http://www.ietf.org/rfc/rfc2578.txt">RFC 2578 <c>(SNMPv2-SMI)</c></see>
    /// </summary>
    public class SnmpModuleIdentity : SnmpType
    {
        /// <summary>
        /// The last updated date.
        /// </summary>
        private string lastUpdated;

        /// <summary>
        /// The organization name.
        /// </summary>
        private string organization;

        /// <summary>
        /// The organization contact information.
        /// </summary>
        private string contactInfo;

        /// <summary>
        /// The list of SNMP revision objects.
        /// </summary>
        private IList<SnmpRevision> revisions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpModuleIdentity"/> class.
        /// </summary>
        /// <param name="lastUpdated">The last updated date</param>
        /// <param name="organization">The organization name</param>
        /// <param name="contactInfo">The organization contact information</param>
        /// <param name="description">The module description</param>
        /// <param name="revisions">The module revisions</param>
        public SnmpModuleIdentity(
            string lastUpdated,
            string organization,
            string contactInfo,
            string description,
            IList<SnmpRevision> revisions)
            : base("MODULE-IDENTITY", description)
        {
            this.lastUpdated = lastUpdated;
            this.organization = organization;
            this.contactInfo = contactInfo;
            this.revisions = revisions;
        }

        /// <summary>
        /// Gets the last updated date.
        /// </summary>
        public string LastUpdated
        {
            get
            {
                return this.lastUpdated;
            }
        }

        /// <summary>
        /// Gets the organization name
        /// </summary>
        public string Organization
        {
            get
            {
                return this.organization;
            }
        }

        /// <summary>
        /// Gets the organization contact information. Any unneeded
        /// indentation will be removed from the text, and it also
        /// replaces all tab characters with 8 spaces.
        /// </summary>
        /// <see cref="UnformattedContactInfo"/>
        public string ContactInfo
        {
            get
            {
                return SnmpType.RemoveIndent(this.contactInfo);
            }
        }

        /// <summary>
        /// Gets the unformatted organization contact information. This
        /// method returns the original MIB file content, without removing
        /// unneeded indentation or similar.
        /// </summary>
        /// <see cref="ContactInfo"/>
        public string UnformattedContactInfo
        {
            get
            {
                return this.contactInfo;
            }
        }

        /// <summary>
        /// Gets a list of all the SNMP module revisions. The returned
        /// list will consist of <c>SnmpRevision</c> instances.
        /// </summary>
        public IEnumerable<SnmpRevision> Revisions
        {
            get
            {
                return this.revisions;
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
                    "only values can have the " + this.Name + " type");
            }

            foreach (var r in this.revisions)
            {
                r.Initialize(log);
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
            builder.Append("\n  Last Updated: ");
            builder.Append(this.lastUpdated);
            builder.Append("\n  Organization: ");
            builder.Append(this.organization);
            builder.Append("\n  Contact Info: ");
            builder.Append(this.contactInfo);
            builder.Append("\n  Description: ");
            builder.Append(this.getDescription("               "));

            foreach (var rev in this.revisions)
            {
                builder.Append("\n  Revision: ");
                builder.Append(rev);
            }

            builder.Append("\n)");
            return builder.ToString();
        }
    }
}