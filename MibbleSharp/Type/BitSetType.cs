//
// BitSetType.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MibbleSharp.Value;

namespace MibbleSharp.Type
{

    /**
     * A bit set MIB type. This is the equivalent of a bit string, i.e.
     * a set of bit values
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.7
     * @since    2.0
     */
    public class BitSetType : MibType, IMibContext
    {

        /**
         * The additional type constraint.
         */
        private IConstraint constraint = null;

        /**
         * The additional defined symbols.
         */
        private Dictionary<string, MibValueSymbol> symbols = new Dictionary<string, MibValueSymbol>();

        /**
         * Creates a new bit set MIB type.
         */
        public BitSetType() : this(true, null, null)
        {

        }

        /**
         * Creates a new bit set MIB type.
         *
         * @param constraint     the additional type constraint
         */
        public BitSetType(IConstraint constraint) : this(true, constraint, null)
        {

        }

        /**
         * Creates a new bit set MIB type.
         *
         * @param values         the additional defined symbols
         */
        public BitSetType(IList<MibValueSymbol> values) : this(true, null, null)
        {

            CreateValueConstraints(values);
        }

        /**
         * Creates a new bit set MIB type.
         *
         * @param primitive      the primitive type flag
         * @param constraint     the type constraint, or null
         * @param symbols        the defined symbols, or null
         */
        private BitSetType(bool primitive,
                           IConstraint constraint,
                           Dictionary<string, MibValueSymbol> symbols)
                : base("BITS", primitive)
        {

            if (constraint != null)
            {
                this.constraint = constraint;
            }
            if (symbols != null)
            {
                this.symbols = symbols;
            }
        }

        /**
         * Initializes the MIB type. This will remove all levels of
         * indirection present, such as references to types or values. No
         * information is lost by this operation. This method may modify
         * this object as a side-effect, and will return the basic
         * type.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @param symbol         the MIB symbol containing this type
         * @param log            the MIB loader log
         *
         * @return the basic MIB type
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         *
         * @since 2.2
         */
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            string message;

            SetTag(true, MibTypeTag.BitString);

            if (constraint != null)
            {
                constraint.Initialize(this, log);
            }
            foreach(MibValueSymbol sym in symbols.Values)
            {
                sym.Initialize(log);
                if (!(sym.Value is NumberValue))
                {
                    message = "value is not compatible with type";
                    throw new MibException(sym.Location, message);
                }
            }
          return this;
        }

        /**
         * Creates a type reference to this type. The type reference is
         * normally an identical type, but with the primitive flag set to
         * false. Only certain types support being referenced, and the
         * default implementation of this method throws an exception.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @return the MIB type reference
         *
         * @since 2.2
         */
        public override MibType CreateReference()
        {
            BitSetType type = new BitSetType(false, constraint, symbols);

            type.SetTag(true, Tag);
            return type;
        }

        /**
         * Creates a constrained type reference to this type. The type
         * reference is normally an identical type, but with the
         * primitive flag set to false. Only certain types support being
         * referenced, and the default implementation of this method
         * throws an exception.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @param constraint     the type constraint
         *
         * @return the MIB type reference
         *
         * @since 2.2
         */
        public override MibType CreateReference(IConstraint constraint)
        {
            BitSetType type = new BitSetType(false, constraint, null);

            type.SetTag(true, Tag);
            return type;
        }

        /**
         * Creates a constrained type reference to this type. The type
         * reference is normally an identical type, but with the
         * primitive flag set to false. Only certain types support being
         * referenced, and the default implementation of this method
         * throws an exception.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @param values         the type value symbols
         *
         * @return the MIB type reference
         *
         * @since 2.2
         */
        public override MibType CreateReference(IList<MibValueSymbol> values)
        {
            BitSetType type;

            type = new BitSetType(false, null, null);
            type.CreateValueConstraints(values);
            type.SetTag(false, Tag);
            return type;
        }

