// <copyright file="SnmpRevision.cs" company="None">
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
    /// <summary>
    /// An SNMP module identity revision value.  This declaration is used
    /// inside a module identity macro type.
    /// </summary>
    /// <see cref="SnmpModuleIdentity"/>
    public class SnmpRevision
    {
        /// <summary>
        /// The revision number
        /// </summary>
        private MibValue value;

        /// <summary>
        /// The revision description.
        /// </summary>
        private string description;

        /// <summary>
        /// The revision comment.
        /// </summary>
        private string comment = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpRevision"/> class,
        /// i.e. a new SNMP module identity revision.
        /// </summary>
        /// <param name="value">The revision number</param>
        /// <param name="description">The revision description</param>
        public SnmpRevision(MibValue value, string description)
        {
            this.value = value;
            this.description = description;
        }

        /// <summary>
        /// Gets the revision number
        /// </summary>
        public MibValue Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Gets the revision description. Any unneeded indentation
        /// will be removed from the description, and it also replaces
        /// all tab characters with 8 spaces.
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
        /// Gets the unformatted revision description. This method
        /// returns the original MIB file description, without removing
        /// unneeded indentation or similar.
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
        /// Gets or sets the revision comment
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
        /// Initializes the MIB revision number. This will remove all
        /// levels of indirection present. No information is lost by this
        /// operation. This method may modify this object as a
        /// side-effect, and will be called by the MIB loader.
        /// </summary>
        /// <param name="log">The MIB loader log</param>
        /// <exception cref="MibException">
        /// If an error was encountered during initialization
        /// </exception>
        public void Initialize(MibLoaderLog log)
        {
            this.value = this.value.Initialize(log, null);
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns> 
        public override string ToString()
        {
            return this.value.ToString() + " (" + this.description + ")";
        }
    }
}