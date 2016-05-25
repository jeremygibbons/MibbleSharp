// <copyright file="SnmpModule.cs" company="None">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// An SNMP module compliance value. This declaration is used inside
    /// the module compliance macro type.
    /// </summary>
    /// <see cref="SnmpModuleCompliance"/>
    public class SnmpModule
    {
        /// <summary>The module name</summary>
        private string module;

        /// <summary>The list of mandatory group values.</summary>
        private IList<MibValue> groups;

        /// <summary>The list of compliances.</summary>
        private IList<SnmpCompliance> compliances;

        /// <summary>The module comment.</summary>
        private string comment = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpModule"/> class.
        /// </summary>
        /// <param name="module">the module name, or null</param>
        /// <param name="groups">the list of mandatory group values</param>
        /// <param name="compliances">the list of compliances</param>
        public SnmpModule(
            string module,
            IList<MibValue> groups,
            IList<SnmpCompliance> compliances)
        {
            this.module = module;
            this.groups = groups;
            this.compliances = compliances;
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
        /// Gets the enumeration of mandatory group values. The returned list
        /// will consist of MibValue instances.
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
        /// Gets the enumeration of compliances. The returned list will
        /// consist of <c>SnmpCompliance</c> instances.
        /// </summary>
        /// <see cref="SnmpCompliance"></see>
        public IEnumerable<SnmpCompliance> Compliances
        {
            get
            {
                return this.compliances;
            }
        }

        /// <summary>
        /// Gets or sets the module comment
        /// </summary>
        public string Comment
        {
            get
            {
                return this.comment;
            }

            set
            {
                if (this.module != null || !"THIS MODULE".Equals(value, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.comment = value;
                }
            }
        }

        /// <summary>
        /// Initializes the object. This will remove all levels of
        /// indirection present, such as references to other types, and
        /// returns the basic type. No type information is lost by this
        /// operation. This method may modify this object as a
        /// side-effect, and will be called by the MIB loader.
        /// </summary>
        /// <param name="log">the MIB loader log</param>
        /// <exception cref="MibException">
        /// if an error was encountered during
        /// the initialization
        /// </exception>
        public void Initialize(MibLoaderLog log)
        {
            this.groups = this.groups.Select(g => g.Initialize(log, null)).ToList();

            foreach (SnmpCompliance c in this.compliances)
            {
                c.Initialize(log);
            }    
        }

        /// <summary>
        /// Provides a string representation of an <c>SnmpModule</c>
        /// </summary>
        /// <returns>
        /// A string containing a representation of the <c>SnmpModule</c>
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (this.module != null)
            {
                builder.Append(this.module);
            }

            if (this.groups.Count > 0)
            {
                builder.Append("\n    Mandatory Groups: ");
                builder.Append(this.groups);
            }

            foreach (SnmpCompliance c in this.compliances)
            {
                builder.Append("\n    Module: ");
                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
