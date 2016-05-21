//
// SnmpAgentCapabilities.cs
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
using System.Text;
using MibbleSharp.Value;

namespace MibbleSharp.Snmp
{
    /**
     * The SNMP agent capabilities macro type. This macro type was added
     * to SMIv2 and is defined in RFC 2580.
     *
     * @see <a href="http://www.ietf.org/rfc/rfc2580.txt">RFC 2580 (SNMPv2-CONF)</a>
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.5
     * @since    2.0
     */
    public class SnmpAgentCapabilities : SnmpType
    {

        /**
         * The product release.
         */
        private string productRelease;

        /**
         * The type status.
         */
        private SnmpStatus status;

        /**
         * The type reference.
         */
        private string reference;

        /**
         * The list of supported modules.
         */
        private IList<SnmpModuleSupport> modules;

        /**
         * Creates a new agent capabilities.
         *
         * @param productRelease the product release
         * @param status         the type status
         * @param description    the type description
         * @param reference      the type reference, or null
         * @param modules        the list of supported modules
         */
        public SnmpAgentCapabilities(string productRelease,
                                     SnmpStatus status,
                                     string description,
                                     string reference,
                                     IList<SnmpModuleSupport> modules)
                : base("AGENT-CAPABILITIES", description)
        {
            this.productRelease = productRelease;
            this.status = status;
            this.reference = reference;
            this.modules = modules;
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
            foreach (var m in modules)
                m.Initialize(log);
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
         * Returns the product release.
         *
         * @return the product release
         */
        public string ProductRelease
        {
            get
            {
                return productRelease;
            }
        }

        /**
         * Returns the type status.
         *
         * @return the type status
         */
        public SnmpStatus Status
        {
            get
            {
                return status;
            }
        }

        /**
         * Returns the type reference.
         *
         * @return the type reference, or
         *         null if no reference has been set
         */
        public string Reference
        {
            get
            {
                return reference;
            }
        }

        /**
         * Returns the list of the supported modules. The returned list
         * will consist of SnmpModuleSupport instances.
         *
         * @return the list of the supported modules
         *
         * @see SnmpModuleSupport
         */
        public IEnumerable<SnmpModuleSupport> Modules
        {
            get
            {
                return modules;
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

            builder.Append(base.ToString());
            builder.Append(" (");
            builder.Append("\n  Product Release: ");
            builder.Append(productRelease);
            builder.Append("\n  Status: ");
            builder.Append(status);
            builder.Append("\n  Description: ");
            builder.Append(getDescription("               "));
            if (reference != null)
            {
                builder.Append("\n  Reference: ");
                builder.Append(reference);
            }
            foreach (var m in modules)
            {
                builder.Append("\n  Supports Module: ");
                builder.Append(m);
            }
            builder.Append("\n)");
            return builder.ToString();
        }
    }

}
