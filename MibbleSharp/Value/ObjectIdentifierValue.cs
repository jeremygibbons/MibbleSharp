//
// ObjectIdentifierValue.cs
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

namespace MibbleSharp.Value
{
    /**
     * An object identifier value. This class stores the component
     * identifier values in a tree hierarchy.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class ObjectIdentifierValue : MibValue
    {

        /**
         * The declaration file location. This variable is only used when
         * resolving value references in order to present correct error
         * messages. After initialization it is set to null to minimize
         * memory impact.
         *
         * @since 2.5
         */
        private FileLocation location = null;

        /**
         * The component parent.
         */
        private MibValue parent;

        /**
         * The component children.
         */
        private IList<ObjectIdentifierValue> children = new List<ObjectIdentifierValue>();

        /**
         * The object identifier component name.
         */
        private string name;

        /**
         * The object identifier component value.
         */
        private int value;

        /**
         * The MIB value symbol referenced by this object identifier.
         */
        private MibValueSymbol symbol = null;

        /**
         * The cached numeric string representation of this value. This
         * variable is set when calling the toString() method the first
         * time and is used to optimize performance by avoiding any
         * subsequent recursive calls.
         *
         * @see #toString()
         */
        private string cachedNumericValue = null;

        /**
         * Creates a new root object identifier value.
         *
         * @param name           the component name, or null
         * @param value          the component value
         */
        public ObjectIdentifierValue(string name, int value) : base("OBJECT IDENTIFIER")
        {
            this.parent = null;
            this.name = name;
            this.value = value;
        }

        /**
         * Creates a new object identifier value.
         *
         * @param location       the declaration file location
         * @param parent         the component parent
         * @param name           the component name, or null
         * @param value          the component value
         *
         * @throws MibException if the object identifier parent already
         *             had a child with the specified value
         */
        public ObjectIdentifierValue(FileLocation location,
                                     ObjectIdentifierValue parent,
                                     string name,
                                     int value)
                : base("OBJECT IDENTIFIER")
        {
            this.parent = parent;
            this.name = name;
            this.value = value;
            if (parent.getChildByValue(value) != null)
            {
                throw new MibException(location,
                                       "cannot add duplicate OID " +
                                       "children with value " + value);
            }
            parent.addChild(null, location, this);
        }

        /**
         * Creates a new object identifier value.
         *
         * @param location       the declaration file location
         * @param parent         the component parent
         * @param name           the component name, or null
         * @param value          the component value
         */
        public ObjectIdentifierValue(FileLocation location,
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

            ValueReference vref = null;
            ObjectIdentifierValue oid;

            if (parent == null)
            {
                return this;
            }
            else if (parent is ValueReference)
            {
                vref = (ValueReference)parent;
            }
            parent = parent.Initialize(log, type);
            if (vref != null)
            {
                if (parent is ObjectIdentifierValue)
                {
                    oid = (ObjectIdentifierValue)parent;
                    oid.addChild(log, location, this);
                }
                else
                {
                    throw new MibException(vref.getLocation(),
                                           "referenced value is not an " +
                                           "object identifier");
                }
            }
            location = null;
            if (parent is ObjectIdentifierValue)
            {
                return ((ObjectIdentifierValue)parent).getChildByValue(value);
            }
            else
            {
                return this;
            }
        }

        /**
         * Creates a value reference to this value. The value reference
         * is normally an identical value. Only certain values support
         * being referenced, and the default implementation of this
         * method throws an exception.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @return the MIB value reference
         *
         * @since 2.2
         */
        public override MibValue CreateReference()
        {
            return this;
        }

        /**
         * Clears and prepares this value for garbage collection. This
         * method will recursively clear any associated types or values,
         * making sure that no data structures references this object.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         */
        public override void Clear()
        {
            Mib mib;

            // Recursively clear all children in same MIB
            if (children != null)
            {
                mib = getMib();
                foreach(var c in children)
                    c.Clear();
            }

            // Remove parent reference if all children were cleared 
            if (getChildCount() <= 0)
            {
                if (parent != null)
                {
                    getParent().children.Remove(this);
                    parent = null;
                }
                children = null;
            }

            // Clear other value data
            symbol = null;
            base.Clear();
        }

        /**
         * Compares this object with the specified object for order. This
         * method will only compare the string representations with each
         * other.
         *
         * @param obj            the object to compare to
         *
         * @return less than zero if this object is less than the specified,
         *         zero if the objects are equal, or
         *         greater than zero otherwise
         *
         * @since 2.6
         */
        public int CompareTo(Object obj)
        {
            return ToString().CompareTo(obj.ToString());
        }

        /**
         * Checks if this object equals another object. This method will
         * compare the string representations for equality.
         *
         * @param obj            the object to compare with
         *
         * @return true if the objects are equal, or
         *         false otherwise
         */
        public override bool Equals(Object obj)
        {
            return ToString().Equals(obj.ToString());
        }

