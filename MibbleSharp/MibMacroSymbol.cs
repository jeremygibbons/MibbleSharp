// <copyright file="MibMacroSymbol.cs" company="None">
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

namespace MibbleSharp
{
    using System.Text;

    /// <summary>
    /// A MIB macro symbol. This class holds information relevant to a MIB
    /// macro definition, i.e.a defined macro name.
    /// </summary>
    public class MibMacroSymbol : MibSymbol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MibMacroSymbol"/> class.
        /// </summary>
        /// <param name="location">The symbol location</param>
        /// <param name="mib">The symbol MIB name</param>
        /// <param name="name">The symbol name</param>
        public MibMacroSymbol(FileLocation location, Mib mib, string name) :
            base(location, mib, name)
        {
        }

        /// <summary>
        /// Initializes the MIB symbol. This will remove all levels of
        /// indirection present, such as references to types or values.No
        /// information is lost by this operation.This method may modify
        /// this object as a side-effect.
        /// </summary>
        /// <param name="log">The MibLoaderLog</param>
        public override void Initialize(MibLoaderLog log)
        {
            // Nothing to be initialized
        }

        /// <summary>
        /// Clears and prepares this MIB symbol for garbage collection.
        /// This method will recursively clear any associated types or
        /// values, making sure that no data structures references this
        /// symbol.
        /// </summary>
        public override void Clear()
        {
            // Nothing to clear
        }

        /// <summary>
        /// Get a string representation of the MibMacroSymbol
        /// </summary>
        /// <returns>A string representation of the MibMacroSymbol</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("MACRO ");
            builder.Append(this.Name);
            return builder.ToString();
        }
    }
}