        /**
         * Creates the constraints and symbol map from a list of value
         * symbols.
         *
         * @param values         the list of value symbols
         */
        private void CreateValueConstraints(IList<MibValueSymbol> values)
        {
            ValueConstraint c;

            foreach(MibValueSymbol sym in values)
            {
                symbols[sym.Name] = sym;
                // TODO: check value constraint compability
                c = new ValueConstraint(null, sym.Value);
                if (constraint == null)
                {
                    constraint = c;
                }
                else
                {
                    constraint = new CompoundConstraint(constraint, c);
                }
            }

        }

        /**
         * Checks if this type has any constraint.
         *
         * @return true if this type has any constraint, or
         *         false otherwise
         */
        public bool HasConstraint()
        {
            return constraint != null;
        }

        /**
         * Checks if this type has any defined value symbols.
         *
         * @return true if this type has any defined value symbols, or
         *         false otherwise
         */
        public bool hasSymbols()
        {
            return symbols.Count > 0;
        }

        /**
         * Checks if the specified value is compatible with this type.  A
         * value is compatible if it is a bit set value with all
         * components compatible with the constraints.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        public override bool IsCompatible(MibValue value)
        {
            return value is BitSetValue
                    && IsCompatible((BitSetValue)value);
        }

        /**
         * Checks if the specified bit set value is compatible with this
         * type.  A value is compatible if all the bits are compatible
         * with the constraints.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        public bool IsCompatible(BitSetValue value)
        {
            if (constraint == null)
            {
                return true;
            }

            IList<NumberValue> bits = value.getBits();
            foreach(NumberValue v in bits)
            {
                if (!constraint.IsCompatible(v))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Returns the optional type constraint. The type constraints for
         * a bit set will typically be value constraints or compound
         * value constraints.
         *
         * @return the type constraint, or
         *         null if no constraint has been set
         *
         * @since 2.2
         */
        public IConstraint GetConstraint()
        {
            return constraint;
        }

        /**
         * Returns a named bit value. The value will be returned as a
         * value symbol, containing a numeric MIB value. The symbol
         * returned is not a normal MIB symbol, i.e. it is not present in
         * any MIB file and only the name and value components are
         * valid.<p>
         *
         * <strong>Note:</strong> As of version 2.4 the method signature
         * was changed to return a MibValueSymbol instead of a MibSymbol.
         *
         * @param name           the symbol name
         *
         * @return the MIB value symbol, or
         *         null if not found
         */
        public MibValueSymbol GetSymbol(string name)
        {
            return symbols[name];
        }

        /**
         * Returns all named bit values. Note that a bit string may allow
         * unnamed values, depending on the constraints. Use the
         * constraint object or the isCompatible() method to check if a
         * value is compatible with this type. Also note that the value
         * symbols returned by this method are not normal MIB symbols,
         * i.e. only the name and value components are valid.
         *
         * @return an array of all named values (as MIB value symbols)
         *
         * @since 2.2
         */
        public IEnumerable<MibValueSymbol> getAllSymbols()
        {
            return symbols.Values;
        }

        /**
         * Searches for a named MIB symbol. This method may search outside
         * the normal (or strict) scope, thereby allowing a form of
         * relaxed search. Note that the results from the normal and
         * expanded search may not be identical, due to the context
         * chaining and the same symbol name appearing in various
         * contexts.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @param name           the symbol name
         * @param expanded       the expanded scope flag
         *
         * @return the MIB symbol, or null if not found
         *
         * @since 2.4
         */
        public MibSymbol FindSymbol(string name, bool expanded)
        {
            return GetSymbol(name);
        }

        /**
         * Returns a string representation of this type.
         *
         * @return a string representation of this type
         */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(base.ToString());

            if (symbols.Count > 0)
            {
                builder.Append(" { ");
                String.Join(", ", symbols.Values.Select(sym => sym.Name + "(" + sym.Value+ ")"));
                builder.Append(" }");
            }
            else if (constraint != null)
            {
                builder.Append(" (");
                builder.Append(constraint.ToString());
                builder.Append(")");
            }

            return builder.ToString();
        }
    }
}
