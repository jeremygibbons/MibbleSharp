//
// NamedNumber.cs
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

using System.Numerics;
using MibbleSharp.Value;

namespace MibbleSharp
{
    /**
    * A named number. This class is used for storing intermediate values
    * during the parsing.
    *
    * @author   Per Cederberg, <per at percederberg dot net>
    * @version  2.0
    * @since    2.0
    */

    class NamedNumber
    {
        /**
         * The value name.
         */
        private string name = null;

        /**
         * The numeric value.
         */
        private BigInteger? number = null;

        /**
         * The value reference.
         */
        private ValueReference reference = null;

        /**
         * Creates a new named number.
         *
         * @param number         the numeric value
         */
        public NamedNumber(BigInteger number) : this(null, number)
        {

        }

        /**
         * Creates a new named number.
         *
         * @param name           the value name
         * @param number         the numeric value
         */
        public NamedNumber(string name, BigInteger number)
        {
            this.name = name;
            this.number = number;
        }

        /**
         * Creates a new named number.
         *
         * @param reference      the value reference
         */
        public NamedNumber(ValueReference reference) : this(null, reference)
        {
          
        }

        /**
         * Creates a new named number.
         *
         * @param name           the value name
         * @param reference      the value reference
         */
        public NamedNumber(string name, ValueReference reference)
        {
            this.name = name;
            this.reference = reference;
        }

        /**
         * Checks if this named number has a name component.
         *
         * @return true if this named number has a name component, or
         *         false otherwise
         */
        public bool hasName()
        {
            return name != null;
        }

        /**
         * Checks if this named number has a number component.
         *
         * @return true if this named number has a number component, or
         *         false otherwise
         */
        public bool hasNumber()
        {
            return number != null;
        }

        /**
         * Checks if this named number has a value reference.
         *
         * @return true if this named number has a value reference, or
         *         false otherwise
         */
        public bool HasReference
        {
            get
            {
                return reference != null;
            }
        }

        /**
         * Returns the value name.
         *
         * @return the value name
         */
        public string Name
        {
            get
            {
                return name;
            }
        }

        /**
         * Returns the numeric value.
         *
         * @return the numeric value
         */
        public BigInteger Number
        {
            get
            {
                if (number == null)
                    return 0;
                return (BigInteger) number;
            }
        }

        public int IntValue
        {
            get
            {
                if (number > int.MaxValue)
                    throw new System.OverflowException();
                return (int)number;
            }
        }

        /**
         * Returns the value reference.
         *
         * @return the value reference
         */
        public ValueReference Reference
        {
            get
            {
                return reference;
            }
        }
    }
}

