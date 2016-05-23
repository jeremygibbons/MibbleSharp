// <copyright file="SnmpAccess.cs" company="None">
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
    /// An SNMP access mode value. This class is used to encapsulate the
    /// access value constants used in several SNMP macro types. Note that
    /// due to the support for both SMIv1 and SMIv2 not all of the
    /// constants defined in this class can be present in all files.
    /// Please see the comments for each individual constant regarding the
    /// support for different SNMP versions.
    /// </summary>
    public class SnmpAccess
    {
        /// <summary>
        /// The not implemented SNMP access mode. This mode is only used
        /// in SMIv2 variation declarations inside an agent capabilities
        /// declaration.
        /// </summary>
        public static readonly SnmpAccess NotImplemented = 
            new SnmpAccess("not-implemented");

        /// <summary>
        /// The not accessible SNMP access mode.
        /// </summary>
        public static readonly SnmpAccess NotAccessible =
            new SnmpAccess("not-accessible");

        /// <summary>
        /// The accessible for notify SNMP access mode. This mode is only
        /// used in SMIv2.
        /// </summary>
        public static readonly SnmpAccess AccessibleForNotify =
            new SnmpAccess("accessible-for-notify");

        /// <summary>
        /// The read-only SNMP access mode.
        /// </summary>
        public static readonly SnmpAccess ReadOnly =
            new SnmpAccess("read-only");

        /// <summary>
        /// The read-write SNMP access mode.
        /// </summary>
        public static readonly SnmpAccess ReadWrite =
            new SnmpAccess("read-write");

        /// <summary>
        /// The read-create SNMP access mode. This mode is only used in
        /// SMIv2.
        /// </summary>
        public static readonly SnmpAccess ReadCreate =
            new SnmpAccess("read-create");

        /// <summary>
        /// The write-only SNMP access mode. This mode is only used in
        /// SMIv1.
        /// </summary>
        public static readonly SnmpAccess WriteOnly =
            new SnmpAccess("write-only");

        /// <summary>
        /// The access mode description.
        /// </summary>
        private string description;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpAccess"/> class.
        /// </summary>
        /// <param name="description">The access mode description</param>
        private SnmpAccess(string description)
        {
            this.description = description;
        }

        /// <summary>
        /// Gets a value indicating whether reading is allowed
        /// </summary>
        public bool CanRead
        {
            get
            {
                return this == SnmpAccess.ReadOnly
                    || this == SnmpAccess.ReadWrite
                    || this == SnmpAccess.ReadCreate;
            }
        }

        /// <summary>
        /// Gets a value indicating whether writing is allowed
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return this == SnmpAccess.ReadWrite
                    || this == SnmpAccess.ReadCreate
                    || this == SnmpAccess.WriteOnly;
            }
        }

        /// <summary>
        /// Returns a string representation of the <c>SnmpAccess</c> object
        /// </summary>
        /// <returns>A string representation of the <c>SnmpAccess</c> object</returns>
        public override string ToString()
        {
            return this.description;
        }
    }
}