        /**
         * Returns a hash code for this object.
         *
         * @return a hash code for this object
         */
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /**
         * Returns the parent object identifier value.
         *
         * @return the parent object identifier value, or
         *         null if no parent exists
         */
        public ObjectIdentifierValue getParent()
        {
            return parent as ObjectIdentifierValue;
        }

        /**
         * Returns this object identifier component name.
         *
         * @return the object identifier component name, or
         *         null if the component has no name
         */
        public string getName()
        {
            return name;
        }

        /**
         * Returns this object identifier component value.
         *
         * @return the object identifier component value
         */
        public int getValue()
        {
            return value;
        }

        /**
         * Returns the symbol connected to this object identifier.
         *
         * @return the symbol connected to this object identifier, or
         *         null if no value symbol is connected
         */
        public MibValueSymbol getSymbol()
        {
            return symbol;
        }

        /**
         * Sets the symbol connected to this object identifier.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @param symbol         the value symbol
         */
        public void setSymbol(MibValueSymbol symbol)
        {
            if (name == null)
            {
                name = symbol.getName();
            }
            this.symbol = symbol;
        }

        /**
         * Returns the MIB that this object identifer is connected to.
         * This method simply returns the symbol MIB.
         *
         * @return the symbol MIB, or
         *         null if no symbol has been set
         */
        private Mib getMib()
        {
            return symbol == null ? null : symbol.getMib();
        }

        /**
         * Returns the number of child object identifier values.
         *
         * @return the number of child object identifier values
         */
        public int getChildCount()
        {
            return children == null ? 0 : children.Count;
        }

        /**
         * Returns a child object identifier value. The children are
         * ordered by their value, not necessarily in the order in which
         * they appear in the original MIB file.
         *
         * @param index          the child position, 0 <= index < count
         *
         * @return the child object identifier value, or
         *         null if not found
         */
        public ObjectIdentifierValue getChild(int index)
        {
            return children[index];
        }

        /**
         * Returns a child object identifier value. The children are
         * searched by their component names. This method uses linear
         * search and therefore has time complexity O(n). Note that most
         * OID:s don't have a component name, but only an associated
         * symbol.
         *
         * @param name           the child name
         *
         * @return the child object identifier value, or
         *         null if not found
         *
         * @since 2.5
         */
        public ObjectIdentifierValue getChildByName(string name)
        {
            return children.Where(c => name.Equals(c.getName())).FirstOrDefault();
        }

