//
// ValueReference.cs
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

using System;

namespace MibbleSharp.Value
{
    /**
     * A reference to a value symbol.<p>
     *
     * <strong>NOTE:</strong> This class is used internally during the
     * MIB parsing only. After loading a MIB file successfully, all value
     * references will have been resolved to other MIB values. Do
     * <strong>NOT</strong> use or reference this class.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class ValueReference : MibValue
    {

        /**
         * The reference location.
         */
        private FileLocation location;

        /**
         * The reference context.
         */
        private IMibContext context;

        /**
         * The referenced name.
         */
        private string name;

        /**
         * Creates a new value reference.
         *
         * @param location       the reference location
         * @param context        the reference context
         * @param name           the reference name
         */
        public ValueReference(FileLocation location,
                              IMibContext context,
                              string name)
                : base("ReferenceToValue(" + name + ")")
        {
            this.location = location;
            this.context = context;
            this.name = name;
        }

        /**
         * Initializes the MIB value. This will remove all levels of
         * indirection present, such as references to other values. No
         * value information is lost by this operation. This method may
         * modify this object as a side-effect, and will return the basic
         * value.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @param log            the MIB loader log
         * @param type           the value type
         *
         * @return the basic MIB value
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        public override MibValue Initialize(MibLoaderLog log, MibType type)
        {
            MibSymbol sym;
            MibValue value;
            string message;

            sym = getSymbol(log);
            if (sym is MibValueSymbol)
            {
                value = ((MibValueSymbol)sym).getValue();
                if (value != null)
                {
                    value = value.Initialize(log, type);
                }
                if (value == null)
                {
                    return null;
                }
                try
                {
                    value = value.CreateReference();
                }
                catch (NotSupportedException e)
                {
                    throw new MibException(location, e.Message);
                }
                if (!(value is ObjectIdentifierValue))
                {
                    value.ReferenceSymbol = (MibValueSymbol)sym;
                }
                return value;
            }
            else if (sym == null)
            {
                message = "undefined symbol '" + name + "'";
                throw new MibException(location, message);
            }
            else
            {
                message = "referenced symbol '" + name + "' is not a value";
                throw new MibException(location, message);
            }
        }

        /**
         * Returns the reference location.
         *
         * @return the reference location
         */
        public FileLocation getLocation()
        {
            return location;
        }

        /**
         * Returns the referenced symbol.
         *
         * @return the referenced symbol
         */
        public MibSymbol getSymbol()
        {
            return getSymbol(null);
        }

        /**
         * Returns the referenced symbol.
         *
         * @param log            the optional loader log
         *
         * @return the referenced symbol
         */
        private MibSymbol getSymbol(MibLoaderLog log)
        {
            MibSymbol sym;
            string message;

            sym = context.FindSymbol(name, false);
            if (sym == null)
            {
                sym = context.FindSymbol(name, true);
                if (sym != null && log != null)
                {
                    message = "missing import for '" + name + "', using " +
                              "definition from " + sym.Mib.Name;
                    log.AddWarning(location, message);
                }
            }
            return sym;
        }

        public override int CompareTo(MibValue other)
        {
            throw new NotImplementedException();
        }

        /**
         * Checks if this object equals another object. This method will
         * compare the string representations for equality.
         *
         * @param obj            the object to compare with
         *
         * @return true if the objects are equal, or
         *         false otherwise
         *
         * @since 2.6
         */
        public override bool Equals(Object obj)
        {
            return ToString().Equals(obj.ToString());
        }

        /**
         * Returns a hash code for this object.
         *
         * @return a hash code for this object
         *
         * @since 2.6
         */
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /**
         * Returns a Java object representation of this value. This
         * method will always return null.
         *
         * @return a Java object representation of this value
         */
        public Object ToObject()
        {
            return null;
        }

        /**
         * Returns a string representation of this value.
         *
         * @return a string representation of this value
         */
        public override string ToString()
        {
            return "ReferenceToValue(" + name + ")";
        }
    }
}
