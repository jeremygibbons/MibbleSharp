using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MibbleSharp.Value;

namespace MibbleSharp.Type
{
    /**
     * An integer MIB type.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.7
     * @since    2.0
     */
    public class IntegerType : MibType, MibContext
    {

        /**
         * The additional type constraint.
         */
        private Constraint constraint = null;

        /**
         * The additional defined symbols.
         */
        private IDictionary<string, MibValueSymbol> symbols = new Dictionary<string, MibValueSymbol>();

        /**
         * Creates a new integer MIB type.
         */
        public IntegerType() : this(true, null, null)
        {

        }

        /**
         * Creates a new integer MIB type.
         *
         * @param constraint     the additional type constraint
         */
        public IntegerType(Constraint constraint) : this(true, constraint, null)
        {

        }

        /**
         * Creates a new integer MIB type.
         *
         * @param values         the additional defined symbols
         */
        public IntegerType(IList<MibValueSymbol> values) : this(true, null, null)
        {
            CreateValueConstraints(values);
        }

        /**
         * Creates a new integer MIB type.
         *
         * @param primitive      the primitive type flag
         * @param constraint     the type constraint, or null
         * @param symbols        the defined symbols, or null
         */
        private IntegerType(bool primitive,
                            Constraint constraint,
                            IDictionary<string, MibValueSymbol> symbols)
                : base("INTEGER", primitive)
        {

            if (constraint != null)
            {
                this.constraint = constraint;
            }
            if (symbols != null)
            {
                this.symbols = symbols;
            }
            setTag(true, MibTypeTag.INTEGER);
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
            if (constraint != null)
            {
                constraint.Initialize(this, log);
            }
            foreach(MibValueSymbol sym in symbols.Values)
            {
                sym.Initialize(log);
                if (!IsCompatibleType(sym.getValue()))
                {
                    string message = "value is not compatible with type";
                    throw new MibException(sym.getLocation(), message);
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
            IntegerType type = new IntegerType(false, constraint, symbols);

            type.setTag(true, getTag());
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
        public override MibType CreateReference(Constraint constraint)
        {
            IntegerType type = new IntegerType(false, constraint, null);

            type.setTag(true, getTag());
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
            IntegerType type;

            type = new IntegerType(false, null, null);
            type.CreateValueConstraints(values);
            type.setTag(true, getTag());
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
            foreach (MibValueSymbol sym in values)
            {
                symbols[sym.getName()] = sym;
                // TODO: check value constraint compability
                c = new ValueConstraint(null, sym.getValue());
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
        public bool HasConstraint
        {
            get
            {
                return constraint != null;
            }
        }

        /**
         * Checks if this type has any defined value symbols.
         *
         * @return true if this type has any defined value symbols, or
         *         false otherwise
         */
        public bool HasSymbols
        {
            get
            {
                return symbols.Count > 0;
            }
            
        }

        /**
         * Checks if the specified value is compatible with this type. A
         * value is compatible if it is an integer number value that is
         * compatible with the constraints.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        public override bool IsCompatible(MibValue value)
        {
            return IsCompatibleType(value)
                && (constraint == null || constraint.IsCompatible(value));
        }

        /**
         * Checks if the specified value is compatible with this type. A
         * value is compatible if it is an integer number value.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        private bool IsCompatibleType(MibValue value)
        {
            return value is NumberValue;
        }

        /**
         * Returns the optional type constraint. The type constraints for
         * an integer will typically be value, value range or compound
         * constraints.
         *
         * @return the type constraint, or
         *         null if no constraint has been set
         *
         * @since 2.2
         */
        public Constraint getConstraint()
        {
            return constraint;
        }

        /**
         * Returns a named integer value. The value will be returned as a
         * value symbol, containing a numeric MIB value. The symbol
         * returned is not a normal MIB symbol, i.e. only the name and
         * value components are valid.<p>
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
         * Returns all named integer values. An integer may also allow
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
        public MibSymbol findSymbol(string name, bool expanded)
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
                String.Join(", ", symbols.Values.Select(sym => sym.getName() + "(" + sym.getValue() + ")"));
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
