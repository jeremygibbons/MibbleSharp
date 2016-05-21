// <copyright file="MibSymbol.cs" company="None">
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
    /// <summary>
    /// A MIB symbol. This is the base class for all symbols in a MIB file.
    /// Each symbol is typically identified by it's name, which must be
    /// unique within the MIB file.All symbols also have a data type.
    /// </summary>
    public abstract class MibSymbol
    {
        /// <summary>The symbol location.</summary>
        private FileLocation location;

        /// <summary>
        /// The MIB containing this symbol.
        /// </summary>
        private Mib mib;

        /// <summary>The symbol name.</summary>
        private string name;

        /// <summary>The symbol comment.</summary>
        private string comment;

        /// <summary>
        /// Initializes a new instance of the <see cref="MibSymbol"/> class
        /// with the specified name. The symbol will also be added to the MIB file.
        /// </summary>
        /// <param name="location">The MIB file location</param>
        /// <param name="mib">The mib name</param>
        /// <param name="name">The symbol name</param>
        public MibSymbol(FileLocation location, Mib mib, string name)
        {
            this.location = location;
            this.mib = mib;
            this.name = name;
            if (mib != null)
            {
                mib.AddSymbol(this);
            }
        }

        /// <summary>
        /// Gets the symbol name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the file location of the symbol
        /// </summary>
        public FileLocation Location
        {
            get
            {
                return this.location;
            }
        }

        /// <summary>
        /// Gets the symbol MIB file. This is the MIB file where the
        /// symbol is defined.
        /// </summary>
        public Mib Mib
        {
            get
            {
                return this.mib;
            }
        }

        /// <summary>Gets or sets the symbol comment.</summary>
        public string Comment
        {
            get { return this.comment; }
            set { this.comment = value; }
        }

        /// <summary>
        /// Initializes the MIB symbol. This will remove all levels of
        /// indirection present, such as references to types or values.No
        /// information is lost by this operation.This method may modify
        /// this object as a side-effect.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="log">THe MIB Loader Log</param>
        public abstract void Initialize(MibLoaderLog log);

        /// <summary>
        /// Clears and prepares this MIB symbol for garbage collection.
        /// This method will recursively clear any associated types or
        /// values, making sure that no data structures references this
        /// symbol.
        /// </summary>
        public abstract void Clear();
    }
}
