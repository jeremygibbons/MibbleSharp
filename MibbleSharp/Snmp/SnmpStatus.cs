// <copyright file="SnmpStatus.cs" company="None">
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
    /// An SNMP status value. This class is used to encapsulate the status
    /// value constants used in several SNMP macro types. Note that, due
    /// to the support for both SMIv1 and SMIv2, not all of the constants
    /// defined in this class can be present in all files. Please see the
    /// comments for each individual constant regarding the support for
    /// different SMI versions.
    /// </summary>
    public class SnmpStatus
    {
        /// <summary>
        /// The mandatory SNMP status. This status is only used in SMIv1.
        /// </summary>
        public static readonly SnmpStatus MANDATORY =
            new SnmpStatus("mandatory");

        /// <summary>
        /// The optional SNMP status. This status is only used in SMIv1.
        /// </summary>
        public static readonly SnmpStatus OPTIONAL =
            new SnmpStatus("optional");

        /// <summary>
        /// The current SNMP status. This status is only used in SMIv2
        /// and later.
        /// </summary>
        public static readonly SnmpStatus CURRENT =
            new SnmpStatus("current");

        /// <summary>
        /// The deprecated SNMP status. This status is only used in SMIv2
        /// and later.
        /// </summary>
        public static readonly SnmpStatus DEPRECATED =
            new SnmpStatus("deprecated");

        /// <summary>
        /// The obsolete SNMP status.
        /// </summary>
        public static readonly SnmpStatus OBSOLETE =
            new SnmpStatus("obsolete");

        /// <summary>
        /// The status description.
        /// </summary>
        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpStatus"/> class.
        /// </summary>
        /// <param name="description">The status description</param>
        private SnmpStatus(string description)
        {
            this.description = description;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return this.description;
        }
    }
}
