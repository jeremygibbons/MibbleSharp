// <copyright file="BitSetType.cs" company="None">
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

namespace MibbleSharp.Type
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MibbleSharp.Value;

    /// <summary>
    /// A bit set MIB type. This is the equivalent of a bit string, i.e.
    /// a set of bit values
    /// </summary>
    public class BitSetType : MibType, IMibContext
    {
        /// <summary>
        /// The additional type constraint.
        /// </summary>
        private IConstraint constraint = null;

        /// <summary>
        /// The additional defined symbols.
        /// </summary>
        private Dictionary<string, MibValueSymbol> symbols = new Dictionary<string, MibValueSymbol>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BitSetType"/> class
        /// </summary>
        public BitSetType() : this(true, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitSetType"/> class
        /// </summary>
        /// <param name="constraint">The additional type constraint</param>
        public BitSetType(IConstraint constraint) : this(true, constraint, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitSetType"/> class
        /// </summary>
        /// <param name="values">The additional symbols</param>
        public BitSetType(IList<MibValueSymbol> values) : this(true, null, null)
        {
            this.CreateValueConstraints(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitSetType"/> class
        /// </summary>
        /// <param name="primitive">The primitive type flag</param>
        /// <param name="constraint">The type constraint, or null</param>
        /// <param name="symbols">The defined symbols, or null</param>
        private BitSetType(
            bool primitive,
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

        /// <summary>
        /// Gets a value indicating whether this type has any constraint.
        /// </summary>
        public bool HasConstraint
        {
            get
            {
                return this.constraint != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this type has any defined value symbols.
        /// </summary>
        public bool HasSymbols
        {
            get
            {
                return this.symbols.Count > 0;
            }
        }

        /// <summary>
        /// Gets the optional type constraint. The type constraints for
        /// a bit set will typically be value constraints or compound
        /// value constraints.
        /// </summary>
        public IConstraint Constraint
        {
            get
            {
                return this.constraint;
            }
        }

        /// <summary>
        /// Gets all named bit values. Note that a bit string may allow
        /// unnamed values, depending on the constraints. Use the
        /// constraint object or the isCompatible() method to check if a
        /// value is compatible with this type. Also note that the value
        /// symbols returned by this method are not normal MIB symbols,
        /// i.e. only the name and value components are valid.
        /// </summary>
        public IEnumerable<MibValueSymbol> AllSymbols
        {
            get
            {
                return this.symbols.Values;
            }
        }

        /// <summary>
        /// Returns a named bit value. The value will be returned as a
        /// value symbol, containing a numeric MIB value. The symbol
        /// returned is not a normal MIB symbol, i.e. it is not present in
        /// any MIB file and only the name and value components are
        /// valid.
        /// NOTE: 
        /// </summary>
        /// <remarks>
        /// As of version 2.4 the method signature was changed to return a 
        /// MibValueSymbol instead of a MibSymbol.
        /// </remarks>
        /// <param name="name">The symbol name</param>
        /// <returns>The MIB value symbol, or null if none was found</returns>
        public MibValueSymbol GetSymbol(string name)
        {
            return this.symbols[name];
        }

        /// <summary>
        /// Initializes the MIB type. This will remove all levels of
        /// indirection present, such as references to types or values. No
        /// information is lost by this operation. This method may modify
        /// this object as a side-effect, and will return the basic
        /// type.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="symbol">the MIB symbol containing this type</param>
        /// <param name="log">The MIB loader log</param>
        /// <returns>The basic MIB type</returns>
        /// <exception cref="MibException">
        /// If an error was encountered during initialization
        /// </exception>
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            string message;

            this.SetTag(true, MibTypeTag.BitString);

            if (this.constraint != null)
            {
                this.constraint.Initialize(this, log);
            }

            foreach (MibValueSymbol sym in this.symbols.Values)
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

        /// <summary>
        /// Creates a type reference to this type. The type reference is
        /// normally an identical type, but with the primitive flag set to
        /// false. Only certain types support being referenced, and the
        /// default implementation of this method throws an exception.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <returns>The MIB type reference</returns>
        public override MibType CreateReference()
        {
            BitSetType type = new BitSetType(false, this.constraint, this.symbols);
            type.SetTag(true, this.Tag);
            return type;
        }

        /// <summary>
        /// Creates a constrained type reference to this type. The type
        /// reference is normally an identical type, but with the
        /// primitive flag set to false. Only certain types support being
        /// referenced, and the default implementation of this method
        /// throws an exception.     
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="constraint">The type constraint</param>
        /// <returns>The MIB type reference</returns>
        public override MibType CreateReference(IConstraint constraint)
        {
            BitSetType type = new BitSetType(false, constraint, null);

            type.SetTag(true, this.Tag);
            return type;
        }

        /// <summary>
        /// Creates a constrained type reference to this type. The type
        /// reference is normally an identical type, but with the
        /// primitive flag set to false. Only certain types support being
        /// referenced, and the default implementation of this method
        /// throws an exception.     
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="values">The type value symbols</param>
        /// <returns>The MIB type reference</returns>
        public override MibType CreateReference(IList<MibValueSymbol> values)
        {
            BitSetType type;

            type = new BitSetType(false, null, null);
            type.CreateValueConstraints(values);
            type.SetTag(false, this.Tag);
            return type;
        }
       
        /// <summary>
        /// Checks if the specified value is compatible with this type.  A
        /// value is compatible if it is a bit set value with all
        /// components compatible with the constraints.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public override bool IsCompatible(MibValue value)
        {
            return value is BitSetValue
                    && this.IsCompatible((BitSetValue)value);
        }

        /// <summary>
        /// Checks if the specified bit set value is compatible with this
        /// type.  A value is compatible if all the bits are compatible
        /// with the constraints.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public bool IsCompatible(BitSetValue value)
        {
            if (this.constraint == null)
            {
                return true;
            }

            IList<NumberValue> bits = value.Bits;
            foreach (NumberValue v in bits)
            {
                if (!this.constraint.IsCompatible(v))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Searches for a named MIB symbol. This method may search outside
        /// the normal (or strict) scope, thereby allowing a form of
        /// relaxed search. Note that the results from the normal and
        /// expanded search may not be identical, due to the context
        /// chaining and the same symbol name appearing in various
        /// contexts.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="name">The symbol name</param>
        /// <param name="expanded">The expanded scope flag</param>
        /// <returns>The MIB symbol, or null if none was found</returns>
        public MibSymbol FindSymbol(string name, bool expanded)
        {
            return this.GetSymbol(name);
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns> 
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(base.ToString());

            if (this.symbols.Count > 0)
            {
                builder.Append(" { ");
                string.Join(", ", this.symbols.Values.Select(sym => sym.Name + "(" + sym.Value + ")"));
                builder.Append(" }");
            }
            else if (this.constraint != null)
            {
                builder.Append(" (");
                builder.Append(this.constraint.ToString());
                builder.Append(")");
            }

            return builder.ToString();
        }
        
        /// <summary>
        /// Creates the constraints and symbol map from a list of value
        /// symbols.
        /// </summary>
        /// <param name="values">The list of value symbols</param>
        private void CreateValueConstraints(IList<MibValueSymbol> values)
        {
            ValueConstraint c;

            foreach (MibValueSymbol sym in values)
            {
                this.symbols[sym.Name] = sym;
                
                // TODO: check value constraint compability
                c = new ValueConstraint(null, sym.Value);
                if (this.constraint == null)
                {
                    this.constraint = c;
                }
                else
                {
                    this.constraint = new CompoundConstraint(this.constraint, c);
                }
            }
        }
    }
}
