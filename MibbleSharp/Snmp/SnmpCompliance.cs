// <copyright file="SnmpCompliance.cs" company="None">
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
    using System.Text;

    /// <summary>
    /// An SNMP module compliance value. This declaration is used inside a
    /// module declaration for both the GROUP and OBJECT compliance parts.
    /// </summary>
    /// <see cref="SnmpModule"/>
    public class SnmpCompliance
    {
        /// <summary>The compliance group flag. </summary>
        private bool group;

        /// <summary>The compliance value.</summary>
        private MibValue value;

        /// <summary>The value syntax.</summary>
        private MibType syntax;

        /// <summary>The value write syntax.</summary>
        private MibType writeSyntax;

        /// <summary>The access mode.</summary>
        private SnmpAccess access;

        /// <summary>The compliance description.</summary>
        private string description;

        /// <summary>The compliance comment.</summary>
        private string comment = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpCompliance"/> class.
        /// </summary>
        /// <param name="group">The group compliance flag</param>
        /// <param name="value">The compliance value</param>
        /// <param name="syntax">The value syntax, or null</param>
        /// <param name="writeSyntax">The value write syntax, or null</param>
        /// <param name="access">The access mode, or null</param>
        /// <param name="description">The compliance description</param>
        public SnmpCompliance(
            bool group,
            MibValue value,
            MibType syntax,
            MibType writeSyntax,
            SnmpAccess access,
            string description)
        {
            this.group = group;
            this.value = value;
            this.syntax = syntax;
            this.writeSyntax = writeSyntax;
            this.access = access;
            this.description = description;
        }

        /// <summary>
        /// Gets a value indicating whether this is a group compliance
        /// </summary>
        public bool IsGroup
        {
            get
            {
                return this.group;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is an object compliance
        /// </summary>
        public bool IsObject
        {
            get
            {
                return !this.group;
            }
        }

        /// <summary>
        /// Gets the value
        /// </summary>
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
        /// <see cref="SnmpAccess"/>
        public SnmpAccess Access
        {
            get
            {
                return this.access;
            }
        }

        /// <summary>
        /// Gets the compliance description. Any unneeded indentation
        /// will be removed from the description, and it also replaces all
        /// tab characters with 8 spaces.
        /// </summary>
        /// <see cref="UnformattedDescription"/>
        public string Description
        {
            get
            {
                return SnmpType.RemoveIndent(this.description);
            }
        }

        /// <summary>
        /// Gets the unformatted compliance description. This method
        /// returns the original MIB file text, without removing unneeded
        /// indentation or similar.
        /// </summary>
        /// <see cref="Description"/>
        public string UnformattedDescription
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Gets or sets the compliance comment
        /// </summary>
        public string Comment
        {
            get
            {
                return this.comment;
            }

            set
            {
                this.comment = value;
            }
        }

        /// <summary>
        /// Initializes this object. This will remove all levels of
        /// indirection present, such as references to other types, and
        /// returns the basic type. No type information is lost by this
        /// operation. This method may modify this object as a
        /// side-effect, and will be called by the MIB loader.
        /// </summary>
        /// <param name="log">The MIB loader log</param>
        /// <exception cref="MibException">
        /// If an error was encountered during the initialization
        /// </exception>
        public void Initialize(MibLoaderLog log)
        {
            this.value = this.value.Initialize(log, null);

            if (this.syntax != null)
            {
                this.syntax = this.syntax.Initialize(null, log);
            }

            if (this.writeSyntax != null)
            {
                this.writeSyntax = this.writeSyntax.Initialize(null, log);
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

            builder.Append("\n      Description: ");
            builder.Append(this.description);
            return builder.ToString();
        }
    }
}
