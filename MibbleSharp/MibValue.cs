// <copyright file="MibValue.cs" company="None">
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

namespace MibbleSharp
{
    using System;

    /// <summary>
    /// The base MIB value class. There are only a few MIB value classes,
    /// each corresponding to a primitive ASN.1 type.
    /// </summary>
    public abstract class MibValue : IComparable<MibValue>
    {
        /// <summary>
        /// The value name
        /// </summary>
        private string name;

        /// <summary>
        /// The value reference symbol. This is set to the referenced
        /// value symbol when resolving this value.
        /// </summary>
        private MibValueSymbol reference = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MibValue"/> class.
        /// </summary>
        /// <param name="name">The value name</param>
        protected MibValue(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets the value name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets or sets the value reference symbol. A value reference is
        /// created whenever a value is defined in a value assignment, and
        /// later referenced by name from some other symbol. The complete
        /// chain of value references is available by calling
        /// getReference() recursively on the value of the returned value
        /// symbol.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        public MibValueSymbol ReferenceSymbol
        {
            get
            {
                return this.reference;
            }

            set
            {
                this.reference = value;
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
        /// <param name="log">The MIB loader log</param>
        /// <param name="type">The value type</param>
        /// <returns>The basic MIB value</returns>
        /// <exception cref="MibException">
        /// If an error was encountered during the initialization
        /// </exception>
        public abstract MibValue Initialize(MibLoaderLog log, MibType type);

        /// <summary>
        /// Creates a value reference to this value. The value reference
        /// is normally an identical value. Only certain values support
        /// being referenced, and the default implementation of this
        /// method throws an exception.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <returns>The MIB value reference</returns>
        public virtual MibValue CreateReference()
        {
            string msg = this.name + " value cannot be referenced";
            throw new NotSupportedException(msg);
        }

        /// <summary>
        /// Clears and prepares this value for garbage collection. This
        /// method will recursively clear any associated types or values,
        /// making sure that no data structures references this object.
        /// NOTE: This is an internal method that should only be called 
        /// by the MIB loader.
        /// </summary>
        public virtual void Clear()
        {
            this.reference = null;
        }

        /// <summary>
        /// Checks if this value referenced the specified value symbol.
        /// </summary>
        /// <param name="name">The value symbol name</param>
        /// <returns>
        /// True if this value was a reference to the symbol, 
        /// false if not
        /// </returns>
        public bool IsReferenceTo(string name)
        {
            if (this.reference == null)
            {
                return false;
            }
            else if (this.reference.Name.Equals(name))
            {
                return true;
            }
            else
            {
                return this.reference.getValue().IsReferenceTo(name);
            }
        }

        /// <summary>
        /// Checks if this value referenced the specified value symbol.
        /// </summary>
        /// <param name="module">The value symbol module (MIB) name</param>
        /// <param name="name">The value symbol name</param>
        /// <returns>
        /// True if this value was a reference to the symbol, or false otherwise
        /// </returns>
        public bool IsReferenceTo(string module, string name)
        {
            Mib mib;

            if (this.reference == null)
            {
                return false;
            }

            mib = this.reference.Mib;

            if (mib.Name.Equals(module) 
                && this.reference.Name.Equals(name))
            {
                return true;
            }
            else
            {
                return this.reference.getValue().IsReferenceTo(module, name);
            }
        }

        /// <summary>
        /// Compare this object to another MibValue
        /// </summary>
        /// <param name="other">The object to compare to</param>
        /// <returns>
        /// 0 if other is equal to this object, a negative 
        /// integer if this object is smaller than other, a positive 
        /// if this object is greater than other
        /// </returns>
        public abstract int CompareTo(MibValue other);
    }
}
