// <copyright file="ValueReference.cs" company="None">
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

namespace MibbleSharp.Value
{
    using System;

    /// <summary>
    /// A reference to a value symbol.
    /// NOTE: This class is used internally during the
    /// MIB parsing only. After loading a MIB file successfully, all value
    /// references will have been resolved to other MIB values. Do
    /// NOT use or reference this class.
    /// </summary>
    public class ValueReference : MibValue
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
        /// The referenced name.
        /// </summary>
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueReference"/> class.
        /// </summary>
        /// <param name="location">The reference file location</param>
        /// <param name="context">The reference context</param>
        /// <param name="name">The reference name</param>
        public ValueReference(
            FileLocation location,
            IMibContext context,
            string name)
            : base("ReferenceToValue(" + name + ")")
        {
            this.location = location;
            this.context = context;
            this.name = name;
        }

        /// <summary>
        /// Gets the reference location.
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
        /// Initializes the MIB value. This will remove all levels of
        /// indirection present, such as references to other values. No
        /// value information is lost by this operation. This method may
        /// modify this object as a side-effect, and will return the basic
        /// value.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="log">The MIB Loader log</param>
        /// <param name="type">The value type</param>
        /// <returns>The basic MIB value</returns>
        /// <exception cref="MibException">
        /// If an error was encountered during the initialization</exception>
        public override MibValue Initialize(MibLoaderLog log, MibType type)
        {
            MibSymbol sym;
            MibValue value;
            string message;

            sym = this.GetSymbol(log);
            if (sym is MibValueSymbol)
            {
                value = ((MibValueSymbol)sym).Value;
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
                    throw new MibException(this.location, e.Message);
                }

                if (!(value is ObjectIdentifierValue))
                {
                    value.ReferenceSymbol = (MibValueSymbol)sym;
                }

                return value;
            }
            else if (sym == null)
            {
                message = "undefined symbol '" + this.name + "'";
                throw new MibException(this.location, message);
            }
            else
            {
                message = "referenced symbol '" + this.name + "' is not a value";
                throw new MibException(this.location, message);
            }
        }
        
        /// <summary>
        /// Compares this ValueReference to another MibValue
        /// </summary>
        /// <param name="other">The value to compare against</param>
        /// <returns>0 if the two are equal, a non-zero integer if not</returns>
        public override int CompareTo(MibValue other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if this object equals another object. This method will
        /// compare the string representations for equality.
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if the objects are equal, false if not</returns>
        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj.ToString());
        }

        /// <summary>
        /// Returns a hash code for this object
        /// </summary>
        /// <returns>A hash code for this object</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Get a string representation of the ValueReference
        /// </summary>
        /// <returns>A string representing ValueReference</returns>
        public override string ToString()
        {
            return "ReferenceToValue(" + this.name + ")";
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

            sym = this.context.FindSymbol(this.name, false);

            if (sym == null)
            {
                sym = this.context.FindSymbol(this.name, true);

                if (sym != null && log != null)
                {
                    message = "missing import for '" + this.name + "', using " +
                              "definition from " + sym.Mib.Name;
                    log.AddWarning(this.location, message);
                }
            }

            return sym;
        }
    }
}
