// <copyright file="TypeReference.cs" company="None">
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

    /// <summary>
    /// A reference to a type symbol.  
    /// </summary>
    /// <remarks>
    /// NOTE: This class is used internally during the
    /// MIB parsing only. After loading a MIB file successfully, all type
    /// references will have been resolved to other MIB types. Do
    /// NOT use or reference this class.
    /// </remarks>
    public class TypeReference : MibType, IMibContext
    {
        /// <summary>
        /// The reference location.
        /// </summary>
        private FileLocation location;

        /// <summary>
        /// The reference context.
        /// </summary>
        private IMibContext context;

        /// <summary>
        /// The referenced type.
        /// </summary>
        private MibType type = null;

        /// <summary>
        /// The additional type constraints.
        /// </summary>
        private IConstraint constraint = null;

        /// <summary>
        /// The additional defined symbols.
        /// </summary>
        private IList<MibValueSymbol> values = null;

        /// <summary>
        /// The MIB type tag to set on the referenced type.
        /// </summary>
        private MibTypeTag tag = null;

        /// <summary>
        /// The implicit type tag flag.
        /// </summary>
        private bool implicitTag = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        /// <param name="location">The reference location</param>
        /// <param name="context">The reference context</param>
        /// <param name="name">The reference name</param>
        public TypeReference(
            FileLocation location,
            IMibContext context,
            string name)
            : base("ReferenceToType(" + name + ")", false)
        {
            this.location = location;
            this.context = context;
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        /// <param name="location">The reference location</param>
        /// <param name="context">The reference context</param>
        /// <param name="name">The reference name</param>
        /// <param name="constraint">The additional type constraint</param>
        public TypeReference(
            FileLocation location,
            IMibContext context,
            string name,
            IConstraint constraint)
            : this(location, context, name)
        {
            this.constraint = constraint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        /// <param name="location">The reference location</param>
        /// <param name="context">The reference context</param>
        /// <param name="name">The reference name</param>
        /// <param name="values">The additional defined symbols</param>
        public TypeReference(
            FileLocation location,
            IMibContext context,
            string name,
            IList<MibValueSymbol> values)
            : this(location, context, name)
        {
            this.values = values;
        }

        /// <summary>
        /// Gets the file containing the reference.
        /// </summary>
        public FileLocation Location
        {
            get
            {
                return this.location;
            }
        }

        /// <summary>
        /// Gets the referenced symbol.
        /// </summary>
        public MibSymbol Symbol
        {
            get
            {
                return this.GetSymbol(null);
            }
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
        /// <param name="symbol">The MIB symbol containing this type</param>
        /// <param name="log">The MIB loader log</param>
        /// <returns>The basic MIB type</returns>
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            MibSymbol sym;
            string message;

            sym = this.GetSymbol(log);
            if (sym is MibTypeSymbol)
            {
                this.type = this.InitializeReference(symbol, log, (MibTypeSymbol)sym);
                if (this.type == null)
                {
                    message = "referenced symbol '" + sym.Name +
                              "' contains undefined type";
                    throw new MibException(this.location, message);
                }

                return this.type;
            }
            else if (sym == null)
            {
                message = "undefined symbol '" + this.Name + "'";
                throw new MibException(this.location, message);
            }
            else
            {
                message = "referenced symbol '" + this.Name + "' is not a type";
                throw new MibException(this.location, message);
            }
        }
        
        /// <summary>
        /// Checks if the specified value is compatible with this
        /// type. This method always returns false for reference types
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, or
        /// false otherwise
        /// </returns>
        public override bool IsCompatible(MibValue value)
        {
            return false;
        }

        /// <summary>
        /// Searches for a named MIB symbol. This method may search outside
        /// the normal (or strict) scope, thereby allowing a form of
        /// relaxed search. Note that the results from the normal and
        /// expanded search may not be identical, due to the context
        /// chaining and the same symbol name appearing in various
        /// contexts. This method checks the referenced type for a
        /// MibContext implementation.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="name">The symbol name</param>
        /// <param name="expanded">The expanded scope flag</param>
        /// <returns>The MIB symbol, or null if none was found</returns>
        public MibSymbol FindSymbol(string name, bool expanded)
        {
            IMibContext mc = this.type as IMibContext;
            if (mc == null)
            {
                return null;
            }

            return mc.FindSymbol(name, expanded);
        }

        /// <summary>
        /// Sets the type tag. This method will keep the type tag stored
        /// until the type reference is resolved.
        /// </summary>
        /// <param name="implicitly">The implicit inheritance tag</param>
        /// <param name="tag">The new type tag</param>
        public override void SetTag(bool implicitly, MibTypeTag tag)
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
        
        /// <summary>
        /// Initializes the referenced MIB type symbol. This will remove
        /// all levels of indirection present, such as references to other
        /// types, and returns the basic type. This method will add any
        /// constraints or defined values if possible.
        /// </summary>
        /// <param name="symbol">the MIB symbol containing this type</param>
        /// <param name="log">The MIB loader log</param>
        /// <param name="tref">the referenced MIB type symbol</param>
        /// <returns>
        /// The basic MIB type, or null if the basic type was unresolved
        /// </returns>
        /// <exception cref="MibException">
        /// If an error was encountered during the
        /// initialization</exception>
        private MibType InitializeReference(
            MibSymbol symbol,
            MibLoaderLog log,
            MibTypeSymbol tref)
        {
            MibType type = tref.Type;

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
                if (this.constraint != null)
                {
                    type = type.CreateReference(this.constraint);
                }
                else if (this.values != null)
                {
                    type = type.CreateReference(this.values);
                }
                else
                {
                    type = type.CreateReference();
                }

                type = type.Initialize(symbol, log);
            }
            catch (NotSupportedException e)
            {
                throw new MibException(this.location, e.Message);
            }

            type.ReferenceSymbol = tref;
            this.InitializeTypeTag(type, this.tag);
            return type;
        }

        /// <summary>
        /// Initializes the type tags for the specified type. The type tag
        /// may be part in a chain of type tags, in which case the chain
        /// is preserved. The last tag in the chain will be added first,
        /// in order to be able to override (or preserve) a previous tag.
        /// </summary>
        /// <param name="type">The MIB type</param>
        /// <param name="tag">The MIB type tag</param>
        private void InitializeTypeTag(MibType type, MibTypeTag tag)
        {
            if (tag == null)
            {
                return;
            }
            else if (tag.Next == null)
            {
                type.SetTag(this.implicitTag, tag);
            }
            else
            {
                this.InitializeTypeTag(type, tag.Next);
                type.SetTag(false, tag);
            }
        }

        /// <summary>
        /// Gets the referenced symbol.
        /// </summary>
        /// <param name="log">The optional loader log</param>
        /// <returns>The referenced symbol</returns>
        private MibSymbol GetSymbol(MibLoaderLog log)
        {
            MibSymbol sym;
            string message;

            sym = this.context.FindSymbol(this.Name, false);

            if (sym == null)
            {
                sym = this.context.FindSymbol(this.Name, true);
                if (sym != null && log != null)
                {
                    message = "missing import for '" + this.Name + "', using " +
                              "definition from " + sym.Mib.Name;
                    log.AddWarning(this.location, message);
                }
            }

            return sym;
        }
    }
}