        /**
         * Returns a child object identifier value. The children are
         * searched by their numerical value. This method uses binary
         * search and therefore has time complexity O(log(n)).
         *
         * @param value          the child value
         *
         * @return the child object identifier value, or
         *         null if not found
         *
         * @since 2.5
         */
        public ObjectIdentifierValue getChildByValue(int value)
        {
            ObjectIdentifierValue child;
            int low = 0;
            int high = children.Count;
            int pos;

            if (low < (long)value && ((long) value) <= high)
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
                child = children[(int)pos];
                if (child.getValue() == value)
                {
                    return child;
                }
                else if (child.getValue() < value)
                {
                    low = (int) pos + 1;
                }
                else
                {
                    high = (int) pos;
                }
                pos = (low + high) / 2;
            }
            return null;
        }

        /**
         * Returns an array of all child object identifier values. The
         * children are ordered by their value, not necessarily in the
         * order in which they appear in the original MIB file.
         *
         * @return the child object identifier values
         *
         * @since 2.3
         */
        public IEnumerable<ObjectIdentifierValue> getAllChildren()
        {
            return children;
        }

        /**
         * Adds a child component. The children will be inserted in the
         * value order. If a child with the same value has already been
         * added, the new child will be merged with the previous one (if
         * possible) and the resulting child will be returned.
         *
         * @param log            the MIB loader log
         * @param location       the file location on error
         * @param child          the child component
         *
         * @return the child object identifier value added
         *
         * @throws MibException if an irrecoverable conflict between two
         *             children occurred
         */
        private ObjectIdentifierValue addChild(MibLoaderLog log,
                                               FileLocation location,
                                               ObjectIdentifierValue child)
        {

            ObjectIdentifierValue value;
            int i = children.Count;

            // Insert child in value order, searching backwards to 
            // optimize the most common case (ordered insertion)
            while (i > 0)
            {
                value = children[i - 1];
                if (value.getValue() == child.getValue())
                {
                    value = value.merge(log, location, child);
                    children[i - 1] = value;
                    return value;
                }
                else if (value.getValue() < child.getValue())
                {
                    break;
                }
                i--;
            }
            children.Insert(i, child);
            return child;
        }

        /**
         * Adds all the children from another object identifier value.
         * The children are not copied, but actually transfered from the
         * other value. If this value lacks a name component, it will be
         * set from other value. This operation thus corresponds to a
         * merge and thus can only be made under certain conditions. For
         * example, no child OID:s may have name conflicts. It is assumed
         * that the other OID has the same numerical value as this one.
         *
         * @param log            the MIB loader log
         * @param location       the file location on error
         * @param parent         the OID parent value for the children
         *
         * @throws MibException if an irrecoverable conflict between two
         *             children occurred
         */
        private void addChildren(MibLoaderLog log,
                                 FileLocation location,
                                 ObjectIdentifierValue parent)
        {

            ObjectIdentifierValue child;
            string msg;

            if (name == null)
            {
                name = parent.name;
            }
            else if (parent.name != null && !parent.name.Equals(name))
            {
                msg = "OID component '" + parent.name + "' was previously " +
                      "defined as '" + name + "'";
                if (log == null)
                {
                    throw new MibException(location, msg);
                }
                else
                {
                    log.addWarning(location, msg);
                }
            }
            if (parent.symbol != null)
            {
                throw new MibException(location,
                                       "INTERNAL ERROR: OID merge with " +
                                       "symbol reference already set");
            }
            for (int i = 0; i < parent.children.Count; i++)
            {
                child = parent.children[i];
                child.parent = this;
                addChild(log, location, child);
            }
            parent.children = null;
        }

        /**
         * Merges this object identifier value with another one. One of
         * the two objects will be discarded and the other will be used
         * as the merge destination and returned. Note that this
         * operation modifies both this value and the specified value.
         * The merge can only be made under certain conditions, for
         * example that no child OID:s have name conflicts. It is also
         * assumed that the two OID:s have the same numerical value.
         *
         * @param log            the MIB loader log
         * @param location       the file location on error
         * @param value          the OID value to merge with
         *
         * @return the merged object identifier value
         *
         * @throws MibException if the merge couldn't be performed due to
         *             some conflict or invalid state
         */
        private ObjectIdentifierValue merge(MibLoaderLog log,
                                            FileLocation location,
                                            ObjectIdentifierValue value)
        {

            if (symbol != null || (value.symbol == null && children.Count > 0))
            {
                addChildren(log, location, value);
                return this;
            }
            else
            {
                value.addChildren(log, location, this);
                return value;
            }
        }

        /**
         * Returns a string representation of this value. The string will
         * contain the full numeric object identifier value with each
         * component separated with a dot ('.').
         *
         * @return a string representation of this value
         */
        public Object toObject()
        {
            return ToString();
        }

        /**
         * Returns a string representation of this value. The string will
         * contain the full numeric object identifier value with each
         * component separated with a dot ('.').
         *
         * @return a string representation of this value
         */
        public override string ToString()
        {
            StringBuilder builder;

            if (cachedNumericValue == null)
            {
                builder = new StringBuilder();
                if (parent != null)
                {
                    builder.Append(parent.ToString());
                    builder.Append(".");
                }
                builder.Append(value);
                cachedNumericValue = builder.ToString();
            }
            return cachedNumericValue;
        }

        /**
         * Returns a detailed string representation of this value. The
         * string will contain the full numeric object identifier value
         * with optional names for each component.
         *
         * @return a detailed string representation of this value
         */
        public string ToDetailString()
        {
            StringBuilder builder = new StringBuilder();

            if (parent is ObjectIdentifierValue) {
                builder.Append(((ObjectIdentifierValue)parent).ToDetailString());
                builder.Append(".");
            }
            if (name == null)
            {
                builder.Append(value);
            }
            else
            {
                builder.Append(name);
                builder.Append("(");
                builder.Append(value);
                builder.Append(")");
            }
            return builder.ToString();
        }

        /**
         * Returns an ASN.1 representation of this value. The string will
         * contain references to any parent OID value that can be found.
         *
         * @return an ASN.1 representation of this value
         * 
         * @since 2.6
         */
        public string toAsn1String()
        {
            StringBuilder builder = new StringBuilder();
            ObjectIdentifierValue  oivref;

            if (parent is ObjectIdentifierValue) {
            oivref = (ObjectIdentifierValue)parent;
                if (oivref.getSymbol() == null) {
                    builder.Append(oivref.toAsn1String());
                } else {
                    builder.Append(oivref.getSymbol().getName());
                }
                builder.Append(" ");
            }
            if (name == null || getSymbol() != null)
            {
                builder.Append(value);
            }
            else
            {
                builder.Append(name);
                builder.Append("(");
                builder.Append(value);
                builder.Append(")");
            }
            return builder.ToString();
        }

        public override int CompareTo(MibValue other)
        {
            throw new NotImplementedException();
        }
    }
}
