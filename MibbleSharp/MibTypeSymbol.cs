// <copyright file="MibTypeSymbol.cs" company="None">
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
    /// A MIB type symbol. This class holds information relevant to a MIB
    /// type assignment, i.e. a defined type name.
    /// </summary>
    public class MibTypeSymbol : MibSymbol
    {
        /// <summary>The symbol type.</summary>
        private MibType type;

        /// <summary>Initializes a new instance of the <see cref="MibTypeSymbol"/> class.</summary>
        /// <param name="location">The symbol location</param>
        /// <param name="mib">The symbol MIB file</param>
        /// <param name="name">The symbol name</param>
        /// <param name="type">The symbol type</param>
        public MibTypeSymbol(
            FileLocation location, 
            Mib mib,
            string name,
            MibType type)
            : base(location, mib, name)
        {
            this.type = type;
        }

        /// <summary>Gets the symbol type.</summary>
        public MibType Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>Initializes the MIB symbol. This will remove all levels of
        /// indirection present, such as references to types or values. No
        /// information is lost by this operation. This method may modify
        /// this object as a side-effect.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="log">The MIB Loader Log</param>
        public override void Initialize(MibLoaderLog log)
        {
            if (this.type != null)
            {
                try
                {
                    this.type = this.type.Initialize(this, log);
                }
                catch (MibException e)
                {
                    log.AddError(e.Location, e.Message);
                    this.type = null;
                }
            }
        }

        /// <summary>
        /// Clears and prepares this MIB symbol for garbage collection.
        /// This method will recursively clear any associated types or
        /// values, making sure that no data structures references this
        /// symbol.
        /// </summary>
        public override void Clear()
        {
            this.type = null;
        }

        /// <summary>Returns a string representation of this object.</summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("TYPE ");
            buffer.Append(this.Name);
            buffer.Append(" ::= ");
            buffer.Append(this.type);
            return buffer.ToString();
        }
    }
}
