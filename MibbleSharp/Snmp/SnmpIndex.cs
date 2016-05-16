/*
 * SnmpIndex.cs
 *
 * This work is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation; either version 2 of the License,
 * or (at your option) any later version.
 *
 * This work is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
 * USA
 *
 * Original Java code Copyright (c) 2004-2016 Per Cederberg. All
 * rights reserved.
 * C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights
 * reserved
 */

using System;
using System.Text;

namespace MibbleSharp.Snmp
{
    ///<summary>
    ///An SNMP index object. This declaration is used inside an object
    /// type index declaration.An index contains either a type or a
    /// value. Indices based on values may be implied.
    /// </summary>
    /// <see cref="SnmpObjectType"/>
    public class SnmpIndex
    {

        ///
        /// <summary>The implied flag</summary>
        ///
        private bool implied;

        /**
         * The index value, or null.
         */
        private MibValue value;

        /**
         * The index type, or null.
         */
        private MibType type;

        ///
        ///<summary>Creates a new SNMP index. Exactly one of the value or type
        /// arguments are supposed to be non-null.</summary> 
        ///
        /// <param name="implied">the implied flag</param>
        /// <param name="value">the index value, or null</param>
        /// <param name="type">the index type, or null</param>
        ///
        public SnmpIndex(bool implied, MibValue value, MibType type)
        {
            this.implied = implied;
            this.value = value;
            this.type = type;
        }
        
        /// <summary>
        /// Initializes the object. This will remove all levels of
        /// indirection present, such as references to other types and
        /// values.No information is lost by this operation.This method
        /// may modify this object as a side-effect, and will be called by
        /// the MIB loader.
        /// </summary>
        /// <param name="symbol">the MIB symbol containing this object</param>
        /// <param name="log">the MIB loader log</param>
        /// <exception cref="MibException">if an error was encountered
        /// during the initialization</exception>
        public void Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            if (value != null)
            {
                value = value.Initialize(log, null);
            }
            if (type != null)
            {
                type = type.Initialize(symbol, log);
            }
        }

        /// <summary>
        /// Checks if this index is an implied value. If this is true, the
        /// index also represents a value index.
        /// </summary>
        /// <value>
        /// true if the index is an implied value,
        /// or false otherwise
        /// </value>
        ///
        public bool Implied
        {
            get
            {
                return implied;
            }
        }

        ///
        /// <summary>Returns the index value if present.</summary>
        ///
        /// <value>the index value, or null if not applicable</value>
        ///
        /// <see cref="Type"/>
        ///
        public MibValue Value
        {
            get
            {
                return value;
            }
        }

        ///
        /// <summary>Returns the index type if present.</summary>
        ///
        /// <value>the index type, or null if not applicable</value>
        ///
        ///<see cref="Value"/>
        ///
        public MibType Type
        {
            get
            {
                return type;
            }
        }

        ///
        /// <summary>Returns the index type or value.</summary>
        ///
        /// <returns>Returns the value or index type, depending on which is non-null</returns>
        ///
        /// <see cref="MibValue"/>
        /// <see cref="MibType"/>
        ///
        public Object getTypeOrValue()
        {
            if (value != null)
            {
                return value;
            }
            else
            {
                return type;
            }
        }

        ///
        /// <summary>Returns a string representation of this object.</summary>
        /// 
        /// <returns>a string representation of this object</returns>
        ///
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (implied)
            {
                builder.Append("IMPLIED ");
            }
            builder.Append(getTypeOrValue());
            return builder.ToString();
        }
    }

}
