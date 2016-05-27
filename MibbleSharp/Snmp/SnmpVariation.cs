// <copyright file="SnmpVariation.cs" company="None">
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
    /// An SNMP module variation value. This declaration is used inside a
    /// module support declaration.
    /// </summary>
    /// <see cref="SnmpModuleSupport"/>
    public class SnmpVariation
    {
        /// <summary>
        /// The variation value.
        /// </summary>
        private MibValue value;

        /// <summary>
        /// The value syntax.
        /// </summary>
        private MibType syntax;

        /// <summary>
        /// The value write syntax.
        /// </summary>
        private MibType writeSyntax;

        /// <summary>
        /// The access mode.
        /// </summary>
        private SnmpAccess access;

        /// <summary>
        /// The cell values required for creation.
        /// </summary>
        private IList<MibValue> requiredCells;

        /// <summary>
        /// The default value.
        /// </summary>
        private MibValue defaultValue;

        /// <summary>
        /// The variation description.
        /// </summary>
        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpVariation"/> class.
        /// </summary>
        /// <param name="value">the variation value</param>
        /// <param name="syntax">the value syntax, or null</param>
        /// <param name="writeSyntax">the value write syntax, or null</param>
        /// <param name="access">the access mode, or null</param>
        /// <param name="requiredCells">the cell values required for creation</param>
        /// <param name="defaultValue">the default value, or null</param>
        /// <param name="description">the variation description</param>
        public SnmpVariation(
            MibValue value,
            MibType syntax,
            MibType writeSyntax,
            SnmpAccess access,
            IList<MibValue> requiredCells,
            MibValue defaultValue,
            string description)
        {
            this.value = value;
            this.syntax = syntax;
            this.writeSyntax = writeSyntax;
            this.access = access;
            this.requiredCells = requiredCells;
            this.defaultValue = defaultValue;
            this.description = description;
        }

        /// <summary>
        /// Gets the base symbol that this variation applies to.
        /// </summary>
        public MibValueSymbol BaseSymbol
        {
            get
            {
                if (this.value is ObjectIdentifierValue)
                {
                    return ((ObjectIdentifierValue)this.value).getSymbol();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the value
        /// </summary>
        /// <returns></returns>
        public MibValue Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Gets the value syntax
        /// </summary>
        public MibType Syntax
        {
            get
            {
                return this.syntax;
            }
        }

        /// <summary>
        /// Gets the value write syntax
        /// </summary>
        public MibType WriteSyntax
        {
            get
            {
                return this.writeSyntax;
            }
        }

        /// <summary>
        /// Gets the access mode
        /// </summary>
        public SnmpAccess Access
        {
            get
            {
                return this.access;
            }
        }

        /// <summary>
        /// Gets cell values required for creation. The returned list
        /// will consist of MibValue instances.
        /// </summary>
        /// <see cref="MibValue"/>
        public IList<MibValue> RequiredCells
        {
            get
            {
                return this.requiredCells;
            }
        }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        public MibValue DefaultValue
        {
            get
            {
                return this.defaultValue;
            }
        }

        /// <summary>
        /// Gets the variation description
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
        }
        
        /// <summary>
        /// Initializes the object. This will remove all levels of
        /// indirection present, such as references to other types, and
        /// returns the basic type. No information is lost by this operation. 
        /// This method may modify this object as a side-effect, and will 
        /// be called by the MIB loader.
        /// </summary>
        /// <param name="log">The MIB loader log</param>
        /// <exception cref="MibException">
        /// If an error occurred during initialization
        /// </exception>
        public void Initialize(MibLoaderLog log)
        {
            MibType type = null;

            this.value = this.value.Initialize(log, null);

            if (this.BaseSymbol != null)
            {
                // TODO: use utility function to retrieve correct base type here
                type = this.BaseSymbol.Type;

                if (type is SnmpTextualConvention)
                {
                    type = ((SnmpTextualConvention)type).Syntax;
                }

                if (type is SnmpObjectType)
                {
                    type = ((SnmpObjectType)type).Syntax;
                }
            }

            if (this.syntax != null)
            {
                this.syntax = this.syntax.Initialize(null, log);
            }

            if (this.writeSyntax != null)
            {
                this.writeSyntax = this.writeSyntax.Initialize(null, log);
            }

            this.requiredCells = this.requiredCells.Select(rc => rc.Initialize(log, type)).ToList();

            if (this.defaultValue != null)
            {
                this.defaultValue = this.defaultValue.Initialize(log, type);
            }
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(this.value);

            if (this.syntax != null)
            {
                builder.Append("\n      Syntax: ");
                builder.Append(this.syntax);
            }

            if (this.writeSyntax != null)
            {
                builder.Append("\n      Write-Syntax: ");
                builder.Append(this.writeSyntax);
            }

            if (this.access != null)
            {
                builder.Append("\n      Access: ");
                builder.Append(this.access);
            }

            if (this.requiredCells.Count > 0)
            {
                builder.Append("\n      Creation-Requires: ");
                builder.Append(this.requiredCells);
            }

            if (this.defaultValue != null)
            {
                builder.Append("\n      Default Value: ");
                builder.Append(this.defaultValue);
            }

            builder.Append("\n      Description: ");
            builder.Append(this.description);

            return builder.ToString();
        }
    }
}