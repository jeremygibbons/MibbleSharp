//
// TypeReference.cs
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

namespace MibbleSharp.Type
{
    /**
     * A reference to a type symbol.<p>
     *
     * <strong>NOTE:</strong> This class is used internally during the
     * MIB parsing only. After loading a MIB file successfully, all type
     * references will have been resolved to other MIB types. Do
     * <strong>NOT</strong> use or reference this class.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class TypeReference : MibType, MibContext
    {

        /**
         * The reference location.
         */
        private FileLocation location;

        /**
         * The reference context.
         */
        private MibContext context;

        /**
         * The referenced type.
         */
        private MibType type = null;

        /**
         * The additional type constraints.
         */
        private Constraint constraint = null;

        /**
         * The additional defined symbols.
         */
        private IList<MibValueSymbol> values = null;

        /**
         * The MIB type tag to set on the referenced type.
         */
        private MibTypeTag tag = null;

        /**
         * The implicit type tag flag.
         */
        private bool implicitTag = true;

        /**
         * Creates a new type reference.
         *
         * @param location       the reference location
         * @param context        the reference context
         * @param name           the reference name
         */
        public TypeReference(FileLocation location,
                             MibContext context,
                             string name)
                : base("ReferenceToType(" + name + ")", false)
        {

            this.location = location;
            this.context = context;
            this.name = name;
        }

        /**
         * Creates a new type reference.
         *
         * @param location       the reference location
         * @param context        the reference context
         * @param name           the reference name
         * @param constraint     the additional type constraint
         */
        public TypeReference(FileLocation location,
                             MibContext context,
                             string name,
                             Constraint constraint)
            : this(location, context, name)
        {
            this.constraint = constraint;
        }

        /**
         * Creates a new type reference.
         *
         * @param location       the reference location
         * @param context        the reference context
         * @param name           the reference name
         * @param values         the additional defined symbols
         */
        public TypeReference(FileLocation location,
                             MibContext context,
                             string name,
                             IList<MibValueSymbol> values)
            : this(location, context, name)
        {
            this.values = values;
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

            MibSymbol sym;
            string message;

            sym = getSymbol(log);
            if (sym is MibTypeSymbol)
            {
                type = InitializeReference(symbol, log, (MibTypeSymbol)sym);
                if (type == null)
                {
                    message = "referenced symbol '" + sym.getName() +
                              "' contains undefined type";
                    throw new MibException(location, message);
                }
                return type;
            }
            else if (sym == null)
            {
                message = "undefined symbol '" + name + "'";
                throw new MibException(location, message);
            }
            else
            {
                message = "referenced symbol '" + name + "' is not a type";
                throw new MibException(location, message);
            }
        }

        /**
         * Initializes the referenced MIB type symbol. This will remove
         * all levels of indirection present, such as references to other
         * types, and returns the basic type. This method will add any
         * constraints or defined values if possible.
         *
         * @param symbol         the MIB symbol containing this type
         * @param log            the MIB loader log
         * @param ref            the referenced MIB type symbol
         *
         * @return the basic MIB type, or
         *         null if the basic type was unresolved
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        private MibType InitializeReference(MibSymbol symbol,
                                            MibLoaderLog log,
                                            MibTypeSymbol tref)
        {

            MibType type = tref.getType();

            if (type != null)
            {
                type = type.Initialize(symbol, log);
            }
            if (type == null)
            {
                return null;
            }
            try
            {
                if (constraint != null)
                {
                    type = type.CreateReference(constraint);
                }
                else if (values != null)
                {
                    type = type.CreateReference(values);
                }
                else
                {
                    type = type.CreateReference();
                }
                type = type.Initialize(symbol, log);
            }
            catch (NotSupportedException e)
            {
                throw new MibException(location, e.Message);
            }
            type.ReferenceSymbol = tref;
            initializeTypeTag(type, tag);
            return type;
        }

        /**
         * Initializes the type tags for the specified type. The type tag
         * may be part in a chain of type tags, in which case the chain
         * is preserved. The last tag in the chain will be added first,
         * in order to be able to override (or preserve) a previous tag.
         *
         * @param type           the MIB type
         * @param tag            the MIB type tag
         */
        private void initializeTypeTag(MibType type, MibTypeTag tag)
        {
            if (tag == null)
            {
                // Do nothing
            }
            else if (tag.Next == null)
            {
                type.setTag(implicitTag, tag);
            }
            else
            {
                initializeTypeTag(type, tag.Next);
                type.setTag(false, tag);
            }
        }

        /**
         * Returns the file containing the reference.
         *
         * @return the file containing the reference
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

            sym = context.findSymbol(name, false);
            if (sym == null)
            {
                sym = context.findSymbol(name, true);
                if (sym != null && log != null)
                {
                    message = "missing import for '" + name + "', using " +
                              "definition from " + sym.getMib().getName();
                    log.addWarning(location, message);
                }
            }
            return sym;
        }

        /**
         * Checks if the specified value is compatible with this type.
         * This metod will always return false for referenced types.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        public override bool IsCompatible(MibValue value)
        {
            return false;
        }

        /**
         * Searches for a named MIB symbol. This method may search outside
         * the normal (or strict) scope, thereby allowing a form of
         * relaxed search. Note that the results from the normal and
         * expanded search may not be identical, due to the context
         * chaining and the same symbol name appearing in various
         * contexts. This method checks the referenced type for a
         * MibContext implementation.<p>
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
            if (type is MibContext)
            {
                return ((MibContext)type).findSymbol(name, expanded);
            }
            else
            {
                return null;
            }
        }

        /**
         * Sets the type tag. This method will keep the type tag stored
         * until the type reference is resolved.
         *
         * @param implicit       the implicit inheritance flag
         * @param tag            the new type tag
         *
         * @since 2.2
         */
        public override void setTag(bool implicitly, MibTypeTag tag)
        {
            if (this.tag == null)
            {
                this.tag = tag;
                this.implicitTag = implicitly;
            }
            else if (implicitly)
            {
                tag.Next = this.tag.Next;
                this.tag = tag;
            }
            else
            {
                tag.Next = this.tag;
                this.tag = tag;
            }
        }
    }
}
