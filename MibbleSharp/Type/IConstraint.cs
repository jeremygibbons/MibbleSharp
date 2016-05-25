// <copyright file="IConstraint.cs" company="None">
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
    /// <summary>
    /// A MIB type constraint.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// Initializes the constraint. This will remove all levels of
        /// indirection present, such as references to types or values.No
        /// constraint information is lost by this operation.This method
        /// may modify this object as a side-effect, and will be called by
        /// the MIB loader.
        /// </summary>
        /// <param name="type">The type to constrain</param>
        /// <param name="log">The MIB loader log</param>
        /// <exception cref="MibException">
        /// If an error occurs during initialization
        /// </exception>
        void Initialize(MibType type, MibLoaderLog log);

        /// <summary>
        /// Checks if the specified type is compatible with this
        /// constraint.
        /// </summary>
        /// <param name="type">The type to be checked</param>
        /// <returns>True if compatible, false if not</returns>
        bool IsCompatible(MibType type);

        /// <summary>
        /// Checks if the specified value is compatible with this
        /// constraint.
        /// </summary>
        /// <param name="value">The value to be checked</param>
        /// <returns>True if compatible, false if not</returns>
        bool IsCompatible(MibValue value);
    }
}