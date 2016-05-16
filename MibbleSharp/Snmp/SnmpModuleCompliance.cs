//
// SnmpModuleCompliance.cs
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
    ///
    /// <summary>
    /// The SNMP module compliance macro type. This macro type was added
    /// to SMIv2 and is defined in RFC 2580.
    /// </summary>
    /// <see cref="http://www.ietf.org/rfc/rfc2580.txt">RFC 2580 (SNMPv2-CONF)</see>
    ///
    public class SnmpModuleCompliance : SnmpType
    {

        /**
         * The type status.
         */
        private SnmpStatus status;

        /**
         * The type reference.
         */
        private string reference;

        /**
         * The list of modules.
         */
        private IList<SnmpModule> modules;

        /**
         * Creates a new SNMP module compliance type.
         *
         * @param status         the type status
         * @param description    the type description
         * @param reference      the type reference, or null
         * @param modules        the list of SNMP modules
         */
        public SnmpModuleCompliance(SnmpStatus status,
                                    string description,
                                    string reference,
                                    IList<SnmpModule> modules)
                : base("MODULE-COMPLIANCE", description)
        {
            this.status = status;
            this.reference = reference;
            this.modules = modules;
        }

        ///
        /// <summary>
        /// Initializes the MIB type. This will remove all levels of
        /// indirection present, such as references to types or values. No
        /// information is lost by this operation. This method may modify
        /// this object as a side-effect, and will return the basic
        /// type.
        /// </summary>
        /// <remarks>This is an internal method that should
        /// only be called by the MIB loader.</remarks>
        ///
        /// <param name="symbol">The MIB symbol containing this type</param>
        /// <param name="log">The MIB loader log</param>
        ///
        /// <returns>The basic MIB type</returns>
        ///
        /// <exception cref="MibException">If an error was encountered during
        /// the initialization
        /// </exception>
        ///
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {

            if (!(symbol is MibValueSymbol))
            {
                throw new MibException(symbol.getLocation(),
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

        /// <summary>
        /// The type status property
        /// </summary>
        /// <remarks>Readonly</remarks>
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
         * Returns the list of SNMP modules. The returned list will
         * consist of SnmpModule instances.
         *
         * @return the list of SNMP modules
         *
         * @see SnmpModule
         */
        public IEnumerable<SnmpModule> Modules
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
                builder.Append("\n  Module: ");
                builder.Append(m);
            }
            builder.Append("\n)");
            return builder.ToString();
        }
    }
}
