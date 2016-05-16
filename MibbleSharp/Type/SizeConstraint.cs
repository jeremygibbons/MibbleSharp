//
// SizeConstraint.cs
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
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using MibbleSharp.Value;

namespace MibbleSharp.Type
{
    /**
     * A MIB type size constraint.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.9
     * @since    2.0
     */
    public class SizeConstraint : Constraint
    {

        /**
         * The constraint location. This value is reset to null once the
         * constraint has been initialized. 
         */
        private FileLocation location;

        /**
         * The constrained size values.
         */
        private Constraint values;

        /**
         * Creates a new size constraint.
         *
         * @param location       the constraint location
         * @param values         the constrained size values
         */
        public SizeConstraint(FileLocation location, Constraint values)
        {
            this.location = location;
            this.values = values;
        }

        /**
         * Initializes the constraint. This will remove all levels of
         * indirection present, such as references to types or values. No
         * constraint information is lost by this operation. This method
         * may modify this object as a side-effect, and will be called by
         * the MIB loader.
         *
         * @param type           the type to constrain
         * @param log            the MIB loader log
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        public void Initialize(MibType type, MibLoaderLog log)
        {

            string message;

            values.Initialize(new IntegerType(), log);
            if (location != null && !IsCompatible(type))
            {
                message = "Size constraint not compatible with this type";
                log.AddWarning(location, message);
            }
            location = null;
        }

        /**
         * Checks if the specified type is compatible with this
         * constraint.
         *
         * @param type            the type to check
         *
         * @return true if the type is compatible, or
         *         false otherwise
         */
        public bool IsCompatible(MibType type)
        {
            return type is SequenceOfType
                    || type is StringType;
        }

        /**
         * Checks if the specified value is compatible with this
         * constraint. Only octet string values can be compatible with a
         * size constraint, and only if the string length is compatible
         * with the value range in the size constraint.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        public bool IsCompatible(MibValue value)
        {
            int size;

            if (value is StringValue)
            {
                size = value.ToString().Length;
                return values.IsCompatible(new NumberValue((ulong) size));
            }
            return false;
        }

        /**
         * Returns a list of the value constraints on the size.
         *
         * @return a list of the value constraints
         */
        public IEnumerable<Constraint> getValues()
        {

            if (values is CompoundConstraint)
            {
                return ((CompoundConstraint)values).getConstraintList();
            }
            else
            {
                List<Constraint> l = new List<Constraint>();
                l.Add(values);
                return l;
            }
        }

        /**
         * Returns the next compatible size constraint value from a start
         * value. The values will be enumerated from lower values to
         * higher.
         *
         * @param start          the initial start value
         *
         * @return the next compatible value, or
         *         -1 if no such value exists
         *
         * @since 2.9
         */
        public int nextValue(int start)
        {
            IEnumerable<Constraint> list = getValues();
            BigInteger val;
            // TODO: the constraint list should be sorted
            foreach(var c in list)
            {
                ValueConstraint vc = c as ValueConstraint;
                if (vc != null)
                {
                    NumberValue nv = vc.getValue() as NumberValue;
                    if (nv != null)
                        val = nv.value;
                    else
                        val = 0; //TODO: need to handle this diferently
                }
                else
                {
                    ValueRangeConstraint vrc = c as ValueRangeConstraint;
                    if (vrc != null)
                    {
                        if (vrc.IsCompatible((ulong) start))
                        {
                            return start;
                        }
                        else
                        {
                            NumberValue nv = vrc.getLowerBound() as NumberValue;
                            if (nv != null)
                                val = nv.value;
                            else
                                //val = -1;
                                throw new NotImplementedException();

                        }

                    }
                    else
                        throw new NotImplementedException();
                }
                if ((ulong) start <= val)
                {
                    return (int) val;
                }
            }
            return -1;
        }

        /**
         * Returns a string representation of this object.
         *
         * @return a string representation of this object
         */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("SIZE (");
            builder.Append(values);
            builder.Append(")");

            return builder.ToString();
        }
    }
}
