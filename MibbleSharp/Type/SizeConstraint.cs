// <copyright file="SizeConstraint.cs" company="None">
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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
//    USA</para>
//    Original Java code Copyright (c) 2004-2016 Per Cederberg. All
//    rights reserved.
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
// </copyright>

namespace MibbleSharp.Type
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Text;
    using MibbleSharp.Value;

    /// <summary>
    /// A MIB type size constraint.
    /// </summary>
    public class SizeConstraint : IConstraint
    {
        /// <summary>
        /// The constraint location. This value is reset to null once the
        /// constraint has been initialized.
        /// </summary>
        private FileLocation location;

        /// <summary>
        /// The constrained size values.
        /// </summary>
        private IConstraint values;

         /// <summary>
         /// Initializes a new instance of the <see cref="SizeConstraint"/> class.
         /// </summary>
         /// <param name="location">The constraint location</param>
         /// <param name="values">The constrained size values</param>
        public SizeConstraint(FileLocation location, IConstraint values)
        {
            this.location = location;
            this.values = values;
        }

        /// <summary>
        /// Gets a list of the value constraints on the size.
        /// </summary>
        public IEnumerable<IConstraint> Values
        {
            get
            {
                if (this.values is CompoundConstraint)
                {
                    return ((CompoundConstraint)this.values).ConstraintList;
                }
                else
                {
                    List<IConstraint> l = new List<IConstraint>();
                    l.Add(this.values);
                    return l;
                }
            }
        }

        /// <summary>
        /// Initializes the constraint. This will remove all levels of
        /// indirection present, such as references to types or values. No
        /// information is lost by this operation. This method may modify
        /// this object as a side-effect, and will return the basic
        /// type.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="type">The MIB type</param>
        /// <param name="log">The MIB loader log</param>
        /// <exception cref="MibException">
        /// If an error is encountered during initialization
        /// </exception>
        public void Initialize(MibType type, MibLoaderLog log)
        {
            string message;

            this.values.Initialize(new IntegerType(), log);
            if (this.location != null && !this.IsCompatible(type))
            {
                message = "Size constraint not compatible with this type";
                log.AddWarning(this.location, message);
            }

            this.location = null;
        }

         /// <summary>
         /// Checks if the specified type is compatible with this
         /// constraint.
         /// </summary>
         /// <param name="type">The type to check</param>
         /// <returns>True if the type is compatible, false if not</returns>
        public bool IsCompatible(MibType type)
        {
            return type is SequenceOfType
                    || type is StringType;
        }

         /// <summary>
         /// Checks if the specified value is compatible with this
         /// constraint.Only octet string values can be compatible with a
         /// size constraint, and only if the string length is compatible
         /// with the value range in the size constraint.
         /// </summary>
         /// <param name="value">The value to check</param>
         /// <returns>True if the value is compatible, false if not</returns>
        public bool IsCompatible(MibValue value)
        {
            int size;

            if (value is StringValue)
            {
                size = value.ToString().Length;
                return this.values.IsCompatible(new NumberValue((BigInteger)size));
            }

            return false;
        }

        /// <summary>
        /// Returns the next compatible size constraint value from a start
        /// value.The values will be enumerated from lower values to
        /// higher.
        /// </summary>
        /// <param name="start">The initial start value</param>
        /// <returns>The next compatible value, or -1 if none exists</returns>
        public int NextValue(int start)
        {
            IEnumerable<IConstraint> list = this.Values;
            BigInteger val;

            // TODO: the constraint list should be sorted
            foreach (var c in list)
            {
                ValueConstraint vc = c as ValueConstraint;
                if (vc != null)
                {
                    NumberValue nv = vc.Value as NumberValue;
                    if (nv != null)
                    {
                        val = nv.value;
                    }
                    else
                    {
                        val = 0; // TODO: need to handle this diferently
                    }
                }
                else
                {
                    ValueRangeConstraint vrc = c as ValueRangeConstraint;
                    if (vrc != null)
                    {
                        if (vrc.IsCompatible((BigInteger)start))
                        {
                            return start;
                        }
                        else
                        {
                            NumberValue nv = vrc.LowerBound as NumberValue;
                            if (nv != null)
                            {
                                val = nv.value;
                            }
                            else
                            {
                                // TODO: fix this
                                throw new NotImplementedException();
                            }                              
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                if ((BigInteger)start <= val)
                {
                    return (int)val;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("SIZE (");
            builder.Append(this.values);
            builder.Append(")");

            return builder.ToString();
        }
    }
}
