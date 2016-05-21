// <copyright file="MibTypeTag.cs" company="None">
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
    using System.Text;

    /// <summary>
    /// A MIB type tag. The type tag consists of a category and value.
    /// Together these two numbers normally identifies a type uniquely, as
    /// all primitive and most (if not all) SNMP types (such as <c>IpAddress</c>
    /// and similar) have type tags assigned to them. Type tags may also
    /// be chained together in a list, in order to not loose information.
    /// Whether to replace or to chain a type tag is determined by the
    /// EXPLICIT or IMPLICIT keywords in the MIB file.
    /// </summary>
    public class MibTypeTag
    {
        /// <summary>
        /// The universal type tag category. This is the type tag category
        /// used for the ASN.1 primitive types.
        /// </summary>
        public static readonly int UniversalCategory = 0;

        /// <summary>The application type tag category.</summary>
        public static readonly int ApplicationCategory = 1;

        /// <summary>
        /// The context specific type tag category. This is the default
        /// type tag category if no other category was specified.
        /// </summary>
        public static readonly int ContextSpecificCategory = 2;

        /// <summary>
        /// The private type tag category.
        /// </summary>
        public static readonly int PrivateCategory = 3;

        /// <summary>
        /// The universal boolean type tag.
        /// </summary>
        public static readonly MibTypeTag Boolean = 
            new MibTypeTag(UniversalCategory, 1);

        /// <summary>
        /// The universal integer type tag.
        /// </summary>
        public static readonly MibTypeTag Integer =
            new MibTypeTag(UniversalCategory, 2);

        /// <summary>
        /// The universal bit string type tag.
        /// </summary>
        public static readonly MibTypeTag BitString =
            new MibTypeTag(UniversalCategory, 3);

        /// <summary>
        /// The universal octet string type tag.
        /// </summary>
        public static readonly MibTypeTag OctetString =
            new MibTypeTag(UniversalCategory, 4);

        /// <summary>
        /// The universal null type tag.
        /// </summary>
        public static readonly MibTypeTag Null =
            new MibTypeTag(UniversalCategory, 5);

        /// <summary>
        /// The universal object identifier type tag.
        /// </summary>
        public static readonly MibTypeTag ObjectIdentifier =
            new MibTypeTag(UniversalCategory, 6);

        /// <summary>
        /// The universal real type tag.
        /// </summary>
        public static readonly MibTypeTag Real =
            new MibTypeTag(UniversalCategory, 9);

        /// <summary>
        /// The universal sequence and sequence of type tag.
        /// </summary>
        public static readonly MibTypeTag Sequence =
            new MibTypeTag(UniversalCategory, 16);

        /// <summary>
        /// The universal set type tag.
        /// </summary>
        public static readonly MibTypeTag Set =
            new MibTypeTag(UniversalCategory, 17);

        /// <summary>
        /// The tag category
        /// </summary>
        private int category;

        /// <summary>
        /// The tag value
        /// </summary>
        private int value;

        /// <summary>
        /// The next tag in the tag chain
        /// </summary>
        private MibTypeTag next = null;

        /// <summary>Initializes a new instance of the <see cref="MibTypeTag"/> class.</summary>
        /// <param name="category">The type tag category</param>
        /// <param name="value">The type tag value</param>
        public MibTypeTag(int category, int value)
        {
            this.category = category;
            this.value = value;
        }

        /// <summary>
        /// Gets the type tag category. The category value corresponds
        /// to one of the defined category constants.
        /// </summary>
        /// <see cref="UniversalCategory"/>
        /// <see cref="ApplicationCategory"/>
        /// <see cref="ContextSpecificCategory"/>
        /// <see cref="PrivateCategory"/>
        public int Category
        {
            get
            {
                return this.category;
            }
        }

        /// <summary>Gets the type tag value. The tag category and value
        /// normally identifies a type uniquely.
        /// </summary>
        public int Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Gets or sets the next tag in the tag chain
        /// </summary>
        public MibTypeTag Next
        {
            get
            {
                return this.next;
            }

            set
            {
                this.next = value;
            }
        }

        /// <summary>
        /// Checks if this type tag equals another object. This method
        /// will only return true if the other object is a type tag with
        /// the same category and value numbers.
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if the objects are equal, false if not</returns>
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            MibTypeTag tag = obj as MibTypeTag;
            if (tag == null)
            {
                return false;
            }

            return this.Equals(tag);
        }

        /// <summary>
        /// Checks if this type tag equals another object. This method
        /// will only return true if the other object is a type tag with
        /// the same category and value numbers.
        /// </summary>
        /// <param name="m">The MibTypeTag to compare to</param>
        /// <returns>True if the tags are equal, false if not</returns>
        public bool Equals(MibTypeTag m)
        {
            return this.category == m.category && this.value == m.value;
        }

        /// <summary>
        /// Checks if this type tag has the specified category and
        /// value numbers.
        /// </summary>
        /// <param name="category">The category to compare to</param>
        /// <param name="value">The value number to compare to</param>
        /// <returns>True if the category and value match with those of the object, false if not</returns>
        public bool Equals(int category, int value)
        {
            return this.category == category && this.value == value;
        }

        /// <summary>
        /// Returns the hash code value for the object. This method is
        /// re-implemented to fulfill the contract of returning the same
        /// hash code for objects that are considered equal.
        /// </summary>
        /// <returns>The hash code value for the object</returns>
        public override int GetHashCode()
        {
            return (this.category << 8) + this.value;
        }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("[");
            if (this.category == UniversalCategory)
            {
                buffer.Append("UNIVERSAL ");
            }
            else if (this.category == ApplicationCategory)
            {
                buffer.Append("APPLICATION ");
            }
            else if (this.category == PrivateCategory)
            {
                buffer.Append("PRIVATE ");
            }

            buffer.Append(this.value);
            buffer.Append("]");

            if (this.next != null)
            {
                buffer.Append(" ");
                buffer.Append(this.next.ToString());
            }

            return buffer.ToString();
        }
    }
}
