// <copyright file="MibType.cs" company="None">
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
    using System.Collections.Generic;
    using MibbleSharp.Type;

    /// <summary><para>The base MIB type class. There are two categories of MIB types
    /// extending this class, primitive ASN.1 type and SNMP macro types.
    /// The primitive types are used in SNMP for transferring data on the
    /// wire. The SNMP macro types are used in the MIB files for adding
    /// additional information to the primitive types or values, such as
    /// descriptions and similar. Most of the SNMP macro types only
    /// support object identifier values, and can only be used at the top
    /// level. The primitive types support whatever values are appropriate
    /// for the specific type, and are normally used inside the SNMP macro
    /// types in a MIB file.</para>
    /// <para>
    /// The best way to extract the specific type information from a MIB
    /// type is to check the type instance and then cast the MibType
    /// object to the corresponding subtype. Each subtype have very
    /// different properties, which is why the API in this class is rather
    /// limited. The example below shows some skeleton code for extracting
    /// type information.
    /// </para>
    /// <c>    if (type instanceof SnmpObjectType) {
    ///        objectType = (SnmpObjectType) type;
    ///        ...
    ///    }</c>
    /// <para>
    /// Another way to check which type is at hand, is to query the type
    /// tags with the hasTag() method. In this way it is possible to
    /// distinguish between types using the same or a similar primitive
    /// ASN.1 type representation (such as <c>DisplayString</c> and <c>IpAddress</c>).
    /// This should normally be done in order to create a correct BER-
    /// or DER-encoding of the type. The example below illustrates how
    /// this could be done.
    /// </para>
    /// <c>    tag = type.getTag();
    ///    if (tag.getCategory() == MibTypeTag.UNIVERSAL) {
    ///        // Set BER and DER identifier bits 8 &amp; 7 to 00
    ///    } else if (tag.getCategory() == MibTypeTag.APPLICATION) {
    ///        // Set BER and DER identifier bits 8 &amp; 7 to 01 
    ///    }
    ///    ... 
    ///    if (!type.isPrimitive()) {
    ///        // Set BER and DER constructed bit
    ///    }</c>
    /// </summary>
    public abstract class MibType
    {
        /// <summary>
        /// The type name.
        /// </summary>
        private string name;

        /// <summary>
        /// The primitive type flag.
        /// </summary>
        private bool primitive;

        /// <summary>
        /// The type tag.
        /// </summary>
        private MibTypeTag tag = null;

        /// <summary>
        /// The type reference symbol. This is set to the referenced type
        /// symbol when resolving this type.
        /// </summary>
        private MibTypeSymbol reference = null;

        /// <summary>
        /// The type comment.
        /// </summary>
        private string comment = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MibType"/> class.
        /// </summary>
        /// <param name="name">The type name</param>
        /// <param name="primitive">The primitive type flag</param>
        protected MibType(string name, bool primitive)
        {
            this.name = name;
            this.primitive = primitive;
        }

        /// <summary>Gets or sets the type reference symbol. A type reference is created
        /// whenever a type is defined in a type assignment, and later
        /// referenced by name from some other symbol. The complete chain
        /// of type references is available by calling <c>getReference()</c>
        /// recursively on the type of the returned type symbol.
        /// In general, this method should be avoided as it is much better
        /// to rely on type tags to distinguish between two types with the
        /// same base type (such as <c>DisplayString</c> and <c>IpAddress</c>).
        /// </summary>
        /// <see cref="getTag"/>
        /// <see cref="Snmp.SnmpTextualConvention.findReference(MibType)"/>
        public MibTypeSymbol ReferenceSymbol
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
        /// Gets or sets the type comment.
        /// </summary>
        public string Comment
        {
            get
            {
                return this.comment;
            }

            set
            {
                this.comment = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this type represents a primitive type. The primitive
        /// types are the basic building blocks of the ASN.1 type system.
        /// By defining new types (that may be identical to a primitive
        /// type), the new type looses it's primitive status.
        /// </summary>
        public bool Primitive
        {
            get
            {
                return this.primitive;
            }
        }

        /// <summary>Gets or sets the type name</summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            protected set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets the type tag. The type tags consist of a category and
        /// value number, and are used to identify a specific type
        /// uniquely (such as <c>IpAddress</c> and similar). Most (if not all)
        /// SNMP types have unique tags that are normally inherited when
        /// the type is referenced. Type tags may also be chained
        /// together, in which case this method returns the first tag in
        /// the chain.
        /// </summary>
        public MibTypeTag Tag
        {
            get
            {
                return this.tag;
            }

            set
            {
                MibTypeTag next = this.tag;

                if (value != null)
                {
                    value.Next = next;
                }

                this.tag = value;
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
        /// <param name="log">The MIB Loader Log</param>
        /// <returns>The basic MIB type</returns>
        /// <exception cref="MibException">If an error was encountered
        /// during initialization</exception>
        public abstract MibType Initialize(MibSymbol symbol, MibLoaderLog log);

        /// <summary>
        /// Creates a type reference to this type. The type reference is
        /// normally an identical type, but with the primitive flag set to
        /// false. Only certain types support being referenced, and the
        /// default implementation of this method throws an exception.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <returns>The MIB type reference</returns>
        public virtual MibType CreateReference()
        {
            return null;
        }

        /// <summary>
        /// Creates a constrained type reference to this type. The type
        /// reference is normally an identical type, but with the
        /// primitive flag set to false. Only certain types support being
        /// referenced, and the default implementation of this method
        /// throws an exception.
        /// Note: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="constraint">The type constraint</param>
        /// <returns>The MIB type reference</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown if the top level virtual method is ever called
        /// </exception>
        public virtual MibType CreateReference(Constraint constraint)
        {
            string msg = this.name + " type cannot be referenced with constraints";
            throw new NotSupportedException(msg);
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
        /// <returns>The MIB Type reference</returns>
        public virtual MibType CreateReference(IList<MibValueSymbol> values)
        {
            string msg = this.name + " type cannot be referenced with " +
                             "defined values";
            throw new NotSupportedException(msg);
        }

        /// <summary>
        /// Checks if the specified value is compatible with this type. A
        /// value is compatible if it has a type that matches this one and
        /// a value that satisfies all constraints.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public abstract bool IsCompatible(MibValue value);

        /// <summary>
        /// Checks if this type has a specific type tag. This method will
        /// check the whole type tag chain.
        /// </summary>
        /// <param name="tag">The type tag to search for</param>
        /// <returns>True if the specified type tag is present, false if not</returns>
        /// <see cref="HasTag(int, int)"/>
        public bool HasTag(MibTypeTag tag)
        {
            return this.HasTag(tag.Category, tag.Value);
        }

        /// <summary>
        /// Checks if this type has a specific type tag. This method will
        /// check the whole type tag chain.
        /// </summary>
        /// <param name="category">The tag category to search or</param>
        /// <param name="value">The tag value to search for</param>
        /// <returns>True if the specified type tag is present, false if not</returns>
        /// <see cref="HasTag(MibTypeTag)"/>
        public bool HasTag(int category, int value)
        {
            MibTypeTag iter = this.Tag;

            while (iter != null)
            {
                if (iter.Equals(category, value))
                {
                    return true;
                }

                iter = iter.Next;
            }

            return false;
        }

        /// <summary>
        /// Checks if this type referenced the specified type symbol. This
        /// method should be avoided if possible, as it is much better to
        /// rely on type tags to distinguish between two types with the
        /// same base type (such as <c>DisplayString</c> and <c>IpAddress</c>).
        /// </summary>
        /// <param name="name">The type symbol name</param>
        /// <returns>
        /// True if this type was a reference to the symbol, or
        /// false otherwise
        /// </returns>
        /// <see cref="HasTag(int, int)"/>
        /// <see cref="HasTag(MibTypeTag)"/>
        /// <see cref="ReferenceSymbol"/>
        /// <see cref="Snmp.SnmpTextualConvention.findReference(MibType)"/>
        public bool HasReferenceTo(string name)
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
                return this.reference.Type.HasReferenceTo(name);
            }
        }

        /// <summary>
        /// Checks if this type referenced the specified type symbol. This
        /// method should be avoided if possible, as it is much better to
        /// rely on type tags to distinguish between two types with the
        /// same base type (such as <c>DisplayString</c> and <c>IpAddress</c>).
        /// </summary>
        /// <param name="module">The type symbol module (MIB) name</param>
        /// <param name="name">The type symbol name</param>
        /// <returns>True if this type was a reference to the symbol, or
        /// false otherwise
        /// </returns>
        /// <see cref="HasTag(int, int)"/>
        /// <see cref="HasTag(MibTypeTag)"/>
        /// <see cref="ReferenceSymbol"/>
        /// <see cref="Snmp.SnmpTextualConvention.findReference(MibType)"/>
        public bool HasReferenceTo(string module, string name)
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
                return this.reference.Type.HasReferenceTo(module, name);
            }
        }

        /// <summary>
        /// Sets the type tag. The old type tag is kept to some extent,
        /// depending on if the implicit flag is set to true or false. For
        /// implicit inheritance, the first tag in the old tag chain is
        /// replaced with the new tag. For explicit inheritance, the new
        /// tag is added first in the tag chain without removing any old
        /// tag.
        /// NOTE: This is an internal method that should
        /// only be called by the MIB loader.
        /// </summary>
        /// <param name="implicitly">the implicit inheritance flag</param>
        /// <param name="newtag">the new type tag</param>
        public virtual void SetTag(bool implicitly, MibTypeTag newtag)
        {
            MibTypeTag next = this.tag;

            if (implicitly && next != null)
            {
                next = next.Next;
            }

            if (newtag != null)
            {
                newtag.Next = next;
            }

            this.tag = newtag;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            if (this.tag != null)
            {
                return this.tag.ToString() + " " + this.name;
            }
            else
            {
                return this.name;
            }
        }
    }
}
