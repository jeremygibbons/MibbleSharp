//
// Constraint.cs
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

namespace MibbleSharp.Type
{
   
    /**
     * A MIB type constraint.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.6
     * @since    2.0
     */
    public interface Constraint
    {

        /**
         * Initializes the constraint. This will remove all levels of
         * indirection present, such as references to types or values. No
         * constraint information is lost by this operation. This method
         * may modify this object as a side-effect, and will be called by
         * the MIB loader.
         *
         * @param type           the type to constrain
         * @param log            the MIB loader log
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        void Initialize(MibType type, MibLoaderLog log);

        /**
         * Checks if the specified type is compatible with this
         * constraint.
         *
         * @param type            the type to check
         *
         * @return true if the type is compatible, or
         *         false otherwise
         */
        bool IsCompatible(MibType type);

        /**
         * Checks if the specified value is compatible with this
         * constraint.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        bool IsCompatible(MibValue value);
    }

}
