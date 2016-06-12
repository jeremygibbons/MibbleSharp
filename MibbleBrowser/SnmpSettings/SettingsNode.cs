// <copyright file="SettingsNode.cs" company="None">
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
//    Copyright (c) 2016 Jeremy Gibbons. All rights reserved
// </copyright>

namespace MibbleBrowser.SnmpSettings
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable()]
    class SettingsNode : ISerializable
    {
        private SettingsNode parent;

        private List<SettingsNode> children = new List<SettingsNode>();

        public SettingsNode()
        {
            this.parent = null;
        }

        public SettingsNode(SettingsNode parent)
        {
            this.parent = parent;
        }

        public SettingsNode(SerializationInfo info, StreamingContext ctxt)
        {

        }

        /// <summary>
        /// The enumeration of SettingsNode types
        /// </summary>
        public enum SettingsNodeType
        {
            /// <summary>
            /// A Group stores settings for a set of Groups and Hosts.
            /// </summary>
            Group,

            /// <summary>
            /// A Host represents settings for an individual host device.
            /// It cannot have child nodes.
            /// </summary>
            Host
        }

        /// <summary>
        /// Gets the parent SettingsNode for this node
        /// </summary>
        public SettingsNode Parent
        {
            get; set;
        }

        /// <summary>
        /// Gets the list of child SettingsNode objects
        /// </summary>
        public IList<SettingsNode> Children
        {
            get
            {
                return children;
            }
        }

        /// <summary>
        /// Serializes this object
        /// </summary>
        /// <param name="info">The SerializationInfo object</param>
        /// <param name="ctxt">The StreamingContext object</param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {

        }
    }
}
