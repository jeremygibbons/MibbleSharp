//
// MibTypeSymbol.cs
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

using System.Text;

namespace MibbleSharp
{
    /**
     * A MIB type symbol. This class holds information relevant to a MIB
     * type assignment, i.e. a defined type name.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.7
     * @since    2.0
     */
    public class MibTypeSymbol : MibSymbol
    {

        /**
         * The symbol type.
         */
        private MibType type;

        /**
         * Creates a new type symbol
         *
         * @param location       the symbol location
         * @param mib            the symbol MIB file
         * @param name           the symbol name
         * @param type           the symbol type
         *
         * @since 2.2
         */
        public MibTypeSymbol(FileLocation location,
                      Mib mib,
                      string name,
                      MibType type) : base(location, mib, name)
        {
            this.type = type;
        }

        /**
         * Initializes the MIB symbol. This will remove all levels of
         * indirection present, such as references to types or values. No
         * information is lost by this operation. This method may modify
         * this object as a side-effect.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @param log            the MIB loader log
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        public override void Initialize(MibLoaderLog log)
        {
            if (type != null)
            {
                try
                {
                    type = type.Initialize(this, log);
                }
                catch (MibException e)
                {
                    log.addError(e.getLocation(), e.Message);
                    type = null;
                }
            }
        }

        /**
         * Clears and prepares this MIB symbol for garbage collection.
         * This method will recursively clear any associated types or
         * values, making sure that no data structures references this
         * symbol.
         */
        public override void Clear()
        {
            type = null;
        }

        /**
         * Returns the symbol type.
         *
         * @return the symbol type
         */
        public MibType getType()
        {
            return type;
        }

        /**
         * Returns a string representation of this object.
         *
         * @return a string representation of this object
         */
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("TYPE ");
            buffer.Append(getName());
            buffer.Append(" ::= ");
            buffer.Append(getType());
            return buffer.ToString();
        }
    }

}
