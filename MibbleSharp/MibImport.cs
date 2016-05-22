// <copyright file="MibImport.cs" company="None">
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
    using System.Collections.Generic;

    /// <summary>
    /// A MIB import list. This class contains a reference to another MIB
    /// and a number of symbols in it.
    /// </summary>
    public class MibImport : IMibContext
    {
        /// <summary>The MIB loader being used.</summary>
        private MibLoader loader;

        /// <summary>The referenced MIB.</summary>
        private Mib mib = null;

        /// <summary>The import location.</summary>
        private FileLocation location;

        /// <summary>The imported MIB name.</summary>
        private string name;

        /// <summary>The imported MIB symbol names.</summary>
        private IList<MibSymbol> symbols;

        /// <summary>
        /// Initializes a new instance of the <see cref="MibImport"/> class.
        /// </summary>
        /// <param name="loader">The MIB Loader to be used</param>
        /// <param name="location">The import location</param>
        /// <param name="name">The imported MIB's name</param>
        /// <param name="symbols">The imported MIB symbol names</param>
        public MibImport(
            MibLoader loader,
            FileLocation location,
            string name,
            IList<MibSymbol> symbols)
        {
            this.loader = loader;
            this.location = location;
            this.name = name;
            this.symbols = symbols;
        }

        /// <summary>
        /// Gets the imported MIB name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the imported MIB.
        /// </summary>
        public Mib Mib
        {
            get
            {
                return this.mib;
            }
        }

        /// <summary>
        /// Gets all symbol names in this MIB import declaration.
        /// </summary>
        public IList<MibSymbol> SymbolNames
        {
            get
            {
                return this.symbols;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the MibImport has any symbols
        /// </summary>
        /// <returns>True if there is a symbol list, false if not</returns>
        public bool HasSymbols
        {
            get
            {
                return this.symbols != null;
            }
        }

        /// <summary>
        /// Initializes the MIB import. This will resolve all referenced
        /// symbols.This method will be called by the MIB loader.
        /// </summary>
        /// <param name="log">The MIB Loader Log</param>
        /// <exception cref="MibException">if an error was encountered during the
        /// initialization
        /// </exception>
        public void Initialize(MibLoaderLog log)
        {
            string message;

            this.mib = this.loader.getMib(this.name);
            if (this.mib == null)
            {
                message = "couldn't find referenced MIB '" + this.name + "'";
                throw new MibException(this.location, message);
            }

            if (this.symbols != null)
            {
                foreach (var s in this.symbols)
                {
                    if (this.mib.GetSymbol(s.ToString()) == null)
                    {
                        message = "couldn't find imported symbol '" +
                                  s + "' in MIB '" + this.name + "'";
                        throw new MibException(this.location, message);
                    }
                }
            }
        }
        
        /// <summary>
        /// Searches for a named MIB symbol. This method may search outside
        /// the normal (or strict) scope, thereby allowing a form of
        /// relaxed search. Note that the results from the normal and
        /// expanded search may not be identical, due to the context
        /// chaining and the same symbol name appearing in various
        /// contexts.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should only be called by the MIB loader.
        /// The expanded parameter currently has no effect
        /// </remarks>
        /// <param name="name">The symbol name</param>
        /// <param name="expanded">The expanded scope flag</param>
        /// <returns>The symbol if found, null if not</returns>
        public MibSymbol FindSymbol(string name, bool expanded)
        {
            if (this.mib == null)
            {
                return null;
            }

            return this.mib.GetSymbol(name);
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return this.name;
        }
    }
}
