//
// SnmpModule.cs
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

namespace MibbleSharp.Snmp
{
    /// <summary>
    /// An SNMP module compliance value. This declaration is used inside
    /// the module compliance macro type.
    /// </summary>
    /// <see cref="SnmpModuleCompliance"/>
    public class SnmpModule
    {
        ///<summary>The module name</summary>
        private string module;

        ///
        /// <summary>The list of mandatory group values.</summary>
        ///
        private IList<MibValue> groups;

        ///
        /// <summary>The list of compliances.</summary>
        ///
        private IList<SnmpCompliance> compliances;

        ///
        /// <summary>The module comment.</summary>
        ///
        private string comment = null;

        ///
        /// <summary>Creates a new module compliance declaration.</summary>
        ///
        /// <param name="module">the module name, or null</param>
        /// <param name="groups">the list of mandatory group values</param>
        /// <param name="compliances">the list of compliances</param>
        ///
        public SnmpModule(string module,
                          IList<MibValue> groups,
                          IList<SnmpCompliance> compliances)
        {

            this.module = module;
            this.groups = groups;
            this.compliances = compliances;
        }

        ///
        /// <summary>
        /// Initializes the object. This will remove all levels of
        /// indirection present, such as references to other types, and
        /// returns the basic type. No type information is lost by this
        /// operation. This method may modify this object as a
        /// side-effect, and will be called by the MIB loader.
        /// </summary>
        /// <param name="log">the MIB loader log</param>
        ///
        /// <exception cref="MibException">
        /// if an error was encountered during
        /// the initialization
        /// </exception>
        ///
        public void Initialize(MibLoaderLog log)
        {
            groups = groups.Select(g => g.Initialize(log, null)).ToList();

            foreach (SnmpCompliance c in compliances)
                c.Initialize(log);
        }

        ///
        /// <value>The module name.</value>
        ///
        public string Module
        {
            get
            {
                return module;
            }
        }

        ///
        /// <summary>
        /// Returns the enumeration of mandatory group values. The returned list
        /// will consist of MibValue instances.
        /// </summary>
        /// 
        /// <value>The list of mandatory group values</value>
        ///
        /// <see cref="MibValue"/>
        ///
        public IEnumerable<MibValue> Groups
        {
            get
            {
                return groups;
            }
        }

        ///
        /// <summary>
        /// Returns the enumeration of compliances. The returned list will
        /// consist of SnmpCompliance instances.
        /// </summary>
        /// 
        /// <value>The list of SnmpCompliance instances as an IEnumerable</value>
        ///
        /// <see cref="SnmpCompliance"></see>
        ///
        public IEnumerable<SnmpCompliance> Compliances
        {
            get
            {
                return compliances;
            }
        }

        ///
        /// <summary>The module comment</summary>
        ///
        public string Comment
        {
            get
            {
                return comment;
            }

            set
            {
                if (module != null || !"THIS MODULE".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.comment = value;
                }
            }
        }
        /// <summary>
        /// Provides a string representation of an SnmpModule
        /// </summary>
        /// <returns>A string containing a representation of the SnmpModule</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (module != null)
            {
                builder.Append(module);
            }
            if (groups.Count > 0)
            {
                builder.Append("\n    Mandatory Groups: ");
                builder.Append(groups);
            }
            foreach(SnmpCompliance c in compliances)
            {
                builder.Append("\n    Module: ");
                builder.Append(c);
            }
            return builder.ToString();
        }
    }

}
