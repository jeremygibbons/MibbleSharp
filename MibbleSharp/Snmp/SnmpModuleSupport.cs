// <copyright file="SnmpModuleSupport.cs" company="None">
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

    /// <summary>
    /// An SNMP module support value. This declaration is used inside the
    /// agent capabilities type.
    /// </summary>
    /// <see cref="SnmpAgentCapabilities"/>
    public class SnmpModuleSupport
    {
        /// <summary>
        /// The module name.
        /// </summary>
        private string module;

        /// <summary>
        /// The list of included group values.
        /// </summary>
        private IList<MibValue> groups;

        /// <summary>
        /// The list of variations.
        /// </summary>
        private IList<SnmpVariation> variations;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpModuleSupport"/> class.
        /// </summary>
        /// <param name="module">The module name, or null</param>
        /// <param name="groups">The list of included group values</param>
        /// <param name="variations">The list of variations</param>
        public SnmpModuleSupport(
            string module,
            IList<MibValue> groups,
            IList<SnmpVariation> variations)
        {
            this.module = module;
            this.groups = groups;
            this.variations = variations;
        }

        /// <summary>
        /// Gets the module name
        /// </summary>
        public string Module
        {
            get
            {
                return this.module;
            }
        }

        /// <summary>
        /// Gets the list of included group values.
        /// </summary>
        /// <see cref="MibValue"/>
        public IEnumerable<MibValue> Groups
        {
            get
            {
                return this.groups;
            }
        }

        /// <summary>
        /// Gets the list of variations
        /// </summary>
        /// <see cref="SnmpVariation"/>
        public IEnumerable<SnmpVariation> Variations
        {
            get
            {
                return this.variations;
            }
        }

        /// <summary>
        /// Initializes the MIB type. This will remove all levels of
        /// indirection present, such as references to types or values. No
        /// information is lost by this operation. This method may modify
        /// this object as a side-effect.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="log">The MIB loader log</param>
        public void Initialize(MibLoaderLog log)
        {
            this.groups = this.groups.Select(g => g.Initialize(log, null)).ToList();

            foreach (var variation in this.variations)
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
                
        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (this.module != null)
            {
                builder.Append(this.module);
            }

            builder.Append("\n    Includes: ");
            builder.Append(this.groups);

            foreach (var variation in this.variations)
            {
                builder.Append("\n    Variation: ");
                builder.Append(variation);
            }

            return builder.ToString();
        }
    }
}
