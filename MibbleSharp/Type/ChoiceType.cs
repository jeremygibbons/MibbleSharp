// <copyright file="ChoiceType.cs" company="None">
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
    using System.Collections.Generic;
    
    /// <summary>
    /// A choice MIB type. In some other languages this is known as a
    /// union.
    /// </summary>
    public class ChoiceType : MibType
    {
        /// <summary>
        /// The choice elements.
        /// </summary>
        private IList<ElementType> elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceType"/> class.
        /// </summary>
        /// <param name="elements">The list of element types</param>
        public ChoiceType(IList<ElementType> elements) : this(true, elements)
        {
        }

         /// <summary>
         /// Initializes a new instance of the <see cref="ChoiceType"/> class.
         /// </summary>
         /// <param name="primitive">The primitive type flag</param>
         /// <param name="elements">The list of element types</param>
        private ChoiceType(bool primitive, IList<ElementType> elements) 
            : base("CHOICE", primitive)
        {
            this.elements = elements;
        }

        /// <summary>
        /// Gets all the element types. These are the types that the
        /// choice type consists of.
        /// </summary>
        public IList<ElementType> AllElements
        {
            get
            {
                return this.elements;
            }
        }

        /// <summary>
        /// Initializes the MIB type. This will remove all levels of
        /// indirection present, such as references to types or values. No
        /// information is lost by this operation. This method may modify
        /// this object as a side-effect, and will return the basic
        /// type.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        /// <param name="symbol">The MIB symbol containing this type</param>
        /// <param name="log">The MIB loader log</param>
        /// <returns>The basic MIB type</returns>
        /// <exception cref="MibException">If an error occurred during initialization</exception>
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            foreach (ElementType elem in this.elements)
            {
                elem.Initialize(symbol, log);
            }

            return this;
        }

        /// <summary>
        /// Creates a type reference to this type. The type reference is
        /// normally an identical type, but with the primitive flag set to
        /// false. Only certain types support being referenced, and the
        /// default implementation of this method throws an exception.
        /// </summary>
        /// <remarks>
        /// This is an internal method that should
        /// only be called by the MIB loader.
        /// </remarks>
        /// <returns>The MIB type reference</returns>
        public override MibType CreateReference()
        {
            ChoiceType type = new ChoiceType(false, this.elements);
            type.SetTag(true, this.Tag);
            return type;
        }

        /// <summary>
        /// Checks if the specified value is compatible with this type. A
        /// values is considered compatible with this type, if it is
        /// compatible with any single type in the union.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is compatible, false if not</returns>
        public override bool IsCompatible(MibValue value)
        {
            foreach (ElementType elem in this.elements)
            {
                if (elem.IsCompatible(value))
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>
        /// A string representation of this object
        /// </returns>
        public override string ToString()
        {
            return base.ToString() + " " + this.elements.ToString();
        }
    }
}