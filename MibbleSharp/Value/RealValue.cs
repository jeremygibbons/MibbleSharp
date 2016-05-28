//
// RealValue.cs
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
using MibbleSharp.Type;

namespace MibbleSharp.Value
{
    class RealValue : MibValue
    {
        private double value;

        /**
        * Creates a new number value.
        *
        * @param value          the number value
        */
        public RealValue(double value) : base("Real")
        {
            this.value = value;
        }

        /**
         * Initializes the MIB value. This will remove all levels of
         * indirection present, such as references to other values. No
         * value information is lost by this operation. This method may
         * modify this object as a side-effect, and will return the basic
         * value.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @param log            the MIB loader log
         * @param type           the value type
         *
         * @return the basic MIB value
         */
        public override MibValue Initialize(MibLoaderLog log, MibType type)
        {
            return this;
        }

        /**
         * Creates a value reference to this value. The value reference
         * is normally an identical value. Only certain values support
         * being referenced, and the default implementation of this
         * method throws an exception.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @return the MIB value reference
         *
         * @since 2.2
         */
        public override MibValue CreateReference()
        {
            return new RealValue(value);
        }


        public override int CompareTo(MibValue m)
        {
            NumberValue nv = m as NumberValue;
            if (nv != null)
                return compareToNumber((double) nv.value);

            return ToString().CompareTo(m);
        }

        /**
         * Compares this object with the specified number for order.
         *
         * @param num            the number to compare to
         *
         * @return less than zero if this number is less than the specified,
         *         zero if the numbers are equal, or
         *         greater than zero otherwise
         */
        private int compareToNumber(double num)
        {
            return (int)(value - num);
        }

        /**
         * Checks if this object equals another object. This method will
         * compare the string representations for equality.
         *
         * @param obj            the object to compare with
         *
         * @return true if the objects are equal, or
         *         false otherwise
         */
        public override bool Equals(Object obj)
        {
            MibValue mv = obj as MibValue;
            if (mv == null)
                return false;
            return CompareTo(mv) == 0;
        }

        /**
         * Returns a hash code for this object.
         *
         * @return a hash code for this object
         */
        public override int GetHashCode()
        {
            return (int)value;
        }

        public double Value
        {
            get
            {
                return value;
            }
        }

        /**
         * Returns a string representation of this value.
         *
         * @return a string representation of this value
         */
        public override string ToString()
        {
            return value.ToString();
        }

        /**
         * Returns the number of bytes required by the specified type and
         * initial value size. If the type has no size requirement
         * specified, a value of one (1) will always be returned. If the
         * type size constraint allows for zero length, a zero might also
         * be returned.
         *
         * @param type           the MIB value type
         * @param initialBytes   the initial number of bytes used
         *
         * @return the number of bytes required
         */
        protected int getByteSize(MibType type, int initialBytes)
        {
            IConstraint c = null;
            int res = -1;

            if (type is StringType)
            {
                c = ((StringType)type).Constraint;
            }
            if (c is SizeConstraint)
            {
                res = ((SizeConstraint)c).NextValue(initialBytes);
            }
            if (res < 0)
            {
                res = 1;
            }
            return res;
        }

    }
}
