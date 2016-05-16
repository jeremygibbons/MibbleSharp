//
// SnmpModuleSupport.cs
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

namespace MibbleSharp.Snmp
{

    /**
     * An SNMP module support value. This declaration is used inside the
     * agent capabilities type.
     *
     * @see SnmpAgentCapabilities
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class SnmpModuleSupport
    {

        /**
         * The module name.
         */
        private string module;

        /**
         * The list of included group values.
         */
        private IList<MibValue> groups;

        /**
         * The list of variations.
         */
        private IList<SnmpVariation> variations;

        /**
         * Creates a new module support declaration.
         *
         * @param module         the module name, or null
         * @param groups         the list of included group values
         * @param variations     the list of variations
         */
        public SnmpModuleSupport(string module,
                                 IList<MibValue> groups,
                                 IList<SnmpVariation> variations)
        {

            this.module = module;
            this.groups = groups;
            this.variations = variations;
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
            groups = groups.Select(g => g.Initialize(log, null)).ToList();

            foreach (var variation in variations)
            {
                try
                {
                    variation.Initialize(log);
                }
                catch (MibException e)
                {
                    log.AddError(e.Location, e.Message);
                }
            }
        }

        /**
         * Returns the module name.
         *
         * @return the module name, or
         *         null if not set
         */
        public string getModule()
        {
            return module;
        }

        /**
         * Returns the list of included group values. The returned list
         * will consist of MibValue instances.
         *
         * @return the list of included group values
         *
         * @see net.percederberg.mibble.MibValue
         */
        public IEnumerable<MibValue> getGroups()
        {
            return groups;
        }

        /**
         * Returns the list of variations. The returned list will consist
         * of SnmpVariation instances.
         *
         * @return the list of variations
         *
         * @see SnmpVariation
         */
        public IEnumerable<SnmpVariation> getVariations()
        {
            return variations;
        }

        /**
         * Returns a string representation of this object.
         *
         * @return a string representation of this object
         */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (module != null)
            {
                builder.Append(module);
            }
            builder.Append("\n    Includes: ");
            builder.Append(groups);
            foreach (var variation in variations)
            {
                builder.Append("\n    Variation: ");
                builder.Append(variation);
            }

            return builder.ToString();
        }
    }

}
