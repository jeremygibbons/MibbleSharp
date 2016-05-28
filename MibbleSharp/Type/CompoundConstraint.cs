// <copyright file="CompoundConstraint.cs" company="None">
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

namespace MibbleSharp.Type
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A compound MIB type constraint. This class holds two constraints,
    /// either one that must be compatible for this constraint to return
    /// true. Effectively this class represents an OR composition of the
    /// two constraints.
    /// </summary>
    public class CompoundConstraint : IConstraint
    {
        /// <summary>
        /// The first constraint.
        /// </summary>
        private IConstraint first;

        /// <summary>
        /// The second constraint.
        /// </summary>
        private IConstraint second;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundConstraint"/> class.
        /// </summary>
        /// <param name="first">The first constraint</param>
        /// <param name="second">The second constraint</param>
        public CompoundConstraint(IConstraint first, IConstraint second)
        {
            this.first = first;
            this.second = second;
        }

        /// <summary>
        /// Gets a list of the constraints in this compound. All
        /// compound constraints will be flattened out and their contents
        /// will be added to the list.
        /// </summary>
        public IList<IConstraint> ConstraintList
        {
            get
            {
                List<IConstraint> list = new List<IConstraint>();

                if (this.first is CompoundConstraint)
                {
                    list.AddRange(((CompoundConstraint)this.first).ConstraintList);
                }
                else
                {
                    list.Add(this.first);
                }

                if (this.second is CompoundConstraint)
                {
                    list.AddRange(((CompoundConstraint)this.second).ConstraintList);
                }
                else
                {
                    list.Add(this.second);
                }

                return list;
            }
        }

        /// <summary>
        /// Initializes the MIB type. This will remove all levels of
        /// indirection present, such as references to types or values. No
        /// information is lost by this operation. This method may modify
        /// this object as a side-effect.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        /// <param name="type">The MIB type</param>
        /// <param name="log">The MIB loader log</param>
        /// <exception cref="MibException">If an error occurred during initialization</exception>
        public void Initialize(MibType type, MibLoaderLog log)
        {
            this.first.Initialize(type, log);
            this.second.Initialize(type, log);
        }

        /// <summary>
        /// Checks if the specified value is compatible with this type. A
        /// values is considered compatible with this type, it is compatible with 
        /// both constraints
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public bool IsCompatible(MibType type)
        {
            return this.first.IsCompatible(type) 
                && this.second.IsCompatible(type);
        }

        /// <summary>
        /// Checks if the specified value is compatible with this constraint set.
        /// A value is considered compatible if it is compatible with 
        /// either of the two constraints
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public bool IsCompatible(MibValue value)
        {
            return this.first.IsCompatible(value) || this.second.IsCompatible(value);
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>
        /// A string representation of this object
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(this.first.ToString());
            builder.Append(" | ");
            builder.Append(this.second.ToString());

            return builder.ToString();
        }
    }
}