//
// MibSymbol.cs
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


namespace MibbleSharp
{
    public abstract class MibSymbol
    {
        /**
     * The symbol location.
     */
        private FileLocation location;

        /**
         * The MIB containing this symbol.
         */
        private Mib mib;

        /**
         * The symbol name.
         */
        private string name;

        /**
         * The symbol comment.
         */
        private string comment;

        /**
         * Creates a new symbol with the specified name. The symbol will
         * also be added to the MIB file.
         *
         * @param location       the symbol location
         * @param mib            the symbol MIB file
         * @param name           the symbol name
         *
         * @since 2.2
         */
        public MibSymbol(FileLocation location, Mib mib, string name)
        {
            this.location = location;
            this.mib = mib;
            this.name = name;
            if (mib != null)
            {
                mib.addSymbol(this);
            }
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
        public abstract void Initialize(MibLoaderLog log);

        /**
         * Clears and prepares this MIB symbol for garbage collection.
         * This method will recursively clear any associated types or
         * values, making sure that no data structures references this
         * symbol.
         */
        public abstract void Clear();

        /**
         * Returns the file location.
         *
         * @return the file location
         */
        public FileLocation getLocation()
        {
            return location;
        }

        /**
         * Returns the symbol MIB file. This is the MIB file where the
         * symbol is defined.
         *
         * @return the symbol MIB file
         *
         * @since 2.2
         */
        public Mib getMib()
        {
            return mib;
        }

        /**
         * Returns the symbol name.
         *
         * @return the symbol name
         */
        public string getName()
        {
            return name;
        }

        /**
         * Returns the symbol comment.
         *
         * @return the symbol comment, or
         *         null if no comment was set
         *
         * @since 2.6
         */
        public string Comment {
            get { return comment; }
            set { comment = value; }
        }
    }
}
