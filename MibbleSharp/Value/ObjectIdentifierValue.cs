// <copyright file="ObjectIdentifierValue.cs" company="None">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///  An object identifier value. This class stores the component
    /// identifier values in a tree hierarchy.
    /// </summary>
    public class ObjectIdentifierValue : MibValue
    {
        /// <summary>
        /// The declaration file location. This variable is only used when
        /// resolving value references in order to present correct error
        /// messages. After initialization it is set to null to minimize
        /// memory impact.
        /// </summary>  
        private FileLocation location = null;

        /// <summary>
        /// The component parent.
        /// </summary>
        private MibValue parent;

        /// <summary>
        /// The component children.
        /// </summary>
        private IList<ObjectIdentifierValue> children = new List<ObjectIdentifierValue>();

        /// <summary>
        /// The object identifier component name.
        /// </summary>
        private string name;

        /// <summary>
        /// The object identifier component value.
        /// </summary>
        private int value;

        /// <summary>
        /// The MIB value symbol referenced by this object identifier.
        /// </summary>
        private MibValueSymbol symbol = null;

        /// <summary>
        /// The cached numeric string representation of this value. This
        /// variable is set when calling the toString() method the first
        /// time and is used to optimize performance by avoiding any
        /// subsequent recursive calls.
        /// </summary>
        /// <see cref="ToString"/>         
        private string cachedNumericValue = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIdentifierValue"/> class.
        /// </summary>
        /// <param name="name">The component name, or null</param>
        /// <param name="value">The component value</param>
        public ObjectIdentifierValue(string name, int value) : base("OBJECT IDENTIFIER")
        {
            this.parent = null;
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIdentifierValue"/> class.
        /// </summary>
        /// <param name="location">The declaration file location</param>
        /// <param name="parent">The component parent</param>
        /// <param name="name">The component name</param>
        /// <param name="value">The component value</param>
        /// <exception cref="MibException">
        /// If the object identifier parent already has a child with
        /// the specified value
        /// </exception>
        public ObjectIdentifierValue(
            FileLocation location,
            ObjectIdentifierValue parent,
            string name,
            int value)
            : base("OBJECT IDENTIFIER")
        {
            this.parent = parent;
            this.name = name;
            this.value = value;
            if (parent.GetChildByValue(value) != null)
            {
                throw new MibException(
                    location,
                    "cannot add duplicate OID children with value " + value);
            }

            parent.AddChild(null, location, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIdentifierValue"/> class.
        /// </summary>
        /// <param name="location">The declaration file location</param>
        /// <param name="parent">The component parent</param>
        /// <param name="name">The component name</param>
        /// <param name="value">The component value</param>
        public ObjectIdentifierValue(
            FileLocation location,
            ValueReference parent,
            string name,
            int value)
            : base("OBJECT IDENTIFIER")
        {
            this.location = location;
            this.parent = parent;
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets the parent object identifier value.
        /// </summary>
        public ObjectIdentifierValue Parent
        {
            get
            {
                return this.parent as ObjectIdentifierValue;
            }
        }

        /// <summary>
        /// Gets this object identifier component value.
        /// </summary>  
        public int Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Gets or sets the symbol connected to this object identifier.
        /// </summary> 
        /// <remarks>
        /// This is an internal property that should only be accessed by
        /// the MIB loader.
        /// </remarks>
        public MibValueSymbol Symbol
        {
            get
            {
                return this.symbol;
            }

            set
            {
                if (this.name == null)
                {
                    this.name = value.Name;
                }

                this.symbol = value;
            }
        }
        
        /// <summary>
        /// Gets the number of child object identifier values.
        /// </summary>
        public int ChildCount
        {
            get
            {
                return this.children == null ? 0 : this.children.Count;
            }
        }

        /// <summary>
        /// Gets an enumeration of all child object identifier values. The
        /// children are ordered by their value, not necessarily in the
        /// order in which they appear in the original MIB file.
        /// </summary>
        /// <returns>The enumeration of child OIVs</returns>
        public IEnumerable<ObjectIdentifierValue> Children
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// Gets a detailed string representation of this value. The
        /// string will contain the full numeric object identifier value
        /// with optional names for each component.
        /// </summary>
        public string ToDetailString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                if (this.parent is ObjectIdentifierValue)
                {
                    builder.Append(((ObjectIdentifierValue)this.parent).ToDetailString);
                    builder.Append(".");
                }

                if (this.name == null)
                {
                    builder.Append(this.value);
                }
                else
                {
                    builder.Append(this.name);
                    builder.Append("(");
                    builder.Append(this.value);
                    builder.Append(")");
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets an ASN.1 representation of this value. The string will
        /// contain references to any parent OID value that can be found.
        /// </summary>
        public string Asn1String
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                ObjectIdentifierValue oivref;

                if (this.parent is ObjectIdentifierValue)
                {
                    oivref = (ObjectIdentifierValue)this.parent;
                    if (oivref.Symbol == null)
                    {
                        builder.Append(oivref.Asn1String);
                    }
                    else
                    {
                        builder.Append(oivref.Symbol.Name);
                    }

                    builder.Append(" ");
                }

                if (this.name == null || this.Symbol != null)
                {
                    builder.Append(this.value);
                }
                else
                {
                    builder.Append(this.name);
                    builder.Append("(");
                    builder.Append(this.value);
                    builder.Append(")");
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the MIB that this object identifier is connected to.
        /// This method simply returns the symbol MIB.
        /// </summary>
        private Mib Mib
        {
            get
            {
                return this.symbol == null ? null : this.symbol.Mib;
            }
        }

        /// <summary>
        /// Initializes the MIB value. This will remove all levels of
        /// indirection present, such as references to other values. No
        /// value information is lost by this operation. This method may
        /// modify this object as a side-effect, and will return the basic
        /// value.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        /// <param name="log">The MIB Loader log</param>
        /// <param name="type">The value type</param>
        /// <returns>The basic MIB value</returns>         
        public override MibValue Initialize(MibLoaderLog log, MibType type)
        {
            ValueReference vref = null;
            ObjectIdentifierValue oid;

            if (this.parent == null)
            {
                return this;
            }
            else if (this.parent is ValueReference)
            {
                vref = (ValueReference)this.parent;
            }

            this.parent = this.parent.Initialize(log, type);

            if (vref != null)
            {
                if (this.parent is ObjectIdentifierValue)
                {
                    oid = (ObjectIdentifierValue)this.parent;
                    oid.AddChild(log, this.location, this);
                }
                else
                {
                    throw new MibException(
                        vref.Location,
                        "referenced value is not an object identifier");
                }
            }

            this.location = null;

            if (this.parent is ObjectIdentifierValue)
            {
                return ((ObjectIdentifierValue)this.parent).GetChildByValue(this.value);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Creates a value reference to this value. The value reference
        /// is normally an identical value. Only certain values support
        /// being referenced, and the default implementation of this
        /// method throws an exception.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        /// <returns>The MIB value reference
        /// </returns>
        public override MibValue CreateReference()
        {
            return this;
        }

        /// <summary>
        /// Clears and prepares this value for garbage collection. This
        /// method will recursively clear any associated types or values,
        /// making sure that no data structures references this object.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        public override void Clear()
        {
            Mib mib;

            // Recursively clear all children in same MIB
            if (this.children != null)
            {
                mib = this.Mib;
                foreach (var c in this.children)
                {
                    c.Clear();
                }
            }

            // Remove parent reference if all children were cleared 
            if (this.ChildCount <= 0)
            {
                if (this.parent != null)
                {
                    this.Parent.children.Remove(this);
                    this.parent = null;
                }

                this.children = null;
            }

            // Clear other value data
            this.symbol = null;
            base.Clear();
        }

        /// <summary>
        /// Checks if this object equals another object. This method will
        /// compare the string representations for equality.
        /// </summary>
        /// <param name="obj">The object to compare with</param>
        /// <returns>True if the objects are equal, false if not</returns>
        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj.ToString());
        }

        /// <summary>
        /// Returns a hash code for this object.
        /// </summary>
        /// <returns>A hash code for this object</returns>         
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Returns a child object identifier value. The children are
        /// ordered by their value, not necessarily in the order in which
        /// they appear in the original MIB file.
        /// </summary>
        /// <param name="index">The child position</param>
        /// <returns>The child OIV, or null if not found</returns>
        public ObjectIdentifierValue GetChild(int index)
        {
            return this.children[index];
        }

        /// <summary>
        /// Returns a child object identifier value. The children are
        /// searched by their component names. This method uses linear
        /// search and therefore has time complexity O(n). Note that most
        /// OID:s don't have a component name, but only an associated
        /// symbol.
        /// </summary>
        /// <param name="name">The child name</param>
        /// <returns>The child OIV, or null if not found</returns>
        public ObjectIdentifierValue GetChildByName(string name)
        {
            return this.children.Where(c => name.Equals(c.Name)).FirstOrDefault();
        }

        /// <summary>
        /// Returns a child object identifier value. The children are
        /// searched by their numerical value. This method uses binary
        /// search and therefore has time complexity O(log(n)).
        /// </summary>
        /// <param name="value">The child value</param>
        /// <returns>The child OIV, or null if not found</returns>
        public ObjectIdentifierValue GetChildByValue(int value)
        {
            ObjectIdentifierValue child;
            int low = 0;
            int high = this.children.Count;
            int pos;

            if (low < value && value <= high)
            {
                // Default to that the value is really the index - 1 
                pos = value - 1;
            }
            else
            {
                // Otherwise use normal interval midpoint
                pos = (low + high) / 2;
            }

            while (low < high)
            {
                child = this.children[(int)pos];
                if (child.Value == value)
                {
                    return child;
                }
                else if (child.Value < value)
                {
                    low = (int)pos + 1;
                }
                else
                {
                    high = (int)pos;
                }

                pos = (low + high) / 2;
            }

            return null;
        }
        
        /// <summary>
        /// Returns a string representation of this value. The string will
        /// contain the full numeric object identifier value with each
        /// component separated with a dot ('.').
        /// </summary>
        /// <returns>A string representation of this value</returns>
        public override string ToString()
        {
            StringBuilder builder;

            if (this.cachedNumericValue == null)
            {
                builder = new StringBuilder();
                if (this.parent != null)
                {
                    builder.Append(this.parent.ToString());
                    builder.Append(".");
                }

                builder.Append(this.value);
                this.cachedNumericValue = builder.ToString();
            }

            return this.cachedNumericValue;
        }

        /// <summary>
        /// Compares this object to another MIB Value.
        /// </summary>
        /// <param name="other">The object to compare against</param>
        /// <returns>0 if both objects are equal, a non-zero integer if not</returns>
        public override int CompareTo(MibValue other)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Merges this object identifier value with another one. One of
        /// the two objects will be discarded and the other will be used
        /// as the merge destination and returned. Note that this
        /// operation modifies both this value and the specified value.
        /// The merge can only be made under certain conditions, for
        /// example that no child OID:s have name conflicts. It is also
        /// assumed that the two OID:s have the same numerical value.
        /// </summary>
        /// <param name="log">The MIB loader log</param>
        /// <param name="location">The file location on error</param>
        /// <param name="value">The OID value to merge with</param>
        /// <returns>The merged object identifier value</returns>
        /// <exception cref="MibException">
        /// If the merge couldn't be performed due to a conflict or
        /// invalid state.
        /// </exception>
        private ObjectIdentifierValue Merge(
            MibLoaderLog log,
            FileLocation location,
            ObjectIdentifierValue value)
        {
            if (this.symbol != null || (value.symbol == null && this.children.Count > 0))
            {
                this.AddChildren(log, location, value);
                return this;
            }
            else
            {
                value.AddChildren(log, location, this);
                return value;
            }
        } 
        
        /// <summary>
        /// Adds a child component. The children will be inserted in the
        /// value order. If a child with the same value has already been
        /// added, the new child will be merged with the previous one (if
        /// possible) and the resulting child will be returned.
        /// </summary>
        /// <param name="log">The MIB loader log</param>
        /// <param name="location">The file location on error</param>
        /// <param name="child">The child component</param>
        /// <returns>The child OIV added</returns>
        /// <exception cref="MibException">If an irrecoverable conflict
        /// between two children occurred</exception>
        private ObjectIdentifierValue AddChild(
            MibLoaderLog log,
            FileLocation location,
            ObjectIdentifierValue child)
        {
            ObjectIdentifierValue value;
            int i = this.children.Count;

            // Insert child in value order, searching backwards to 
            // optimize the most common case (ordered insertion)
            while (i > 0)
            {
                value = this.children[i - 1];
                if (value.Value == child.Value)
                {
                    value = value.Merge(log, location, child);
                    this.children[i - 1] = value;
                    return value;
                }
                else if (value.Value < child.Value)
                {
                    break;
                }

                i--;
            }

            this.children.Insert(i, child);
            return child;
        }

        /// <summary>
        /// Adds all the children from another object identifier value.
        /// The children are not copied, but actually transferred from the
        /// other value. If this value lacks a name component, it will be
        /// set from other value. This operation thus corresponds to a
        /// merge and thus can only be made under certain conditions. For
        /// example, no child OID:s may have name conflicts. It is assumed
        /// that the other OID has the same numerical value as this one.
        /// </summary>
        /// <param name="log">The MIB loader log</param>
        /// <param name="location">The file location on error</param>
        /// <param name="parent">The OID parent value for the children</param>
        /// <exception cref="MibException">If an irrecoverable conflict
        /// between two children occurred</exception>
        private void AddChildren(
            MibLoaderLog log,
            FileLocation location,
            ObjectIdentifierValue parent)
        {
            ObjectIdentifierValue child;
            string msg;

            if (this.name == null)
            {
                this.name = parent.name;
            }
            else if (parent.name != null && !parent.name.Equals(this.name))
            {
                msg = "OID component '" + parent.name + "' was previously " +
                      "defined as '" + this.name + "'";
                if (log == null)
                {
                    throw new MibException(location, msg);
                }
                else
                {
                    log.AddWarning(location, msg);
                }
            }

            if (parent.symbol != null)
            {
                throw new MibException(
                    location,
                    "INTERNAL ERROR: OID merge with symbol reference already set");
            }

            for (int i = 0; i < parent.children.Count; i++)
            {
                child = parent.children[i];
                child.parent = this;
                this.AddChild(log, location, child);
            }

            parent.children = null;
        }
    }
}
