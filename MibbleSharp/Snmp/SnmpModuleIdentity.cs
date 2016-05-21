//
// SnmpModuleIdentity.cs
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
using MibbleSharp.Value;

namespace MibbleSharp.Snmp
{
    /**
     * The SNMP module identity macro type. This macro type was added
     * to SMIv2 and is defined in RFC 2578.
     *
     * @see <a href="http://www.ietf.org/rfc/rfc2578.txt">RFC 2578 (SNMPv2-SMI)</a>
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.6
     * @since    2.0
     */
    public class SnmpModuleIdentity : SnmpType
    {

        /**
         * The last updated date.
         */
        private string lastUpdated;

        /**
         * The organization name.
         */
        private string organization;

        /**
         * The organization contact information.
         */
        private string contactInfo;

        /**
         * The list of SNMP revision objects.
         */
        private IList<SnmpRevision> revisions;

        /**
         * Creates a new SNMP module identity.
         *
         * @param lastUpdated    the last updated date
         * @param organization   the organization name
         * @param contactInfo    the organization contact information
         * @param description    the module description
         * @param revisions      the list of module revisions
         */
        public SnmpModuleIdentity(string lastUpdated,
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
            foreach (var r in revisions)
            {
                r.Initialize(log);
            }
            return this;
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
         * Returns the last updated date.
         *
         * @return the last updated date
         */
        public string getLastUpdated()
        {
            return lastUpdated;
        }

        /**
         * Returns the organization name.
         *
         * @return the organization name
         */
        public string getOrganization()
        {
            return organization;
        }

        /**
         * Returns the organization contact information. Any unneeded
         * indentation will be removed from the text, and it also
         * replaces all tab characters with 8 spaces.
         *
         * @return the organization contact information
         *
         * @see #getUnformattedContactInfo()
         */
        public string getContactInfo()
        {
            return RemoveIndent(contactInfo);
        }

        /**
         * Returns the unformatted organization contact information. This
         * method returns the original MIB file content, without removing
         * unneeded indentation or similar.
         *
         * @return the unformatted organization contact information.
         *
         * @see #getContactInfo()
         *
         * @since 2.6
         */
        public string getUnformattedContactInfo()
        {
            return contactInfo;
        }

        /**
         * Returns a list of all the SNMP module revisions. The returned
         * list will consist of SnmpRevision instances.
         *
         * @return a list of all the SNMP module revisions
         *
         * @see SnmpRevision
         */
        public IEnumerable<SnmpRevision> getRevisions()
        {
            return revisions;
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
            builder.Append("\n  Last Updated: ");
            builder.Append(lastUpdated);
            builder.Append("\n  Organization: ");
            builder.Append(organization);
            builder.Append("\n  Contact Info: ");
            builder.Append(contactInfo);
            builder.Append("\n  Description: ");
            builder.Append(getDescription("               "));
            foreach (var rev in revisions)
            {
                builder.Append("\n  Revision: ");
                builder.Append(rev);
            }
            builder.Append("\n)");
            return builder.ToString();
        }
    }

}
