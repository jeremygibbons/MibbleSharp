//
// MibImport.cs
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

using System.Collections.Generic;

namespace MibbleSharp
{
    /// <summary>
    /// A MIB import list. This class contains a referenc to another MIB
    /// and a number of symbols in it.
    /// </summary>
    public class MibImport : IMibContext
    {

        ///
        /// The MIB loader being used.
        ///
        private MibLoader loader;

        ///
        /// The referenced MIB.
        ///
        private Mib mib = null;

        ///
        /// The import location.
        ///
        private FileLocation location;

        ///
        /// The imported MIB name.
        ///
        private string name;

        ///
        /// The imported MIB symbol names.
        ///
        private IList<MibSymbol> symbols;

        /// 
        /// <summary>
        /// Creates a new MIB import.
        /// </summary>
        /// <param name="loader">The MIB Loader to be used</param>
        /// <param name="location">The import location</param>
        /// <param name="name">The imported MIB's name</param>
        /// <param name="symbols">The imported MIB symbol names</param>
        /// 
        public MibImport(MibLoader loader,
              FileLocation location,
              string name,
              IList<MibSymbol> symbols)
        {

            this.loader = loader;
            this.location = location;
            this.name = name;
            this.symbols = symbols;
        }

        /// 
        /// <summary>
        /// Initializes the MIB import. This will resolve all referenced
        /// symbols.This method will be called by the MIB loader.
        /// </summary>
        /// <param name="log">The MIB Loader Log</param>
        /// <exception cref="MibException">if an error was encountered during the
        /// initialization
        /// </exception>
        /// 
        public void Initialize(MibLoaderLog log)
        {
            string message;

            mib = loader.getMib(name);
            if (mib == null)
            {
                message = "couldn't find referenced MIB '" + name + "'";
                throw new MibException(location, message);
            }
            if (symbols != null)
            {
                foreach(var s in symbols)
                {
                    if(mib.GetSymbol(s.ToString()) == null)
                    {
                        message = "couldn't find imported symbol '" +
                                  s + "' in MIB '" + name + "'";
                        throw new MibException(location, message);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if this import has a symbol list.
        /// </summary>
        /// <returns>True if there is a symbol list, false if not</returns>
        public bool HasSymbols()
        {
            return symbols != null;
        }

        /// <summary>
        /// The imported MIB name.
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// The imported MIB.
        /// </summary>
        public Mib Mib
        {
            get
            {
                return mib;
            }
        }

        /// <summary>
        /// Returns all symbol names in this MIB import declaration.
        /// </summary>
        /// <returns></returns>
        public IList<MibSymbol> SymbolNames
        {
            get
            {
                return symbols;
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
        /// <returns></returns>
        public MibSymbol FindSymbol(string name, bool expanded)
        {
            if (mib == null)
            {
                return null;
            }

            return mib.GetSymbol(name);
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }
    }

}
